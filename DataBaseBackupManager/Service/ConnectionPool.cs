using DataBaseBackupManager.Interfaces;
using DataBaseBackupManager.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DataBaseBackupManager.Services
{
    /// <summary>
    /// Manages the connections of database.
    /// </summary>
    public class ConnectionPool : IConnectionPool
    {
        private const int MillisecondsTimeout = 5000;
        private const int CONECTION_REQUEST_TIMEOUT = 20;
        private readonly IList<Connection> _connections;
        private readonly ILogger<ConnectionPool> _logger;
        private static readonly object _obj = new object();

        /// <summary>
        /// Constructor.
        /// Initialize the connections and set it as available.
        /// </summary>
        public ConnectionPool(IList<Connection> connections, ILoggerFactory logger)
        {
            _logger = logger.CreateLogger<ConnectionPool>();
            _connections = connections;
            StartConnections();
        }

        /// <summary>
        /// Starts the connections.
        /// </summary>
        public void StartConnections()
        {
            _logger.LogInformation($"Starting {_connections.Count} connecitons at the Database.");
            foreach (Connection connection in _connections)
            {
                connection.Connect();
            }
        }

        /// <summary>
        /// Retrieves a connection when it is available. Otherwise, stops the thread.
        /// </summary>
        /// <returns>A connection that is not being used.</returns>
        public Connection GetConnection()
        {
            Connection connection = null;
            DateTime requestedAt = DateTime.Now;

            while (connection == null)
            {
                lock (_obj)
                {
                    connection = _connections.FirstOrDefault(conn => conn.IsConnectionIdle());
                    if (connection != null)
                    {
                        connection.OccupyConnection();
                    }
                }
                if (connection == null)
                {
                    if (requestedAt.AddSeconds(CONECTION_REQUEST_TIMEOUT) > DateTime.Now)
                    {
                        _logger.LogWarning($"[Warning] connection requested past {CONECTION_REQUEST_TIMEOUT}+ seconds. Do not forget to Leave the connection after usage.");
                        requestedAt = DateTime.Now;
                    }
                    Thread.Sleep(MillisecondsTimeout);
                }
            }
            lock (_obj)
            {
                _logger.LogDebug($"[Conn {connection.Id}] OCUPADA PELA THREAD {connection.ThreadId}");
                return connection;
            }
        }

        /// <summary>
        /// Flush all active connections.
        /// </summary>
        public void CloseConnections()
        {
            lock (_obj)
            {
                foreach (Connection conn in _connections)
                {
                    conn.LeaveConnection();
                    conn.Close();
                }
                _logger.LogInformation($"{_connections.Count} Database connections closed.");
            }
        }
    }
}
