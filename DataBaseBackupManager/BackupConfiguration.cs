using DataBaseBackupManager.Interfaces;
using System.Collections.Generic;

namespace DataBaseBackupManager
{
    /// <summary>
    /// A backup/restore configuration.
    /// </summary>
    public class BackupConfiguration : IBackupConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataBaseType">The backup/restore database type.</param>
        /// <param name="backupStorageStrategy">The backup storage strategy.</param>
        /// <param name="tablesToExclude">Tables that will not be copied/restored.</param>
        /// <param name="tableColumnsToExclude">Columns that will not be copied/restored.</param>
        public BackupConfiguration(DataBaseType dataBaseType, BackupStorageStrategy backupStorageStrategy, string[] tablesToExclude, IDictionary<string, string[]> tableColumnsToExclude)
        {
            DataBaseType = dataBaseType;
            BackupStorageStrategy = backupStorageStrategy;
            TablesToExclude = tablesToExclude;
            TableColumnsToExclude = tableColumnsToExclude;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataBaseType">The backup/restore database type.</param>
        /// <param name="backupStorageStrategy">The backup storage strategy.</param>
        public BackupConfiguration(DataBaseType dataBaseType, BackupStorageStrategy backupStorageStrategy)
        {
            DataBaseType = dataBaseType;
            BackupStorageStrategy = backupStorageStrategy;
            TablesToExclude = new string[0];
            TableColumnsToExclude = new Dictionary<string, string[]>();
        }

        /// <summary>
        /// The backup/restore database type.
        /// </summary>
        public DataBaseType DataBaseType { get; }

        /// <summary>
        /// The backup storage strategy.
        /// </summary>
        public BackupStorageStrategy BackupStorageStrategy { get; }

        /// <summary>
        /// Tables that will not be copied/restored.
        /// </summary>
        public string[] TablesToExclude { get; }

        /// <summary>
        /// A dictionary containing the columns of a table that will not be copied/restored.
        /// The key must be the table name and the value must be a list of column names.
        /// </summary>
        public IDictionary<string, string[]> TableColumnsToExclude { get; }
    }
}
