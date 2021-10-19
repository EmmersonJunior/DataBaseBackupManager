using System;
using System.Collections.Generic;

namespace DataBaseBackupManager.Interfaces
{
    /// <summary>
    /// Maps the methods for every reader strategy.
    /// </summary>
    public interface IReaderStrategy : IDisposable
    {
        /// <summary>
        /// Read the STDIN using a defined strategy.
        /// Returns a list of lines containing a list of column values.
        /// </summary>
        /// <param name="rows">The rows quantity to be read.</param>
        internal IList<IList<string>> Read(int rows);

        /// <summary>
        /// Get the column names sorted.
        /// </summary>
        /// <returns>The ordered column name.</returns>
        internal IList<string> GetColumnNamesSorted();
    }
}
