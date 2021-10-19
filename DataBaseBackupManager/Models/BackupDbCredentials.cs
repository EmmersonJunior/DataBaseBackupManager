namespace DataBaseBackupManager.Models
{
    /// <summary>
    ///  Maps the DataBase credentials for connection establishing.
    /// </summary>
    public class BackupDbCredentials : DbCredentials
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="host">The address (IP or DNS) of database.</param>
        /// <param name="port"> The database service port.</param>
        /// <param name="database">The schema of database to be connected.</param>
        /// <param name="schema">The schema of database to be connected.</param>
        /// <param name="user">The database user.</param>
        /// <param name="password">The user password.</param>
        /// <param name="backupFileName">The backup file name.</param>
        public BackupDbCredentials(string host, int port, string database, string user, string password, string schema, string backupFileName)
            : base(host, port, database, user, password, schema, backupFileName)
        {
        }
    }
}
