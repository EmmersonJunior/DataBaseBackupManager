using System.Collections.Generic;

namespace DataBaseBackupManager.Interfaces
{
    /// <summary>
    /// Allows the user to make a personalized configuration to backup/restore
    /// </summary>
    public interface IBackupConfiguration
    {
        /// <summary>
        /// The backup/restore database type.
        /// </summary>
        DataBaseType DataBaseType { get; }

        /// <summary>
        /// The backup storage strategy.
        /// </summary>
        BackupStorageStrategy BackupStorageStrategy { get; }

        /// <summary>
        /// Tables that will not be copied/restored.
        /// </summary>
        string[] TablesToExclude { get; }

        /// <summary>
        /// A dictionary containing the columns of a table that will not be copied/restored.
        /// The key must be the table name and the value must be a list of column names.
        /// </summary>
        IDictionary<string, string[]> TableColumnsToExclude { get; }
    }
}
