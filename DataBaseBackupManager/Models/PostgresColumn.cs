using DataBaseBackupManager.Abstractions;
using System.Text;

namespace DataBaseBackupManager.Models
{
    /// <summary>
    /// Maps the database column properties
    /// </summary>
    internal class PostgresColumn : Column
    {
        /// <summary>
        /// Test if type needs quotation.
        /// </summary>
        /// <returns>True if needs quotation.</returns>
        internal bool IsText()
        {
            if (Type.Equals("varchar") || Type.Equals("text") || Type.Equals("character varying") || Type.StartsWith("timestamp"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Test if type needs braces.
        /// </summary>
        /// <returns>True if needs quotation.</returns>
        internal bool IsJson()
        {
            if (Type.Equals("json"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Used to format a column value into query.
        /// </summary>
        /// <param name="stringBuilder">Contains all columns values.</param>
        /// <param name="value">The actual column value.</param>
        internal void AppendColumnValue(ref StringBuilder stringBuilder, string value)
        {
            string separator = stringBuilder.Length > 0 ? ", " : string.Empty;
            string body;

            if (string.IsNullOrWhiteSpace(value) || value.ToUpper().Equals("NULL"))
            {
                body = IsNullable ? "NULL" : "''";
            }
            else if (IsText())
            {
                body = $"'{value}'".Replace("\\n", "\n").Replace("\\r", "\r");
            }
            else if (IsJson())
            {
                body = value.StartsWith("'{") ? value : "'{}'";
            }
            else
            {
                body = value;
            }

            stringBuilder.Append($"{separator}{body}");
        }
    }
}
