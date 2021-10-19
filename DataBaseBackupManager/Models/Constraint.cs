namespace DataBaseBackupManager.Models
{
    /// <summary>
    /// Represents a dataBase constraint.
    /// </summary>
    internal class Constraint
    {
        /// <summary>
        /// The constraint name.
        /// </summary>
        internal string ConstraintName { get; set; }

        /// <summary>
        /// The column name.
        /// </summary>
        internal string ColumnName { get; set; }

        /// <summary>
        /// The table owner of the constraint.
        /// </summary>
        internal string TableReference { get; set; }

        /// <summary>
        /// The column owner of the key at the owner table.
        /// </summary>
        internal string ColumnReference { get; set; }

        /// <summary>
        /// The data type of content.
        /// </summary>
        internal ConstraintType DataType { get; set; }
    }
}
