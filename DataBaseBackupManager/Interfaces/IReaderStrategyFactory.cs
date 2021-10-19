namespace DataBaseBackupManager.Interfaces
{
    /// <summary>
    /// Maps the methods for Reader factory.
    /// </summary>
    public interface IReaderStrategyFactory
    {
        /// <summary>
        /// Instantiate a new reader.
        /// </summary>
        /// <param name="table">The table name to be written.</param>
        /// <returns>A new Writer Strategy.</returns>
        internal IReaderStrategy GetReaderStrategy(string table);
    }
}
