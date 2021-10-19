using DataBaseBackupManager.Interfaces;

namespace DataBaseBackupManager.Models
{
    /// <summary>
    ///  Maps the DataBase credentials for connection establishing.
    /// </summary>
    public class DbCredentials : IDbCredentials
    {
        private const string EMPTY = "";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="host">The address (IP or DNS) of database.</param>
        /// <param name="port"> The database service port.</param>
        /// <param name="database">The Database to be connected.</param>
        /// <param name="schema">The schema of database to be connected.</param>
        /// <param name="user">The database user.</param>
        /// <param name="password">The user password.</param>
        public DbCredentials(string host, int port, string database, string user, string password, string schema, string backupFileName = EMPTY)
        {
            Host = host;
            Port = port;
            DataBase = database;
            Schema = schema;
            User = user;
            Password = password;
            BackupFileName = string.IsNullOrWhiteSpace(backupFileName) ? DataBase : backupFileName;
        }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public DbCredentials()
        {
        }

        /// <summary>
        /// The address (IP or DNS) of database.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The database service port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The Database to be connected.
        /// </summary>
        public string DataBase { get; set; }

        /// <summary>
        /// The schema of database to be connected.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// The database user.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// The user password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The backup file name.
        /// </summary>
        public string BackupFileName { get; set; }
    }
}
