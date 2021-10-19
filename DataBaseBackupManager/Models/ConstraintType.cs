using System;
using System.Collections.Generic;
using System.Text;

namespace DataBaseBackupManager.Models
{
    /// <summary>
    /// Maps the constraint types.
    /// </summary>
    internal enum  ConstraintType
    {
        /// <summary>
        /// The table foreign key.
        /// </summary>
        ForeignKey = 'f',

        /// <summary>
        /// The primary key of the table.
        /// </summary>
        PrimaryKey = 'p',

        /// <summary>
        /// A unique key for column.
        /// </summary>
        UniqueKey = 'u'
    }
}
