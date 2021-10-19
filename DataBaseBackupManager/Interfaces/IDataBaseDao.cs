using DataBaseBackupManager.Abstractions;
using DataBaseBackupManager.Models;
using System;
using System.Collections.Generic;

namespace DataBaseBackupManager.Interfaces
{
    /// <summary>
    /// Methods to access DB information for dump and restoring.
    /// </summary>
    public interface IDataBaseDao
    {
        /// <summary>
        /// Drops the database.
        /// </summary>
        /// <param name="credentials">Maps the DataBase credentials for connection establishing.</param>
        internal static void DropDataBase(DbCredentials credentials)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Drops the table.
        /// </summary>
        /// <param name="table">Hold table informations.</param>
        /// <param name="connection">The connection of database.</param>
        internal void DropTable(Table table, Connection connection);

        /// <summary>
        /// Creates the database.
        /// </summary>
        /// <param name="credentials">Maps the DataBase credentials for connection establishing.</param>
        internal static void CreateDataBase(DbCredentials credentials)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get all table names from a schema.
        /// </summary>
        /// <param name="schema">The schema to be dumped.</param>
        /// <param name="connection">The connection of database.</param>
        /// <returns>A list containing all table names from a schema.</returns>
        internal IList<string> GetTables(string schema, Connection connection);

        /// <summary>
        /// Get the number of registers of a table.
        /// </summary>
        /// <param name="table">The table name.</param>
        /// <param name="connection">The connection of database.</param>
        /// <returns>The count of a table registers.</returns>
        internal long GetColumnsRowCount(string table, Connection connection);

        /// <summary>
        /// Get data from a table bit by bit.
        /// </summary>
        /// <param name="table">The table name.</param>
        /// <param name="columns">The columns to be fetched.</param>
        /// <param name="offset">The amount of registers to be retrieved once.</param>
        /// <param name="increment">The number of page.</param>
        /// <param name="connection">The connection of database.</param>
        /// <returns>The data separated by pipe '|'.</returns>
        internal string GetDataPagingSelect(Column[] columns, string table, long offset, long increment, Connection connection);

        /// <summary>
        /// Get table information to assembly the insertion statement.
        /// </summary>
        /// <param name="schema">The schema of database.</param>
        /// <param name="tables">The tables to retrieve data.</param>
        /// <param name="connection">The active connection at the database.</param>
        /// <returns>Table data.</returns>
        internal IList<Table> GetTableInformations(string schema, IList<string> tables, Connection connection);

        /// <summary>
        /// Insert data into the table.
        /// </summary>
        /// <param name="table">The table to insert data.</param>
        /// <param name="header">The columns that compose the query header.</param>
        /// <param name="values">The value of columns for bulk insertion.</param>
        /// <param name="connection">The database connection.</param>
        internal void InsertData(string table, string header, string values, Connection connection);

        /// <summary>
        /// Get the indexes informations from DB.
        /// </summary>
        /// <param name="schema">The schema name.</param>
        /// <param name="connection">The database connection.</param>
        /// <returns>A list of all indexes in DataBase.</returns>
        internal IList<DbIndex> GetIndexes(string schema, Connection connection);

        /// <summary>
        /// Drop the indexes in order to optimize the data insertion.
        /// </summary>
        /// <param name="indexes">The indexes that are going to be dropped.</param>
        /// <param name="connection">The DataBase connection.</param>
        internal void DropIndexes(IList<DbIndex> indexes, Connection connection);

        /// <summary>
        /// Recreate the indexes after data insertion.
        /// </summary>
        /// <param name="indexes">The indexes that are going to be recreated.</param>
        /// <param name="connection">The DataBase connection.</param>
        internal void RecreateIndexes(IList<DbIndex> indexes, Connection connection);
    }
}
