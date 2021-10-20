namespace DataBaseBackupManager.Models
{
    /// <summary>
    /// Hold information about the queries of dump and restore.
    /// </summary>
    internal static class PostgreSqlQueryConstants
    {
        internal const string SELECT_TABLE_NAME = "SELECT table_name FROM information_schema.\"tables\" where table_schema = \'{0}\'";
        internal const string TABLE_NAME = "table_name";
        internal const string COLUMN_NAME = "column_name";
        internal const string TABLE_REFERENCE = "foreign_table_name";
        internal const string COLUMN_REFERENCE = "foreign_column_name";
        internal const string CONSTRAINT_NAME = "constraint_name";
        internal const string CONSTRAINT_TYPE = "constraint_type";
        internal const string COUNT_COLUMN = "count";
        internal const string INDEXNAME = "indexname";
        internal const string TABLENAME = "tablename";
        internal const string INDEXDEF = "indexdef";
        internal const string COLUMNS_ROW_COUNT = "select COUNT(1) from {0}";
        internal const string TABLE_PAGING_SELECT = "select {0} from {1} limit {2} offset {3}";
        internal const string INSERT_REGISTER = "INSERT INTO {0}({1}) VALUES {2}";
        internal const string CREATE_DATABASE = "CREATE DATABASE {0}";
        internal const string DROP_DATABASE = "DROP DATABASE IF EXISTS {0}";
        internal const string DROP_TABLE = "DROP TABLE IF EXISTS {0}";
        internal const string SELECT_TABLE_INFORMATION = "SELECT column_name, data_type, is_nullable FROM information_schema.\"columns\" where table_name = \'{0}\'";
        internal const string DATA_TYPE = "data_type";
        internal const string IS_NULLABLE = "is_nullable";
        internal const string INSERT_DATA = "INSERT INTO {0} {1} VALUES {2}";
        //internal const string GET_CONSTRAINT_INDEX = "SELECT constraint_name FROM information_schema.constraint_table_usage WHERE table_schema = '{0}'";
        internal const string GET_CONSTRAINT_INDEX = "SELECT " +
                                                    "tc.table_schema, " +
                                                    "tc.constraint_name, " +
                                                    "tc.constraint_type, " +
                                                    "tc.table_name, " +
                                                    "kcu.column_name, " +
                                                    "ccu.table_schema AS foreign_table_schema, " +
                                                    "ccu.table_name AS foreign_table_name, " +
                                                    "ccu.column_name AS foreign_column_name " +
                                                    "FROM " +
                                                    "information_schema.table_constraints AS tc " +
                                                    "JOIN information_schema.key_column_usage AS kcu " +
                                                       "ON tc.constraint_name = kcu.constraint_name " +
                                                        "AND tc.table_schema = kcu.table_schema " +
                                                    "JOIN information_schema.constraint_column_usage AS ccu " +
                                                        "ON ccu.constraint_name = tc.constraint_name " +
                                                        "AND ccu.table_schema = tc.table_schema " +
                                                    "WHERE tc.table_schema = '{0}'";
        internal const string GET_ALL_INDEXES = "SELECT indexname, tablename, indexdef FROM pg_indexes WHERE schemaname = '{0}'";
        internal const string SELECT_SCHEMA_CONSTRAINTS = "SELECT * FROM ( SELECT " +
                                                        "pgc.contype as constraint_type, " +
                                                        "pgc.conname as constraint_name, " +
                                                        "ccu.table_schema AS table_schema, " +
                                                        "kcu.table_name as table_name, " +
                                                        "CASE WHEN(pgc.contype = 'f') THEN kcu.COLUMN_NAME ELSE ccu.COLUMN_NAME END as column_name, " +
                                                        "CASE WHEN (pgc.contype = 'f') THEN ccu.TABLE_NAME ELSE (null) END as reference_table, " +
                                                        "CASE WHEN(pgc.contype = 'f') THEN ccu.COLUMN_NAME ELSE (null) END as reference_col, " +
                                                        "CASE WHEN(pgc.contype = 'p') THEN 'yes' ELSE 'no' END as auto_inc, " +
                                                        "CASE WHEN(pgc.contype = 'p') THEN 'NO' ELSE 'YES' END as is_nullable, " +
                                                                                                                "'integer' as data_type, " +
                                                                                                                "'0' as numeric_scale, " +
                                                                                                                "'32' as numeric_precision " +
                                                    "FROM " +
                                                        "pg_constraint AS pgc " +
                                                        "JOIN pg_namespace nsp ON nsp.oid = pgc.connamespace " +
                                                        "JOIN pg_class cls ON pgc.conrelid = cls.oid " +
                                                        "JOIN information_schema.key_column_usage kcu ON kcu.constraint_name = pgc.conname " +
                                                        "LEFT JOIN information_schema.constraint_column_usage ccu ON pgc.conname = ccu.CONSTRAINT_NAME " +
                                                        "AND nsp.nspname = ccu.CONSTRAINT_SCHEMA " +
                                                        "UNION " +
                                                        "SELECT  null as constraint_type , null as constraint_name , '{0}' as \"table_schema\" , " +
                                                        "table_name , column_name, null as refrence_table , null as refrence_col , 'no' as auto_inc , " +
                                                        "is_nullable , data_type, numeric_scale , numeric_precision " +
                                                        "FROM information_schema.columns cols " +
                                                        "Where 1=1 " +
                                                        "AND table_schema = '{0}' " +
                                                        "and column_name not in ( " +
                                                            "SELECT CASE WHEN (pgc.contype = 'f') THEN kcu.COLUMN_NAME ELSE kcu.COLUMN_NAME END " +
                                                            "FROM " +
                                                            "pg_constraint AS pgc " +
                                                            "JOIN pg_namespace nsp ON nsp.oid = pgc.connamespace " +
                                                            "JOIN pg_class cls ON pgc.conrelid = cls.oid " +
                                                            "JOIN information_schema.key_column_usage kcu ON kcu.constraint_name = pgc.conname " +
                                                            "LEFT JOIN information_schema.constraint_column_usage ccu ON pgc.conname = ccu.CONSTRAINT_NAME " +
                                                            "AND nsp.nspname = ccu.CONSTRAINT_SCHEMA " +
                                                        ") " +
                                                    ") as foo " +
                                                    "where constraint_type is not null and constraint_type = 'f' and table_name = '{1}' " +
                                                    "ORDER BY table_name desc ";

    }
}
