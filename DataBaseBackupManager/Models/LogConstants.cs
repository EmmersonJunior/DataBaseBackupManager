namespace DataBaseBackupManager.Models
{
    /// <summary>
    /// Holds the constants for logging.
    /// </summary>
    internal static class LogConstants
    {
        internal const string DATABASE_BACKUP = "Starting DataBase backup.";
        internal const string BACKUP_TABLE = "Performing backup of table => ";
        internal const string DATABASE_RESTORING = "Starting DataBase restoring..";
        internal const string BACKUP_CREATED = "Database schema backup created..";
        internal const string INDEX_FETCHED = "Indexes information fetched..";
        internal const string TABLES_INFO_FETCHED = "Tables information fetched..";
        internal const string INDEXES_REMOVED = "Indexes removed.";
        internal const string SORTING_TABLES = "Sorting tables by dependency priority.";
        internal const string TABLE_PRIORITY = "Restoring table of priority ";
        internal const string RESTORING_TABLE = "Performing restoring of table => ";
        internal const string TABLES_RESTORED = "Tables Restored.";
        internal const string INDEXES_RECREATED = "Indexes recreated.";
    }
}
