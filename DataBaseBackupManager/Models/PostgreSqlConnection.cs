using DataBaseBackupManager.Interfaces;
using Npgsql;
using System.Collections.Generic;

namespace DataBaseBackupManager.Models
{
    internal class PostgreSqlConnection : Connection
    {
        #region constants
        private const string SPARE_DATABASE = "postgres";
        private const int CONNECTION_TIMEOUT = 300;
        #endregion

        /// <summary>
        /// The connection at PostgreSql.
        /// </summary>
        internal NpgsqlConnection Connection { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The connection sequential number.</param>
        /// <param name="credentials">Maps the DataBase credentials for connection establishing.</param>
        internal PostgreSqlConnection(int id, IDbCredentials credentials, bool spareDataBase = false) : base(id)
        {
            Connection = new NpgsqlConnection($"Host={credentials.Host};port={credentials.Port};Database={(spareDataBase ? SPARE_DATABASE : credentials.DataBase)};Username={credentials.User};Password={credentials.Password};Pooling=false;Timeout={CONNECTION_TIMEOUT};CommandTimeout={CONNECTION_TIMEOUT}");
        }

        /// <summary>
        /// Check if a connection is active and not fetching data.
        /// </summary>
        /// <returns>True of connection is Idle.</returns>
        internal override bool IsConnectionIdle()
        {
            return Connection.FullState == System.Data.ConnectionState.Open && !Occupied;
        }

        /// <summary>
        /// Connect to the database.
        /// </summary>
        internal override void Connect()
        {
            Connection.Open();
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        internal override void Close()
        {
            Connection.Close();
        }
    }
}
