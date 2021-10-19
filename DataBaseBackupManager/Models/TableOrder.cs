using System.Collections.Generic;
using System.Linq;

namespace DataBaseBackupManager.Models
{
    /// <summary>
    /// Used to order the sequence of tables to restore.
    /// </summary>
    internal static class TableOrder
    {
        /// <summary>
        /// Populates the table priority according its constraints.
        /// </summary>
        /// <param name="tables">The tables to be checked.</param>
        internal static IList<Table> SetTablePriority(this IList<Table> tables)
        {
            foreach (Table table in tables)
            {
                IList<Table> dependentTables = tables.Where(t => table.Constraints.Select(cons => cons.TableReference).Contains(t.Name)).ToList();

                foreach (Table dependentTable in dependentTables)
                {
                    if (dependentTable.Priority <= table.Priority)
                    {
                        dependentTable.Priority = table.Priority + 1;
                    }
                }
            }
            return tables.OrderByDescending(table => table.Priority).ToList();
        }
    }
}