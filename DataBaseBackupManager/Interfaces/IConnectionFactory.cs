using DataBaseBackupManager.Models;
using System.Collections.Generic;

namespace DataBaseBackupManager.Interfaces
{
    /// <summary>
    /// Maps the factory connection methods.
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// Initialize a defined number of connections.
        /// </summary>
        /// <param name="connectionPoolSize">The number of connections to be initialized.</param>
        /// <param name="dbCredentials">The database credentials for connecting.</param>
        /// <returns>A list of connections.</returns>
        internal abstract IList<Connection> CreateConnnections(int connectionPoolSize, IDbCredentials dbCredentials);
    }
}
