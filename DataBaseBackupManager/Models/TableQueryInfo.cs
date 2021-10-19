namespace DataBaseBackupManager.Models
{
    /// <summary>
    /// The table page offset mapping the pages for dumping.
    /// </summary>
    internal class TableQueryInfo
    {
        /// <summary>
        /// The number of lines of table.
        /// </summary>
        internal long Count { get; set; }

        /// <summary>
        /// The amount of registers to be fetched by once.
        /// </summary>
        internal long QueryAmount { get; set; }

        /// <summary>
        /// The max size of page.
        /// </summary>
        internal long Offset { get; set; }
    }
}
