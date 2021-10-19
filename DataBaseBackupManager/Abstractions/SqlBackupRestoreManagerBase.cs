using DataBaseBackupManager.Interfaces;
using DataBaseBackupManager.Models;
using DataBaseBackupManager.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DataBaseBackupManager.Abstractions
{
    /// <summary>
    /// Maps the common methods and properties for Backup and Restore Manager.
    /// </summary>
    public abstract class SqlBackupRestoreManagerBase : IDisposable
    {
        protected readonly IDbCredentials DbCredentials;
        protected readonly ConnectionPool ConnectionPool;
        protected readonly ILogger<SqlBackupRestoreManagerBase> Logger;
        protected DateTime? RestoreStartedAt;
        protected DateTime? BackupStartedAt;
        private bool disposedValue;

        /// <summary>
        /// The base constructor.
        /// </summary>
        /// <param name="dbCredentials">The credentials for DB connection.</param>
        /// <param name="connectionFactory">The database connection manager.</param>
        /// <param name="connectionPoolSize">The quantity of connections at the DataBase.</param>
        protected SqlBackupRestoreManagerBase(IDbCredentials dbCredentials, IConnectionFactory connectionFactory, ILoggerFactory logger, int connectionPoolSize)
        {
            IList<Connection> connections = connectionFactory.CreateConnnections(connectionPoolSize, dbCredentials);
            ConnectionPool = new ConnectionPool(connections, logger);
            DbCredentials = dbCredentials;
            Logger = logger.CreateLogger<SqlBackupRestoreManagerBase>();
        }

        /// <summary>
        /// Dispose once the connections.
        /// </summary>
        /// <param name="disposing">True to close the connections.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ConnectionPool.CloseConnections();

                    if (RestoreStartedAt == null && BackupStartedAt != null)
                    {
                        Logger.LogWarning($"{(DateTime.Now - (DateTime)BackupStartedAt).TotalMinutes} minutes for backup execution.");
                    }
                    else if (RestoreStartedAt != null)
                    {
                        Logger.LogWarning($"{(DateTime.Now - (DateTime)RestoreStartedAt).TotalMinutes} minutes for restore execution.");
                    }
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The string representation of instance.
        /// </summary>
        /// <returns>instance name.</returns>
        public override string ToString()
        {
            return "[Backup Restore Manager]";
        }
    }
}
