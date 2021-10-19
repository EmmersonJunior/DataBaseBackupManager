using DataBaseBackupManager.Abstractions;
using DataBaseBackupManager.Exceptions;
using DataBaseBackupManager.Interfaces;
using DataBaseBackupManager.Models;
using DataBaseBackupManager.Services;
using DataBaseBackupManager.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DataBaseBackupManager.Repository
{
    public class PostgreSqlRestoreManager : SqlRestoreManagerBase
    {
        #region constants
        private const int PAGE_INSERT_OFFSET = 500;
        #endregion

        private readonly IDataBaseDao _dataBaseDao;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="credentials">The credentials for DB connection.</param>
        /// <param name="connectionFactory">The database connection manager.</param>
        /// <param name="dataBaseDao">Methods to access DB information for dump and restoring.</param>
        /// <param name="readerStrategyFactory">Maps the methods for every reader strategy.</param>
        /// <param name="logger">The logging instance.</param>
        public PostgreSqlRestoreManager(RestoreDbCredentials credentials, IConnectionFactory connectionFactory, IDataBaseDao dataBaseDao, IReaderStrategyFactory readerStrategyFactory, ILoggerFactory logger)
            : base(credentials, connectionFactory, readerStrategyFactory, logger)
        {
            _dataBaseDao = dataBaseDao;
        }

        /// <summary>
        /// Restore Database using generation script.
        /// </summary>
        internal override void RestoreDataBase()
        {
            string generationCommand =
                $"{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "set" : "export")} PGPASSWORD={DbCredentials.Password}\n" +
                $"{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ThirdLibs\\pg_restore.exe " : "pg_restore")} --no-owner --clean --host {DbCredentials.Host} --port {DbCredentials.Port} --username {DbCredentials.User} --dbname {DbCredentials.DataBase} {BakupStorageFolderManager.GetBackupFolder()}\\{DbCredentials.BackupFileName}";
            string batFilePath = $"{BakupStorageFolderManager.GetBackupFolder()}\\RESTORE_COMMAND.{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bat" : "sh")}";

            try
            {
                string batchContent = "";
                batchContent += $"{generationCommand}";
                File.WriteAllText(batFilePath, batchContent, Encoding.ASCII);
                ProcessExecutor.Run(batFilePath);
            }
            catch (Exception e)
            {
                IList<string> exceptions = e.ToString().Split("\r\n");

                foreach (string exception in exceptions)
                {
                    Logger.LogWarning(exception);
                }
            }
        }

        /// <summary>
        /// Restore a backup to a database.
        /// </summary>
        /// <returns>The task be executed.</returns>
        internal override IList<Table> GetTables()
        {
            Connection conn = ConnectionPool.GetConnection();
            IList<string> tableNames = _dataBaseDao.GetTables(DbCredentials.Schema, conn);
            IList<Table> tables = _dataBaseDao.GetTableInformations(DbCredentials.Schema, tableNames, conn);
            conn.LeaveConnection();
            tables = tables.SetTablePriority();

            return tables;
        }

        /// <summary>
        /// Restore a table data.
        /// </summary>
        /// <param name="table">The table to be restored.</param>
        internal override Table GetTable(Table table, IReaderStrategy reader)
        {
            IList<string> columnNamesOrdered = GetSortedColumns(reader);
            table.SetColumnNumberBy(columnNamesOrdered);
            table.PageInsertOffset = PAGE_INSERT_OFFSET;

            return table;
        }

        /// <summary>
        /// Drop a table that does not exist on backup bunch.
        /// </summary>
        /// <param name="table">The table data.</param>
        internal override void DropTable(Table table)
        {
            Connection conn = ConnectionPool.GetConnection();
            _dataBaseDao.DropTable(table, conn);
            conn.LeaveConnection();
        }

        /// <summary>
        /// Restore a band of rows of a table.
        /// </summary>
        /// <param name="table">The table to receive data.l</param>
        /// <param name="lines">The lines values.</param>
        internal override void RestoreRow(Table table, IList<IDictionary<Column, string>> lines)
        {
            try
            {
                string lineInsertion = FormatField(lines, table);

                if (!string.IsNullOrWhiteSpace(lineInsertion))
                {
                    string header = FormatHeader(table);
                    Connection connection = ConnectionPool.GetConnection();
                    _dataBaseDao.InsertData(table.Name, header, lineInsertion, connection);
                    connection.LeaveConnection();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error when restoring table [{table.Name}]", ex);
            }
        }

        private string FormatField(IList<IDictionary<Column, string>> lines, Table table)
        {
            StringBuilder formattedValues = new StringBuilder();

            foreach (IDictionary<Column, string> columnValues in lines)
            {
                StringBuilder formattedValue = new StringBuilder();

                foreach (Column columnValueKey in columnValues.Keys)
                {
                    try
                    {
                        PostgresColumn postgresColumn = (PostgresColumn)columnValueKey;
                        postgresColumn.AppendColumnValue(ref formattedValue, columnValues[columnValueKey]);
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new ColumnNotFoundException($"The Column [{columnValueKey.Name}] of table [{table.Name}] was not found", ex);
                    }
                }
                formattedValues.Append(formattedValues.Length > 0 ? $", ({formattedValue})" : $"({formattedValue})");
            }
            return formattedValues.ToString();
        }

        private string FormatHeader(Table table)
        {
            string[] values = table.Columns
                .Where(col => !col.Excluded)
                .OrderBy(col => col.ColumnNumber)
                .Select(column => column.Name)
                .ToArray();

            string formattedValue = $"(\"{string.Join("\", \"", values)}\")";

            return formattedValue;
        }

        /// <summary>
        /// Gets the indexes information.
        /// </summary>
        /// <returns>An array of indexes.</returns>
        internal override IList<DbIndex> GetIndexes()
        {
            Connection conn = ConnectionPool.GetConnection();
            IList<DbIndex> indexes = _dataBaseDao.GetIndexes(DbCredentials.Schema, conn);
            conn.LeaveConnection();

            return indexes;
        }

        /// <summary>
        /// Removes indexes temporally until insertion completed.
        /// </summary>
        /// <param name="indexes">The indexes informations.</param>
        internal override void RemoveIndexes(IList<DbIndex> indexes)
        {
            Connection conn = ConnectionPool.GetConnection();
            _dataBaseDao.DropIndexes(indexes, conn);
            conn.LeaveConnection();
        }

        /// <summary>
        /// Recreate the indexes after insertion.
        /// </summary>
        /// <param name="indexes">The indexes informations.</param>
        internal override void RecreateIndexes(IList<DbIndex> indexes)
        {
            Connection conn = ConnectionPool.GetConnection();
            _dataBaseDao.RecreateIndexes(indexes, conn);
            conn.LeaveConnection();
        }
    }
}
