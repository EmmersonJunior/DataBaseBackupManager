namespace DataBaseBackupManager.Abstractions
{
    /// <summary>
    /// Maps the database column properties
    /// </summary>
    public abstract class Column
    {
        /// <summary>
        /// The column name.
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// The column type.
        /// </summary>
        internal string Type { get; set; }

        /// <summary>
        /// Hold if a column is NULLABLE or not.
        /// </summary>
        internal bool IsNullable { get; set; }

        /// <summary>
        /// Is a constraint.
        /// </summary>
        internal bool IsConstraint { get; set; }

        /// <summary>
        /// The column number to link the data.
        /// </summary>
        internal int ColumnNumber { get; set; }

        /// <summary>
        /// Indicates if the column is excluded or not from a backup/restore process.
        /// </summary>
        internal bool Excluded { get; set; }

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
