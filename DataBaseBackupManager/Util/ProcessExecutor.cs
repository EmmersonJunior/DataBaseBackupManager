using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace DataBaseBackupManager.Util
{
    /// <summary>
    /// Hold methods to manage a process execution.
    /// </summary>
    internal static class ProcessExecutor
    {
        #region constants
        private const int SUCCESS_EXIT_CODE = 0;
        #endregion

        /// <summary>
        /// Run a script file.
        /// </summary>
        /// <param name="file">The file name with full path.</param>
        internal static void Run(string file)
        {
            ProcessStartInfo info = ProcessInfoByOS(file);

            using (Process process = Process.Start(info))
            {
                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            output.AppendLine(e.Data);
                        }
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            errorWaitHandle.Set();
                        }
                        else
                        {
                            error.AppendLine(e.Data);
                        }
                    };

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    if (!process.ExitCode.Equals(SUCCESS_EXIT_CODE))
                    {
                        throw new IOException($"Error when executing file [{file}] => {error}");
                    }
                }
            }
        }

        private static ProcessStartInfo ProcessInfoByOS(string batFilePath)
        {
            ProcessStartInfo info;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                info = new ProcessStartInfo(batFilePath);
            }
            else
            {
                info = new ProcessStartInfo("sh")
                {
                    Arguments = $"{batFilePath}"
                };
            }

            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;

            return info;
        }
    }
}
