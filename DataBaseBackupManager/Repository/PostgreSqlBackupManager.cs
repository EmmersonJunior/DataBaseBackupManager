using DataBaseBackupManager.Abstractions;
using DataBaseBackupManager.Interfaces;
using DataBaseBackupManager.Models;
using DataBaseBackupManager.Services;
using DataBaseBackupManager.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace DataBaseBackupManager.Repository
{
    /// <summary>
    /// Maps the common methods and properties for POSTGRES Backup Manager.
    /// </summary>
    public class PostgreSqlBackupManager : SqlBackupManagerBase
    {
        #region constants
        private const int PAGE_QUERY_OFFSET = 2000;
        private const int DIVISION_REST = 1;
        #endregion

        private readonly IDataBaseDao _dataBaseDao;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="credentials">The credentials for DB connection.</param>
        /// <param name="connectionFactory">The database connection manager.</param>
        /// <param name="dataBaseDao">Methods to access DB information for dump and restoring.</param>
        /// <param name="writerStrategyFactory">Creates the writer strategy to backup.</param>
        /// <param name="logger">The logging instance.</param>
        public PostgreSqlBackupManager(BackupDbCredentials credentials, IConnectionFactory connectionFactory, IDataBaseDao dataBaseDao, IWriterStrategyFactory writerStrategyFactory, ILoggerFactory logger)
            : base(credentials, connectionFactory, writerStrategyFactory, logger)
        {
            _dataBaseDao = dataBaseDao;
        }

        /// <summary>
        /// Create the Database generation script.
        /// </summary>
        internal override void GenerateDataBase()
        {
            string generationCommand =
                $"{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "set" : "export")} PGPASSWORD={DbCredentials.Password}\n" +
                $"{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ThirdLibs\\pg_dump.exe " : "pg_dump")} -s -x --no-owner --host {DbCredentials.Host} --port {DbCredentials.Port} --username {DbCredentials.User} --format tar --file {BakupStorageFolderManager.GetBackupFolder()}\\{DbCredentials.BackupFileName} {DbCredentials.DataBase}";
            string batFilePath = $"{BakupStorageFolderManager.GetBackupFolder()}\\DUMP_COMMAND.{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bat" : "sh")}";

            try
            {
                string batchContent = "";
                batchContent += $"{generationCommand}";
                File.WriteAllText(batFilePath, batchContent, Encoding.ASCII);
                ProcessExecutor.Run(batFilePath);
            }
            catch (Exception e)
            {
                Logger.LogWarning(e.ToString());
            }
        }

        /// <summary>
        /// Extract a backup from a database.
        /// </summary>
        /// <returns>The task be executed.</returns>
        internal override IList<Table> GetTables()
        {
            Connection conn = ConnectionPool.GetConnection();
            IList<string> tableNames = _dataBaseDao.GetTables(DbCredentials.Schema, conn);
            IList<Table> tables = _dataBaseDao.GetTableInformations(DbCredentials.Schema, tableNames, conn);
            conn.LeaveConnection();

            return tables;
        }

        /// <summary>
        /// Get all data from a specific table bit by bit.
        /// It splits the table into bands and fetch the data asynchronously for each band.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>The query to be executed.</returns>
        internal override TableQueryInfo DumpTable(Table table)
        {
            Connection tableConn = ConnectionPool.GetConnection();
            long count = _dataBaseDao.GetColumnsRowCount(table.Name, tableConn);
            long queryAmount = count > 0 ? (count / PAGE_QUERY_OFFSET) + DIVISION_REST : 0;
            tableConn.LeaveConnection();

            return new TableQueryInfo()
            {
                Count = count,
                QueryAmount = queryAmount,
                Offset = PAGE_QUERY_OFFSET
            };
        }

        /// <summary>
        /// Get data from a band of the table.
        /// </summary>
        /// <param name="increment">The band number to be retrieved.</param>
        /// <param name="table">The table name.</param>
        /// <param name="columns">The column names.</param>
        /// <param name="offset">The amount of rows in a band.</param>
        /// <param name="writerStrategy">The strategy to write the retrieved data.</param>
        internal override void DumpRows(long increment, string table, Column[] columns, long offset, IWriterStrategy writerStrategy)
        {
            Connection conn = ConnectionPool.GetConnection();
            string line = _dataBaseDao.GetDataPagingSelect(columns, table, offset, increment, conn);
            conn.LeaveConnection();
            writerStrategy.Write(line);
        }
    }
}
