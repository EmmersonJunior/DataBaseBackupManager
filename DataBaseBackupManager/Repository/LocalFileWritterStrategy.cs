using DataBaseBackupManager.Abstractions;
using DataBaseBackupManager.Interfaces;
using DataBaseBackupManager.Models;
using DataBaseBackupManager.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataBaseBackupManager.Repository
{
    /// <summary>
    /// The local file write strategy.
    /// </summary>
    internal class LocalFileWritterStrategy : IWriterStrategy
    {
        #region constants
        private const string TABLES_FOLDER = "DATA";
        #endregion

        private readonly StreamWriter _writer;
        private readonly object _obj = new object();

        /// <summary>
        /// Empty Constructor.
        /// </summary>
        internal LocalFileWritterStrategy()
        {
        }

        /// <summary>
        /// Constructor.
        /// Initialize the writer and writes the table header.
        /// </summary>
        /// <param name="table">The table to yield the file.</param>
        internal LocalFileWritterStrategy(Table table)
        {
            string folderName = BakupStorageFolderManager.GetBackupFolder();
            Directory.CreateDirectory(folderName + $"\\{TABLES_FOLDER}");
            string filePath = $"{folderName}\\{TABLES_FOLDER}\\{table.Name}.csv";
            _writer = new StreamWriter(filePath);
            _writer.WriteLine(GetColumnNameSorted(table.Columns));
        }

        private string GetColumnNameSorted(Column[] columns)
        {
            IList<Column> orderedColumns = columns.OrderBy(col => col.ColumnNumber).ToList();
            string columnHeader = string.Join('|', orderedColumns);

            return columnHeader;
        }

        /// <summary>
        /// CLose the stream.
        /// </summary>
        void IDisposable.Dispose()
        {
            _writer.Dispose();
        }

        /// <summary>
        /// Writes the STDOUT to a local file.
        /// </summary>
        void IWriterStrategy.Write(string line)
        {
            lock (_obj)
            {
                _writer.WriteLine(line);
            }
        }
    }
}
