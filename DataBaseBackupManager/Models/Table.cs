using DataBaseBackupManager.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace DataBaseBackupManager.Models
{
    /// <summary>
    /// Maps the database table properties
    /// </summary>
    internal class Table
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal Table()
        {
            Priority = 0;
        }

        /// <summary>
        /// Table name.
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// The table columns.
        /// </summary>
        internal Column[] Columns { get; set; }

        /// <summary>
        /// The table constraints.
        /// </summary>
        internal DbIndex[] Constraints { get; set; }

        /// <summary>
        /// The priority for restoring.
        /// </summary>
        internal int Priority { get; set; }

        /// <summary>
        /// The value of lines to be inserted on bulk insertion.
        /// </summary>
        internal int PageInsertOffset { get; set; }

        /// <summary>
        /// Indicates if the tables is excluded or not from a backup/restore process.
        /// </summary>
        internal bool Excluded { get; set; }

        /// <summary>
        /// Set the column order according the list of names.
        /// </summary>
        /// <param name="columnNames">The list of names ordered.</param>
        /// <returns></returns>
        internal void SetColumnNumberBy(IList<string> columnNames)
        {
            int columnNumber = 0;

            foreach (string columnName in columnNames)
            {
                Column column = Columns.FirstOrDefault(col => col.Name.Equals(columnName));

                if (column != null)
                {
                    if (!column.Excluded)
                    {
                        column.ColumnNumber = columnNumber;
                        columnNumber++;
                    }
                    else
                    {
                        column.ColumnNumber = -1;
                    }
                }
            }
        }

        /// <summary>
        /// The string representation of this instance.
        /// </summary>
        /// <returns>Name.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
