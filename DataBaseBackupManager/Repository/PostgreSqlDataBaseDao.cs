using DataBaseBackupManager.Abstractions;
using DataBaseBackupManager.Interfaces;
using DataBaseBackupManager.Models;
using DataBaseBackupManager.Util;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataBaseBackupManager.Repository
{
    /// <summary>
    /// Methods to access DB information for dump and restoring.
    /// </summary>
    public class PostgreSqlDataBaseDao : IDataBaseDao
    {
        #region constants
        private const string NULLABLE_VALUE = "YES";
        private const int UNIQUE_CONSTRAINT_PRIORITY = 0;
        private const int PRIMARY_KEY_CONSTRAINT_PRIORITY = 1;
        private const int FOREIGN_KEY_CONSTRAINT_PRIORITY = 2;
        private const int PLAIN_INDEX_PRIORITY = 3;
        #endregion

        private readonly ILogger<PostgreSqlDataBaseDao> _logger;
        private readonly IBackupConfiguration _backupConfiguration;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The logger factory.</param>
        /// <param name="backupConfiguration">The user backup configuration.</param>
        public PostgreSqlDataBaseDao(ILoggerFactory logger, IBackupConfiguration backupConfiguration)
        {
            _logger = logger.CreateLogger<PostgreSqlDataBaseDao>();
            _backupConfiguration = backupConfiguration;
        }

        /// <summary>
        /// Drops the database.
        /// </summary>
        /// <param name="credentials">Maps the DataBase credentials for connection establishing.</param>
        internal static void DropDataBase(IDbCredentials credentials)
        {
            PostgreSqlConnection postgreSqlConnection = new PostgreSqlConnection(0, credentials, spareDataBase: true);
            postgreSqlConnection.Connect();
            NpgsqlCommand dropDataBaseCommand = new NpgsqlCommand(string.Format(PostgreSqlQueryConstants.DROP_DATABASE, credentials.DataBase), postgreSqlConnection.Connection);
            dropDataBaseCommand.ExecuteNonQuery();
            postgreSqlConnection.Close();
        }

        /// <summary>
        /// Drops the table.
        /// </summary>
        /// <param name="table">Hold table informations.</param>
        /// <param name="connection">The connection of database.</param>
        void IDataBaseDao.DropTable(Table table, Connection connection)
        {
            PostgreSqlConnection postgreSqlConnection = (PostgreSqlConnection)connection;
            NpgsqlCommand dropDataBaseCommand = new NpgsqlCommand(string.Format(PostgreSqlQueryConstants.DROP_TABLE, table.Name), postgreSqlConnection.Connection);
            dropDataBaseCommand.ExecuteNonQuery();
            postgreSqlConnection.Close();
        }

        /// <summary>
        /// Creates the database.
        /// </summary>
        /// <param name="credentials">Maps the DataBase credentials for connection establishing.</param>
        internal static void CreateDataBase(IDbCredentials credentials)
        {
            PostgreSqlConnection postgreSqlConnection = new PostgreSqlConnection(0, credentials, spareDataBase: true);
            postgreSqlConnection.Connect();
            NpgsqlCommand createDataBaseCommand = new NpgsqlCommand(string.Format(PostgreSqlQueryConstants.CREATE_DATABASE, credentials.DataBase), postgreSqlConnection.Connection);
            createDataBaseCommand.ExecuteNonQuery();
            postgreSqlConnection.Close();
        }

        /// <summary>
        /// Get all table names from a schema.
        /// </summary>
        /// <param name="schema">The schema to be dumped.</param>
        /// <param name="connection">The connection of database.</param>
        /// <returns>A list containing all table names from a schema.</returns>
        IList<string> IDataBaseDao.GetTables(string schema, Connection connection)
        {
            PostgreSqlConnection postgreSqlConnection = (PostgreSqlConnection)connection;
            NpgsqlCommand getTablesCommand = new NpgsqlCommand(string.Format(PostgreSqlQueryConstants.SELECT_TABLE_NAME, schema), postgreSqlConnection.Connection);
            NpgsqlDataReader getTablesreader = getTablesCommand.ExecuteReader();
            IList<string> tables = new List<string>();

            while (getTablesreader.Read())
            {
                tables.Add(getTablesreader[PostgreSqlQueryConstants.TABLE_NAME].ToString());
            }
            getTablesreader.Close();

            return tables;
        }

        /// <summary>
        /// Get the number of registers of a table.
        /// </summary>
        /// <param name="table">The table name.</param>
        /// <param name="connection">The connection of database.</param>
        /// <returns>The count of a table registers.</returns>
        long IDataBaseDao.GetColumnsRowCount(string table, Connection connection)
        {
            PostgreSqlConnection postgreSqlConnection = (PostgreSqlConnection)connection;
            NpgsqlCommand countRowsCommand = new NpgsqlCommand(string.Format(PostgreSqlQueryConstants.COLUMNS_ROW_COUNT, table), postgreSqlConnection.Connection);
            NpgsqlDataReader countRowsReader = countRowsCommand.ExecuteReader();
            countRowsReader.Read();
            long count = (long)countRowsReader[PostgreSqlQueryConstants.COUNT_COLUMN];
            countRowsReader.Close();

            return count;
        }

        /// <summary>
        /// Get the indexes informations from DB.
        /// </summary>
        /// <param name="schema">The schema name.</param>
        /// <param name="connection">The database connection.</param>
        /// <returns>A list of all indexes in DataBase.</returns>
        IList<DbIndex> IDataBaseDao.GetIndexes(string schema, Connection connection)
        {
            return GetIndexes(schema, connection);
        }

        private static IList<DbIndex> GetIndexes(string schema, Connection connection)
        {
            PostgreSqlConnection postgreSqlConnection = (PostgreSqlConnection)connection;

            NpgsqlCommand getConstraintIndexesCommand = new NpgsqlCommand(string.Format(PostgreSqlQueryConstants.GET_CONSTRAINT_INDEX, schema), postgreSqlConnection.Connection);
            NpgsqlDataReader constraintIndexesReader = getConstraintIndexesCommand.ExecuteReader();
            IList<DbIndex> indexes = new List<DbIndex>();

            while (constraintIndexesReader.Read())
            {
                string indexName = constraintIndexesReader[PostgreSqlQueryConstants.CONSTRAINT_NAME].ToString();

                if (!indexes.Any(i => i.IndexName.Equals(indexName)))
                {
                    DbIndex index = new DbIndex()
                    {
                        IndexName = indexName,
                        Table = constraintIndexesReader[PostgreSqlQueryConstants.TABLE_NAME].ToString(),
                        Columns = new List<string>() { constraintIndexesReader[PostgreSqlQueryConstants.COLUMN_NAME].ToString() },
                        IndexType = IndexTypeParser.Parse(constraintIndexesReader[PostgreSqlQueryConstants.CONSTRAINT_TYPE].ToString()),
                        ColumnReference = constraintIndexesReader[PostgreSqlQueryConstants.COLUMN_REFERENCE].ToString(),
                        TableReference = constraintIndexesReader[PostgreSqlQueryConstants.TABLE_REFERENCE].ToString()
                    };
                    indexes.Add(index);
                }
                else
                {
                    DbIndex index = indexes.First(i => i.IndexName.Equals(indexName));

                    if (index.IndexType.Equals(IndexType.UniqueKey))
                    {
                        index.Columns.Add(constraintIndexesReader[PostgreSqlQueryConstants.COLUMN_NAME].ToString());
                    }
                }
            }
            constraintIndexesReader.Close();
            NpgsqlCommand getAllIndexesCommand = new NpgsqlCommand(string.Format(PostgreSqlQueryConstants.GET_ALL_INDEXES, schema), postgreSqlConnection.Connection);
            NpgsqlDataReader allIndexesReader = getAllIndexesCommand.ExecuteReader();

            while (allIndexesReader.Read())
            {
                string indexName = allIndexesReader[PostgreSqlQueryConstants.INDEXNAME].ToString();
                DbIndex index = indexes.FirstOrDefault(i => i.IndexName.Equals(indexName));

                if (index == null)
                {
                    indexes.Add(new DbIndex()
                    {
                        IndexName = indexName,
                        Table = allIndexesReader[PostgreSqlQueryConstants.TABLENAME].ToString(),
                        TableReference = allIndexesReader[PostgreSqlQueryConstants.TABLENAME].ToString(),
                        IndexCreationQuery = allIndexesReader[PostgreSqlQueryConstants.INDEXDEF].ToString(),
                        IndexDeletionQuery = $"DROP INDEX {indexName} CASCADE",
                        Priority = PLAIN_INDEX_PRIORITY
                    });
                }
            }
            allIndexesReader.Close();

            foreach (DbIndex index in indexes)
            {
                switch (index.IndexType)
                {
                    case IndexType.ForeignKey:
                        index.IndexCreationQuery = $"ALTER TABLE ONLY {schema}.{index.Table} ADD CONSTRAINT {index.IndexName} FOREIGN KEY ({index.Columns.First()}) REFERENCES {schema}.{index.TableReference}({index.ColumnReference});";
                        index.IndexDeletionQuery = $"ALTER TABLE {schema}.{index.Table} DROP CONSTRAINT {index.IndexName} CASCADE;";
                        index.Priority = FOREIGN_KEY_CONSTRAINT_PRIORITY;
                        break;

                    case IndexType.PrimaryKey:
                        index.IndexCreationQuery = $"ALTER TABLE ONLY {schema}.{index.Table} ADD CONSTRAINT {index.IndexName} PRIMARY KEY ({index.Columns.First()});";
                        index.IndexDeletionQuery = $"ALTER TABLE {schema}.{index.Table} DROP CONSTRAINT {index.IndexName} CASCADE;";
                        index.Priority = PRIMARY_KEY_CONSTRAINT_PRIORITY;
                        break;

                    case IndexType.UniqueKey:
                        index.IndexCreationQuery = $"ALTER TABLE ONLY {schema}.{index.Table} ADD CONSTRAINT {index.IndexName} UNIQUE ({string.Join(", ", index.Columns.Distinct())});";
                        index.IndexDeletionQuery = $"ALTER TABLE {schema}.{index.Table} DROP CONSTRAINT {index.IndexName} CASCADE;";
                        index.Priority = UNIQUE_CONSTRAINT_PRIORITY;
                        break;

                    default:
                        break;
                }
            }
            indexes = indexes.OrderBy(i => i.Priority).ToList();

            return indexes;
        }

        /// <summary>
        /// Drop the indexes in order to optimize the data insertion.
        /// </summary>
        /// <param name="indexes">The indexes that are going to be dropped.</param>
        /// <param name="connection">The DataBase connection.</param>
        void IDataBaseDao.DropIndexes(IList<DbIndex> indexes, Connection connection)
        {
            PostgreSqlConnection postgreSqlConnection = (PostgreSqlConnection)connection;

            foreach (DbIndex index in indexes)
            {
                try
                {
                    NpgsqlCommand dropIndexCommand = new NpgsqlCommand(index.IndexDeletionQuery, postgreSqlConnection.Connection);
                    dropIndexCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Recreate the indexes after data insertion.
        /// </summary>
        /// <param name="indexes">The indexes that are going to be recreated.</param>
        /// <param name="connection">The DataBase connection.</param>
        void IDataBaseDao.RecreateIndexes(IList<DbIndex> indexes, Connection connection)
        {
            PostgreSqlConnection postgreSqlConnection = (PostgreSqlConnection)connection;

            foreach (DbIndex index in indexes)
            {
                try
                {
                    NpgsqlCommand dropIndexCommand = new NpgsqlCommand(index.IndexCreationQuery, postgreSqlConnection.Connection);
                    dropIndexCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Get data from a table bit by bit.
        /// </summary>
        /// <param name="table">The table name.</param>
        /// <param name="columns">The columns to be fetched.</param>
        /// <param name="offset">The amount of registers to be retrieved once.</param>
        /// <param name="increment">The number of page.</param>
        /// <param name="connection">The connection of database.</param>
        /// <returns>The data separated by pipe '|'.</returns>
        string IDataBaseDao.GetDataPagingSelect(Column[] columns, string table, long offset, long increment, Connection connection)
        {
            PostgreSqlConnection postgreSqlConnection = (PostgreSqlConnection)connection;
            string columnHeader = string.Join(',', columns.Select(col => $"\"{col.Name}\""));
            NpgsqlCommand getRegistersPagingCommand = new NpgsqlCommand(string.Format(PostgreSqlQueryConstants.TABLE_PAGING_SELECT, columnHeader, table, offset, (offset * increment)), postgreSqlConnection.Connection);
            NpgsqlDataReader getRegistersPagingReader = getRegistersPagingCommand.ExecuteReader();
            IList<string> queryInsertion = new List<string>();

            while (getRegistersPagingReader.Read())
            {
                IList<string> rowInsertion = new List<string>();

                for (int j = 0; j < getRegistersPagingReader.FieldCount; j++)
                {
                    string value = getRegistersPagingReader.GetFieldValue<object>(j).ToString()
                        .Replace("\n", "\\n")
                        .Replace("\r", "\\r");
                    rowInsertion.Add(string.IsNullOrWhiteSpace(value) ? "NULL" : value);
                }
                queryInsertion.Add($"{string.Join('|', rowInsertion)}");
            }
            string line = string.Join('\n', queryInsertion);
            getRegistersPagingReader.Close();

            return line;
        }

        /// <summary>
        /// Get table information to assembly the insertion statement.
        /// </summary>
        /// <param name="schema">The schema of database.</param>
        /// <param name="tables">The tables to retrieve data.</param>
        /// <param name="connection">The active connection at the database.</param>
        /// <returns>Table data.</returns>
        IList<Table> IDataBaseDao.GetTableInformations(string schema, IList<string> tables, Connection connection)
        {
            IList<Table> tablesData = new List<Table>();
            IList<DbIndex> indexes = GetIndexes(schema, connection);

            foreach (string tableName in tables)
            {
                PostgreSqlConnection postgreSqlConnection = (PostgreSqlConnection)connection;
                NpgsqlCommand selectTableInfoCommand = new NpgsqlCommand(string.Format(PostgreSqlQueryConstants.SELECT_TABLE_INFORMATION, tableName), postgreSqlConnection.Connection);
                NpgsqlDataReader selectTableInfoReader = selectTableInfoCommand.ExecuteReader();
                IList<Column> columns = new List<Column>();
                int columnNumber = 0;
                IList<string> excludedColumns = GetExcludedColumns(tableName);

                while (selectTableInfoReader.Read())
                {
                    string columnName = selectTableInfoReader[PostgreSqlQueryConstants.COLUMN_NAME].ToString();
                    Column column = new PostgresColumn()
                    {
                        Name = columnName,
                        Type = selectTableInfoReader[PostgreSqlQueryConstants.DATA_TYPE].ToString(),
                        IsNullable = selectTableInfoReader[PostgreSqlQueryConstants.IS_NULLABLE].ToString() == NULLABLE_VALUE,
                        ColumnNumber = columnNumber,
                        Excluded = excludedColumns?.Contains(columnName) ?? false
                    };
                    columns.Add(column);
                    columnNumber++;

                    if (column.Excluded)
                    {
                        _logger.LogWarning($"Column {columnName} from table {tableName} was flagged as excluded from backup/restore.");
                    }
                }
                selectTableInfoReader.Close();
                Table table = new Table()
                {
                    Name = tableName,
                    Columns = columns.ToArray(),
                    Constraints = indexes.Where(i => i.Table.Equals(tableName)).ToArray(),
                    Excluded = GetExcludedTable(tableName)
                };
                tablesData.Add(table);

                if (table.Excluded)
                {
                    _logger.LogWarning($"Table {table.Name} was flagged as excluded from backup/restore.");
                }
            }
            return tablesData;
        }

        private IList<string> GetExcludedColumns(string table)
        {
            _backupConfiguration.TableColumnsToExclude.TryGetValue(table, out string[] excludedColumns);

            return excludedColumns;
        }

        private bool GetExcludedTable(string table)
        {
            return _backupConfiguration.TablesToExclude.Contains(table);
        }

        /// <summary>
        /// Insert data into the table.
        /// </summary>
        /// <param name="table">The table to insert data.</param>
        /// <param name="header">The columns that compose the query header.</param>
        /// <param name="values">The value of columns for bulk insertion.</param>
        /// <param name="connection">The database connection.</param>
        void IDataBaseDao.InsertData(string table, string header, string values, Connection connection)
        {
            PostgreSqlConnection postgreSqlConnection = (PostgreSqlConnection)connection;
            NpgsqlCommand countRowsCommand = new NpgsqlCommand(string.Format(PostgreSqlQueryConstants.INSERT_DATA, table, header, values), postgreSqlConnection.Connection);
            countRowsCommand.ExecuteNonQuery();
        }
    }
}
