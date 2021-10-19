using DataBaseBackupManager.Models;

namespace DataBaseBackupManager.Interfaces
{
    /// <summary>
    /// Maps the methods for Writer factory.
    /// </summary>
    public interface IWriterStrategyFactory
    {
        /// <summary>
        /// Instantiate a new writer.
        /// </summary>
        /// <param name="table">The table name to be written.</param>
        /// <returns>A new Writer Strategy.</returns>
        internal IWriterStrategy GetWriterStrategy(Table table);
    }
}
