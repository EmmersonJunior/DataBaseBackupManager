using System;
using System.Threading;

namespace DataBaseBackupManager.Models
{
    /// <summary>
    /// Hold informations about a connection.
    /// </summary>
    public abstract class Connection
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The connection sequential number.</param>
        protected Connection(int id)
        {
            Occupied = false;
            Identifier = Guid.NewGuid();
            Id = id;
        }

        /// <summary>
        /// Connection is available.
        /// </summary>
        protected bool Occupied { get; set; }

        /// <summary>
        /// The connection number.
        /// </summary>
        internal int Id { get; set; }

        /// <summary>
        /// The Task where the connection is being used.
        /// </summary>
        internal Guid Identifier { get; set; }

        /// <summary>
        /// The Thread unique identifier.
        /// </summary>
        internal int? ThreadId { get; set; }

        /// <summary>
        /// Check if a connection is active and not fetching data.
        /// </summary>
        /// <returns>True of connection is Idle.</returns>
        internal abstract bool IsConnectionIdle();

        /// <summary>
        /// Connect to the database.
        /// </summary>
        internal abstract void Connect();

        /// <summary>
        /// Closes the connection.
        /// </summary>
        internal abstract void Close();

        /// <summary>
        /// Set the connection as busy.
        /// </summary>
        internal void OccupyConnection()
        {
            Occupied = true;
            ThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// Sets the connection as free to be used.
        /// </summary>
        internal void LeaveConnection()
        {
            Occupied = false;
            ThreadId = null;
        }
    }
}
