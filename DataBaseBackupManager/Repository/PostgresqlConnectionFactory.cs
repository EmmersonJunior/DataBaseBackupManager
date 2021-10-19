using DataBaseBackupManager.Interfaces;
using DataBaseBackupManager.Models;
using System.Collections.Generic;

namespace DataBaseBackupManager.Repository
{
    public class PostgresqlConnectionFactory : IConnectionFactory
    {
        /// <summary>
        /// Initialize a defined number of connections.
        /// </summary>
        /// <param name="connectionPoolSize">The number of connections to be initialized.</param>
        /// <param name="dbCredentials">The database credentials for connecting.</param>
        /// <returns>A list of connections.</returns>
        IList<Connection> IConnectionFactory.CreateConnnections(int connectionPoolSize, IDbCredentials dbCredentials)
        {
            IList<Connection> postgreSqlConnections = new List<Connection>();

            for (int i = 0; i < connectionPoolSize; i++)
            {
                postgreSqlConnections.Add(new PostgreSqlConnection(i, dbCredentials));
            }
            return postgreSqlConnections;
        }
    }
}
