using DataBaseBackupManager.Models;
using System.Collections.Generic;

namespace DataBaseBackupManager.Util
{
    /// <summary>
    /// The parser for index type ENUM.
    /// </summary>
    public static class IndexTypeParser
    {
        private static readonly IDictionary<string, IndexType> _indexValue = new Dictionary<string, IndexType>()
        {
            {"PRIMARY KEY", IndexType.PrimaryKey },
            {"FOREIGN KEY", IndexType.ForeignKey },
            {"UNIQUE", IndexType.UniqueKey },
        };

        /// <summary>
        /// Parse the string value into IndexType ENUM.
        /// </summary>
        /// <param name="value">The string value of index.</param>
        /// <returns>IndexType ENUM.</returns>
        internal static IndexType Parse(string value)
        {
            return _indexValue[value];
        }
    }
}