﻿// File: Logs/Logger.cs
// Date: 2024/08/28
// Description: Loggers
//
// Copyright (C) 2024 detached64
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using GalArc.Properties;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GalArc.Logs
{
    public enum LogLevel
    {
        Error,
        Info,
        Debug
    }

    public static class Logger
    {
        public static event EventHandler<string> Process;
        public static event EventHandler<string> ErrorOccured;
        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private static void SaveLog(string log)
        {
            File.AppendAllText("log.txt", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]" + log);
        }

        private static void WriteAndSaveLog(string log, int logLevel = 1)
        {
            LogLevel level = (LogLevel)logLevel;
            if ((level == LogLevel.Debug && BaseSettings.Default.IsDebugMode) || level == LogLevel.Info || level == LogLevel.Error)
            {
                if (LogWindow.Instance.log_txtLog.InvokeRequired)
                {
                    LogWindow.Instance.log_txtLog.Invoke(new Action(() => LogWindow.Instance.log_txtLog.AppendText(log + Environment.NewLine)));
                }
                else
                {
                    LogWindow.Instance.log_txtLog.AppendText(log + Environment.NewLine);
                }
            }
            if (BaseSettings.Default.ToSaveLog)
            {
                SaveLog("[" + level.ToString() + "]" + log + Environment.NewLine);
            }
        }

        public static void NewInstance()
        {
            File.AppendAllText("log.txt", Environment.NewLine + "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "][" + Resources.logNewInstance + "]" + Environment.NewLine);
        }

        /// <summary>
        /// Show error log and throw exception if <paramref name="toThrow"/> is true.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="toThrow"></param>
        /// <exception cref="Exception"></exception>
        public static void Error(string log, bool toThrow = true)
        {
            if (!toThrow)
            {
                WriteAndSaveLog(log, 0);
                OnErrorOccur(Resources.logErrorOccur);
            }
            else
            {
                throw new Exception(log);
            }
        }
        public static void Info(string log)
        {
            WriteAndSaveLog(log, 1);
        }
        public static void Debug(string log)
        {
            WriteAndSaveLog(log, 2);
        }
        public static void InfoRevoke(string log)
        {
            WriteAndSaveLog(log, 1);
            OnProcess(log);
        }
        public static void DebugRevoke(string log)
        {
            WriteAndSaveLog(log, 2);
            OnProcess(log);
        }
        public static void ErrorInvalidArchive()
        {
            throw new Exception(Resources.logErrorNotValidArc);
        }
        public static void ErrorNeedAnotherFile(string file)
        {
            throw new Exception(string.Format(Resources.logErrorSpecifiedFileNotFound, file));
        }
        public static void ErrorNeedOriginalFile(string file)
        {
            throw new Exception(string.Format(Resources.logErrorOriginalFileNotFound, file));
        }

        public static void InitPack(string inputFolderPath, string outputFilePath)
        {
            Debug(Resources.logInputFolder + "\t" + inputFolderPath);
            Debug(Resources.logOutputFile + "\t" + outputFilePath);
            Info(Resources.logPacking);
            LogWindow.Instance.bar.Invoke(new Action(() => LogWindow.Instance.bar.Value = 0));
        }
        public static void FinishPack()
        {
            LogWindow.Instance.bar.Invoke(new Action(() => LogWindow.Instance.bar.Value = LogWindow.Instance.bar.Maximum));
            InfoRevoke(Resources.logPackFinished);
        }
        public static void InitUnpack(string inputFilePath, string outputFolderPath)
        {
            Debug(Resources.logInputFile + "\t" + inputFilePath);
            Debug(Resources.logOutputFolder + "\t" + outputFolderPath);
            Info(Resources.logUnpacking);
            LogWindow.Instance.bar.Invoke(new Action(() => LogWindow.Instance.bar.Value = 0));
        }
        public static void FinishUnpack()
        {
            LogWindow.Instance.bar.Invoke(new Action(() => LogWindow.Instance.bar.Value = LogWindow.Instance.bar.Maximum));
            InfoRevoke(Resources.logUnpackFinished);
        }

        internal static void OnProcess(string message)
        {
            Process?.Invoke(null, message);
        }
        internal static void OnErrorOccur(string message)
        {
            ErrorOccured?.Invoke(null, message);
        }
        internal static async Task OnShowAndDisappear(string message, int second = 5)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            try
            {
                OnProcess(message);
                await Task.Delay(second * 1000, token);
                if (!token.IsCancellationRequested)
                {
                    OnProcess(string.Empty);
                }
            }
            catch (TaskCanceledException)
            {
                //ignore
            }
        }

        public static void InitBar(int max)
        {
            InitBar(max, 1);
        }
        public static void InitBar(uint max)
        {
            InitBar(max, 1);
        }
        public static void InitBar(int max, int m)
        {
            LogWindow.Instance.bar.Invoke(new Action(() => LogWindow.Instance.bar.Maximum = m * max));
            Debug(string.Format(Resources.logFileCountInside, max));
        }
        public static void InitBar(uint max, int m)
        {
            LogWindow.Instance.bar.Invoke(new Action(() => LogWindow.Instance.bar.Maximum = m * (int)max));
            Debug(string.Format(Resources.logFileCountInside, max));
        }
        public static void UpdateBar()
        {
            LogWindow.Instance.bar.Invoke(new Action(() => LogWindow.Instance.bar.PerformStep()));
        }

        public static void SetBarValue(int value)
        {
            LogWindow.Instance.bar.Invoke(new Action(() => LogWindow.Instance.bar.Value = value));
        }

        public static void SetBarMax(int max)
        {
            LogWindow.Instance.bar.Invoke(new Action(() => LogWindow.Instance.bar.Maximum = max));
        }

        public static void ResetBar()
        {
            LogWindow.Instance.bar.Invoke(new Action(() => LogWindow.Instance.bar.Value = 0));
        }

        public static void ShowCheckingUpdate()
        {
            InfoRevoke(Resources.logUpdating);
        }
        public static async void ShowCheckSuccess(bool existNewer)
        {
            if (existNewer)
            {
                Info(Resources.logHasUpdate);
                await OnShowAndDisappear(Resources.logHasUpdate);
            }
            else
            {
                Info(Resources.logNoUpdate);
                await OnShowAndDisappear(Resources.logNoUpdate);
            }
        }
        public static void ShowCheckError()
        {
            Error(Resources.logUpdateError, false);
        }
        public static void ShowProgramVersion(string cv, string lv)
        {
            Debug(string.Format(Resources.logVersions, cv, lv));
        }

        public static void ShowVersion(string extension, int version)
        {
            Info(string.Format(Resources.logValidArchiveDetected, extension, version));
        }

        public static void ShowVersion(string extension, string version)
        {
            Info(string.Format(Resources.logValidArchiveDetected, extension, version));
        }
    }
}
