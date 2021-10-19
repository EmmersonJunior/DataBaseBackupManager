using System;
using System.IO;

namespace DataBaseBackupManager.Util
{
    /// <summary>
    /// The backup folder manager.
    /// </summary>
    internal static class BakupStorageFolderManager
    {
        private static readonly DateTime _backupCreatedAt = DateTime.Now;
        private static readonly string _backupPath = "C:\\backups";

        /// <summary>
        /// Get the backup folder based on date of backup creation.
        /// </summary>
        /// <returns>The backup folder.</returns>
        internal static string GetBackupFolder()
        {
            string folderName = $"{_backupPath}\\BACKUP_{_backupCreatedAt.Day}-{_backupCreatedAt.Month}-{_backupCreatedAt.Year}_{_backupCreatedAt.Hour}-{_backupCreatedAt.Minute}-{_backupCreatedAt.Second}";
            Directory.CreateDirectory(folderName);

            return folderName;
        }
    }
}
