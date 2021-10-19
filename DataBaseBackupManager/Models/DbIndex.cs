namespace DataBaseBackupManager.Models
{
    /// <summary>
    /// Hold information about the index.
    /// </summary>
    internal class DbIndex
    {
        /// <summary>
        /// The table name which the index belongs to.
        /// </summary>
        internal string TableName { get; set; }

        /// <summary>
        /// The index name.
        /// </summary>
        internal string IndexName { get; set; }

        /// <summary>
        /// The query for index deletion.
        /// </summary>
        internal string IndexDeletionQuery { get; set; }

        /// <summary>
        /// The query for index creation.
        /// </summary>
        internal string IndexCreationQuery { get; set; }

        /// <summary>
        /// The string representation of this instance.
        /// </summary>
        /// <returns>The instance name.</returns>
        public override string ToString()
        {
            return $"{TableName} - {IndexName}";
        }
    }
}
