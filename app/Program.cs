using DataBaseBackupManager;
using DataBaseBackupManager.Abstractions;
using DataBaseBackupManager.Interfaces;
using DataBaseBackupManager.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

internal static class Program
{
    private static void Main(string[] args)
    {
        RestoreDbCredentials localRestoreCredential = new RestoreDbCredentials("localhost", 5432, "servico_documentacao_pessoa_dev", "postgres", "emmerson", "public", "servico_documentacao_pessoa_dev.backup");
        BackupDbCredentials localBackupCredential = new BackupDbCredentials("localhost", 5432, "servico_documentacao_pessoa_prod", "postgres", "emmerson", "public", "servico_documentacao_pessoa_dev.backup");
        //BackupDbCredentials remoteBackupCredential = new BackupDbCredentials("postgresql-foundation-devhml2.cjxg8cvlshhk.us-east-1.rds.amazonaws.com", 5432, "servico_documentacao_pessoa_dev", "servico_documentacao_pessoa", "10wYCpxZop7ga9xKeCAm", "servico_documentacao_pessoa_dev.backup");
        ILoggerFactory loggerFactory = LoggerFactory.Create(log => log.AddSimpleConsole(options =>
        {
            options.IncludeScopes = false;
            options.SingleLine = true;
            options.TimestampFormat = "hh:mm:ss ";
        }).SetMinimumLevel(LogLevel.Information));
        string[] tablesToExclude = new string[1] { "aluno_documento" };

        IDictionary<string, string[]> tableColumnToExclude = new Dictionary<string, string[]>()
            {
                {"documento_referencia_extracao", new string[1] { "conteudo" } }
            };

        IBackupConfiguration backupConfiguration = new BackupConfiguration(DataBaseType.Postgesql, BackupStorageStrategy.LocalStorage, tablesToExclude, tableColumnToExclude);
        DataBaseBackupProvider dataBaseBackupProvider = new DataBaseBackupProvider(localBackupCredential, localRestoreCredential, backupConfiguration, loggerFactory);

        using (SqlBackupManagerBase sqlBackupManager = dataBaseBackupProvider.BuildDumpManager())
        {
            sqlBackupManager.StartDump();
        }

        using (SqlRestoreManagerBase sqlRestoreManager = dataBaseBackupProvider.BuildRestoreManager())
        {
            sqlRestoreManager.StartRestore();
        }
    }
}
