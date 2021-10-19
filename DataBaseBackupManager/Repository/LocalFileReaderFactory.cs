using DataBaseBackupManager.Interfaces;

namespace DataBaseBackupManager.Repository
{
    /// <summary>
    /// The local file writer factory.
    /// </summary>
    public class LocalFileReaderFactory : IReaderStrategyFactory
    {
        /// <summary>
        /// Instantiate a new writer.
        /// </summary>
        /// <param name="table">The table name to be written.</param>
        /// <returns>A new Writer Strategy.</returns>
        IReaderStrategy IReaderStrategyFactory.GetReaderStrategy(string table)
        {
            IReaderStrategy readerStrategy = new LocalFileReaderStrategy(table);

            return readerStrategy;
        }
    }
}
