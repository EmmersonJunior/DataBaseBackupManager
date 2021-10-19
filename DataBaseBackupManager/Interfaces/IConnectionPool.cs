using DataBaseBackupManager.Models;

namespace DataBaseBackupManager.Interfaces
{
    /// <summary>
    /// The methods to make a object usable at connection pool.
    /// </summary>
    internal interface IConnectionPool
    {
        /// <summary>
        /// Starts the connections.
        /// </summary>
        public void StartConnections();

        /// <summary>
        /// Flush all active connections.
        /// </summary>
        public void CloseConnections();

        /// <summary>
        /// Retrieves a connection when it is available. Otherwise, stops the thread.
        /// </summary>
        /// <returns>A connection that is not being used.</returns>
        public Connection GetConnection();
    }
}
