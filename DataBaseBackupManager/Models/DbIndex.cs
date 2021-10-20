using System.Collections.Generic;

namespace DataBaseBackupManager.Models
{
    /// <summary>
    /// Represents a dataBase constraint.
    /// </summary>
    internal class DbIndex
    {
        /// <summary>
        /// The constraint name.
        /// </summary>
        internal string IndexName { get; set; }

        /// <summary>
        /// The column name.
        /// </summary>
        internal IList<string> Columns { get; set; }

        /// <summary>
        /// The column owner of the key at the owner table.
        /// </summary>
        internal string ColumnReference { get; set; }

        /// <summary>
        /// The table of the constraint.
        /// </summary>
        internal string Table { get; set; }

        /// <summary>
        /// The table owner of the constraint.
        /// </summary>
        internal string TableReference { get; set; }

        /// <summary>
        /// The query for constraint deletion.
        /// </summary>
        internal string IndexDeletionQuery { get; set; }

        /// <summary>
        /// The query for constraint creation.
        /// </summary>
        internal string IndexCreationQuery { get; set; }

        /// <summary>
        /// The data type of constraint.
        /// </summary>
        internal IndexType IndexType { get; set; }

        /// <summary>
        /// The recreation index priority.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// The string representation of this instance.
        /// </summary>
        /// <returns>The instance name.</returns>
        public override string ToString()
        {
            return $"{TableReference} - {IndexName}";
        }
    }
}
