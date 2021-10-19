using DataBaseBackupManager.Interfaces;
using DataBaseBackupManager.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackupManager.Abstractions
{
    /// <summary>
    /// Maps the common methods and properties for Backup Manager.
    /// </summary>
    public abstract class SqlRestoreManagerBase : SqlBackupRestoreManagerBase
    {
        protected readonly IReaderStrategyFactory ReaderStrategyFactory;

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="dbCredentials">The credentials for DB connection.</param>
        /// <param name="connectionFactory">The database connection manager.</param>
        /// <param name="readerStrategyFactory">Creates the reader strategy to backup.</param>
        /// <param name="connectionPoolSize">The quantity of connections at the DataBase.</param>
        protected SqlRestoreManagerBase(RestoreDbCredentials dbCredentials, IConnectionFactory connectionFactory, IReaderStrategyFactory readerStrategyFactory, ILoggerFactory logger, int connectionPoolSize = 10)
            : base(dbCredentials, connectionFactory, logger, connectionPoolSize)
        {
            ReaderStrategyFactory = readerStrategyFactory;
        }

        /// <summary>
        /// Starts the restore flow.
        /// </summary>
        public void StartRestore()
        {
            RestoreStartedAt = DateTime.Now;
            Logger.LogInformation(LogConstants.DATABASE_RESTORING);
            RestoreDataBase();
            Logger.LogInformation(LogConstants.BACKUP_CREATED);
            IList<DbIndex> indexes = GetIndexes();
            Logger.LogInformation(LogConstants.INDEX_FETCHED);
            IList<Table> tables = GetTables().Where(table => !table.Excluded).ToList();
            Logger.LogInformation(LogConstants.TABLES_INFO_FETCHED);
            RemoveIndexes(indexes);
            Logger.LogInformation(LogConstants.INDEXES_REMOVED);
            IList<int> priorityLevels = tables.Select(table => table.Priority).OrderByDescending(prior => prior).Distinct().ToList();
            Logger.LogInformation(LogConstants.SORTING_TABLES);

            foreach (int priorityLevel in priorityLevels)
            {
                Logger.LogInformation(LogConstants.TABLE_PRIORITY + priorityLevel);
                IList<Table> priorizedTables = tables.Where(table => table.Priority.Equals(priorityLevel)).ToList();
                Parallel.ForEach(priorizedTables, table =>
                {
                    Logger.LogInformation(LogConstants.RESTORING_TABLE + table.Name);
                    try
                    {
                        using (IReaderStrategy reader = ReaderStrategyFactory.GetReaderStrategy(table.Name))
                        {
                            Table columnSortedTable = GetTable(table, reader);
                            IList<IList<string>> lines = reader.Read(columnSortedTable.PageInsertOffset);
                            IList<IDictionary<Column, string>> columnValues = FormatColumnValue(table.Columns, lines);

                            while (lines.Any())
                            {
                                RestoreRow(table, columnValues);
                                lines = reader.Read(columnSortedTable.PageInsertOffset);
                                columnValues = FormatColumnValue(table.Columns, lines);
                            }
                        }
                    }
                    catch (FileNotFoundException ex)
                    {
                        DropTable(table);
                        Logger.LogWarning($"Table file {table.Name} Not found. Table has been dropped.", ex);
                    }
                });
            }
            Logger.LogInformation(LogConstants.TABLES_RESTORED);
            RecreateIndexes(indexes);
            Logger.LogInformation(LogConstants.INDEXES_RECREATED);
        }

        private IList<IDictionary<Column, string>> FormatColumnValue(IList<Column> columns, IList<IList<string>> lines)
        {
            IList<IDictionary<Column, string>> columnValueList = new List<IDictionary<Column, string>>();
            IList<Column> validColumns = columns.Where(col => !col.Excluded).ToList();

            foreach (IList<string> line in lines)
            {
                IDictionary<Column, string> columnValue = new Dictionary<Column, string>();

                for (int i = 0; i < validColumns.Count; i++)
                {
                    Column column = columns.First(col => col.ColumnNumber.Equals(i));

                    if (!column.Excluded)
                    {
                        columnValue.Add(column, line[i]);
                    }
                }
                columnValueList.Add(columnValue);
            }
            return columnValueList;
        }

        /// <summary>
        /// Get the column name sorted.
        /// </summary>
        /// <returns></returns>
        internal IList<string> GetSortedColumns(IReaderStrategy reader)
        {
            return reader.GetColumnNamesSorted();
        }

        /// <summary>
        /// Restore Database using generation script.
        /// </summary>
        internal abstract void RestoreDataBase();

        /// <summary>
        /// Gets the indexes information.
        /// </summary>
        /// <returns>An array of indexes.</returns>
        internal abstract IList<DbIndex> GetIndexes();

        /// <summary>
        /// Removes indexes temporally until insertion completed.
        /// </summary>
        /// <param name="indexes">The indexes informations.</param>
        internal abstract void RemoveIndexes(IList<DbIndex> indexes);

        /// <summary>
        /// Recreate the indexes after insertion.
        /// </summary>
        /// <param name="indexes">The indexes informations.</param>
        internal abstract void RecreateIndexes(IList<DbIndex> indexes);

        /// <summary>
        /// Restore a backup to a database.
        /// </summary>
        /// <returns>The task be executed.</returns>
        internal abstract IList<Table> GetTables();

        /// <summary>
        /// Restore a table data.
        /// </summary>
        /// <param name="table">The table to be restored.</param>
        /// <param name="reader">The backup reader strategy.</param>
        internal abstract Table GetTable(Table table, IReaderStrategy reader);

        /// <summary>
        /// Drop a table that does not exist on backup bunch.
        /// </summary>
        /// <param name="table">The table data.</param>
        internal abstract void DropTable(Table table);

        /// <summary>
        /// Restore a band of rows of a table.
        /// </summary>
        /// <param name="table">The table to receive data.</param>
        /// <param name="lines">The lines values.</param>
        internal abstract void RestoreRow(Table table, IList<IDictionary<Column, string>> lines);
    }
}
