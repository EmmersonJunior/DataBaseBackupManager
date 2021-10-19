using DataBaseBackupManager.Interfaces;
using DataBaseBackupManager.Models;

namespace DataBaseBackupManager.Repository
{
    public class LocalFileWriterFactory : IWriterStrategyFactory
    {
        /// <summary>
        /// Instantiate a new writer.
        /// </summary>
        /// <param name="table">The table to be written.</param>
        /// <returns>A new Writer Strategy.</returns>
        IWriterStrategy IWriterStrategyFactory.GetWriterStrategy(Table table)
        {
            IWriterStrategy writerStrategy = new LocalFileWritterStrategy(table);

            return writerStrategy;
        }
    }
}
