namespace DataBaseBackupManager.Models
{
    /// <summary>
    /// Maps the constraint types.
    /// </summary>
    internal enum IndexType
    {
        /// <summary>
        /// The table foreign key.
        /// </summary>
        ForeignKey = 1,

        /// <summary>
        /// The primary key of the table.
        /// </summary>
        PrimaryKey = 2,

        /// <summary>
        /// A unique key for column.
        /// </summary>
        UniqueKey = 3
    }
}
