using DataBaseBackupManager.Interfaces;
using DataBaseBackupManager.Util;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataBaseBackupManager.Repository
{
    /// <summary>
    /// The local file reader strategy.
    /// </summary>
    internal class LocalFileReaderStrategy : IReaderStrategy
    {
        #region constants
        private const string TABLES_FOLDER = "DATA";
        #endregion

        private readonly StreamReader _reader;
        private readonly object _obj = new object();
        private readonly string _firstLine;

        /// <summary>
        /// Empty constructor.
        /// </summary>
        internal LocalFileReaderStrategy()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="table">The table name to yield the file name.</param>
        internal LocalFileReaderStrategy(string table)
        {
            string folderName = BakupStorageFolderManager.GetBackupFolder();
            string filePath = $"{folderName}\\{TABLES_FOLDER}\\{table}.csv";
            _reader = new StreamReader(filePath);
            _firstLine = _reader.ReadLine();
        }

        /// <summary>
        /// CLose the stream.
        /// </summary>
        void IDisposable.Dispose()
        {
            _reader.Dispose();
        }

        /// <summary>
        /// Get the column names sorted.
        /// </summary>
        /// <returns>The ordered column name.</returns>
        IList<string> IReaderStrategy.GetColumnNamesSorted()
        {
            IList<string> columnOder = _firstLine.Split('|', StringSplitOptions.None);

            return columnOder;
        }

        /// <summary>
        /// Read the STDIN using a defined strategy.
        /// </summary>
        /// <param name="rows">The rows quantity to be read.</param>
        /// <returns>A list of row values. Null if end of file.</returns>
        IList<IList<string>> IReaderStrategy.Read(int rows)
        {
            IList<IList<string>> rowList = new List<IList<string>>();
            lock (_obj)
            {
                for (int i = 0; i < rows; i++)
                {
                    string value = _reader.ReadLine();

                    if (string.IsNullOrEmpty(value))
                    {
                        break;
                    }
                    else
                    {
                        IList<string> values = value.Split('|', StringSplitOptions.None);
                        rowList.Add(values);
                    }
                }
            }
            return rowList;
        }
    }
}
