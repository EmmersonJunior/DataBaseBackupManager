using DataBaseBackupManager.Abstractions;
using DataBaseBackupManager.Interfaces;
using DataBaseBackupManager.Models;
using DataBaseBackupManager.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace DataBaseBackupManager
{
    /// <summary>
    /// Manages the backup/restore flow.
    /// </summary>
    public class DataBaseBackupProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILoggerFactory _loggerFactory;
        private IBackupConfiguration _backupConfiguration;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="backupDbCredentials">The connections for dump DataBase.</param>
        /// <param name="restoreDbCredentials">The connection for restore DataBase.</param>
        public DataBaseBackupProvider(BackupDbCredentials backupDbCredentials, RestoreDbCredentials restoreDbCredentials, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _serviceProvider = RegisterServices(backupDbCredentials, restoreDbCredentials);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="backupDbCredentials">The connections for dump DataBase.</param>
        /// <param name="restoreDbCredentials">The connection for restore DataBase.</param>
        /// <param name="dataBaseType">The DataBase type.</param>
        /// <param name="backupStorageStrategy">The strategy for backup storage.</param>
        public DataBaseBackupProvider(BackupDbCredentials backupDbCredentials, RestoreDbCredentials restoreDbCredentials, DataBaseType dataBaseType, BackupStorageStrategy backupStorageStrategy, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _serviceProvider = RegisterServices(backupDbCredentials, restoreDbCredentials, dataBaseType, backupStorageStrategy);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="backupDbCredentials">The connections for dump DataBase.</param>
        /// <param name="restoreDbCredentials">The connection for restore DataBase.</param>
        /// <param name="backupConfiguration">The configuration for backup/restore.</param>
        public DataBaseBackupProvider(BackupDbCredentials backupDbCredentials, RestoreDbCredentials restoreDbCredentials, IBackupConfiguration backupConfiguration, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _backupConfiguration = backupConfiguration;
            _serviceProvider = RegisterServices(backupDbCredentials, restoreDbCredentials);
        }

        /// <summary>
        /// Build the SQL Dump Manager.
        /// </summary>
        /// <returns>An instance of SqlDumpManager.</returns>
        public SqlBackupManagerBase BuildDumpManager()
        {
            SqlBackupManagerBase sqlDumpManager = _serviceProvider.GetService<SqlBackupManagerBase>();

            return sqlDumpManager;
        }

        /// <summary>
        /// Builds the SQL Restore Manager.
        /// </summary>
        /// <returns>An instance of SqlRestoreManager.</returns>
        public SqlRestoreManagerBase BuildRestoreManager()
        {
            SqlRestoreManagerBase sqlRestoreManager = _serviceProvider.GetService<SqlRestoreManagerBase>();

            return sqlRestoreManager;
        }
        private IServiceProvider RegisterServices(BackupDbCredentials backupDbCredentials, RestoreDbCredentials restoreDbCredentials)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureCredentials(ref serviceCollection, backupDbCredentials, restoreDbCredentials);

            if (_backupConfiguration == null)
            {
                IBackupConfiguration backupConfiguration = GetConfig();
                ConfigureDataBaseServices(ref serviceCollection, backupConfiguration.DataBaseType);
                ConfigureBackupStorageServices(ref serviceCollection, backupConfiguration.BackupStorageStrategy);
            }
            else
            {
                ConfigureDataBaseServices(ref serviceCollection, _backupConfiguration.DataBaseType);
                ConfigureBackupStorageServices(ref serviceCollection, _backupConfiguration.BackupStorageStrategy);
                ConfigureBackupConfiguration(ref serviceCollection);
            }
            ConfigureLog(ref serviceCollection);
            IServiceProvider provider = serviceCollection.BuildServiceProvider();

            return provider;
        }

        private IServiceProvider RegisterServices(BackupDbCredentials backupDbCredentials, RestoreDbCredentials restoreDbCredentials, DataBaseType dataBaseType, BackupStorageStrategy backupStorageStrategy)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureCredentials(ref serviceCollection, backupDbCredentials, restoreDbCredentials);

            if (_backupConfiguration == null)
            {
                IBackupConfiguration backupConfiguration = GetConfig(dataBaseType, backupStorageStrategy);
                ConfigureDataBaseServices(ref serviceCollection, backupConfiguration.DataBaseType);
                ConfigureBackupStorageServices(ref serviceCollection, backupConfiguration.BackupStorageStrategy);
                ConfigureBackupConfiguration(ref serviceCollection);
            }
            ConfigureDataBaseServices(ref serviceCollection, dataBaseType);
            ConfigureBackupStorageServices(ref serviceCollection, backupStorageStrategy);
            ConfigureLog(ref serviceCollection);
            IServiceProvider provider = serviceCollection.BuildServiceProvider();

            return provider;
        }

        private static void ConfigureCredentials(ref IServiceCollection serviceCollection, BackupDbCredentials backupDbCredentials, RestoreDbCredentials restoreDbCredentials)
        {
            serviceCollection
                .AddSingleton(backupDbCredentials)
                .AddSingleton(restoreDbCredentials);
        }

        private static void ConfigureDataBaseServices(ref IServiceCollection serviceCollection, DataBaseType dataBaseType)
        {
            switch (dataBaseType)
            {
                case DataBaseType.Postgesql:
                    serviceCollection
                        .AddScoped<IConnectionFactory, PostgresqlConnectionFactory>()
                        .AddScoped<IDataBaseDao, PostgreSqlDataBaseDao>()
                        .AddScoped<SqlBackupManagerBase, PostgreSqlBackupManager>()
                        .AddScoped<SqlRestoreManagerBase, PostgreSqlRestoreManager>();
                    break;
                default:
                    throw new NotImplementedException("DataBase type not implemented.");
            }
        }

        private static void ConfigureBackupStorageServices(ref IServiceCollection serviceCollection, BackupStorageStrategy backupStorageStrategy)
        {
            switch (backupStorageStrategy)
            {
                case BackupStorageStrategy.LocalStorage:
                    serviceCollection
                        .AddScoped<IWriterStrategyFactory, LocalFileWriterFactory>()
                        .AddScoped<IReaderStrategyFactory, LocalFileReaderFactory>();

                    break;
                default:
                    throw new NotImplementedException("BackupStorageStrategy was not implemented.");
            }
        }

        private void ConfigureLog(ref IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(_loggerFactory);
        }

        private void ConfigureBackupConfiguration(ref IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(_backupConfiguration);
        }

        private IBackupConfiguration GetConfig(DataBaseType dataBaseType = DataBaseType.Postgesql, BackupStorageStrategy backupStorageStrategy = BackupStorageStrategy.LocalStorage)
        {
            _backupConfiguration = new BackupConfiguration(dataBaseType, backupStorageStrategy);

            return _backupConfiguration;
        }
    }
}
