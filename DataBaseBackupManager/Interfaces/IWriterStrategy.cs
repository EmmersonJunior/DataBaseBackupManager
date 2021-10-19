using System;

namespace DataBaseBackupManager.Interfaces
{
    /// <summary>
    /// Maps the methods for every writer strategy.
    /// </summary>
    public interface IWriterStrategy : IDisposable
    {
        /// <summary>
        /// Write the STDOUT using a defined strategy.
        /// </summary>
        internal void Write(string line);
    }
}
