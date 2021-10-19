using DataBaseBackupManager.Interfaces;
using DataBaseBackupManager.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackupManager.Abstractions
{
    /// <summary>
    /// Maps the common methods and properties for Backup Manager.
    /// </summary>
    public abstract class SqlBackupManagerBase : SqlBackupRestoreManagerBase
    {
        protected readonly IWriterStrategyFactory WriterStrategyFactory;

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="dbCredentials">The credentials for DB connection.</param>
        /// <param name="connectionFactory">The database connection manager.</param>
        /// <param name="writerStrategyFactory">Creates the writer strategy to backup.</param>
        /// <param name="connectionPoolSize">The quantity of connections at the DataBase.</param>
        protected SqlBackupManagerBase(BackupDbCredentials dbCredentials, IConnectionFactory connectionFactory, IWriterStrategyFactory writerStrategyFactory, ILoggerFactory logger, int connectionPoolSize = 10)
            : base(dbCredentials, connectionFactory, logger, connectionPoolSize)
        {
            WriterStrategyFactory = writerStrategyFactory;
        }

        /// <summary>
        /// Starts the dump flow.
        /// </summary>
        public void StartDump()
        {
            BackupStartedAt = DateTime.Now;
            GenerateDataBase();
            Logger.LogInformation(LogConstants.DATABASE_BACKUP);
            IList<Table> tablesToDump = GetTables().Where(table => !table.Excluded).ToList();
            Logger.LogInformation(LogConstants.TABLES_INFO_FETCHED);

            foreach (Table table in tablesToDump)
            {
                Logger.LogInformation(LogConstants.BACKUP_TABLE + table.Name);
                using (IWriterStrategy writerStrategy = WriterStrategyFactory.GetWriterStrategy(table))
                {
                    TableQueryInfo tablePageOffsetModel = DumpTable(table);
                    Parallel.For(0, tablePageOffsetModel.QueryAmount, count =>
                    {
                        Column[] columnsToRetrieve = table.Columns.Where(column => !column.Excluded).ToArray();
                        DumpRows(count, table.Name, columnsToRetrieve, tablePageOffsetModel.Offset, writerStrategy);
                    });
                }
            }
        }

        /// <summary>
        /// Create the Database generation script.
        /// </summary>
        internal abstract void GenerateDataBase();

        /// <summary>
        /// Extract a backup from a database.
        /// </summary>
        internal abstract IList<Table> GetTables();

        /// <summary>
        /// Get all data from a specific table bit by bit.
        /// It splits the table into bands and fetch the data asynchronously for each band.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>The query to be executed.</returns>
        internal abstract TableQueryInfo DumpTable(Table table);

        /// <summary>
        /// Get data from a band of the table.
        /// </summary>
        /// <param name="increment">The band number to be retrieved.</param>
        /// <param name="table">The table name.</param>
        /// <param name="columns">The column names.</param>
        /// <param name="offset">The amount of rows in a band.</param>
        /// <param name="writerStrategy">The strategy to write the retrieved data.</param>
        internal abstract void DumpRows(long increment, string table, Column[] columns, long offset, IWriterStrategy writerStrategy);
    }
}
