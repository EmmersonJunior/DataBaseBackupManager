namespace DataBaseBackupManager.Interfaces
{
    /// <summary>
    ///  Maps the DataBase credentials for connection establishing.
    /// </summary>
    public interface IDbCredentials
    {
        /// <summary>
        /// The address (IP or DNS) of database.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The database service port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The schema of database to be connected.
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
