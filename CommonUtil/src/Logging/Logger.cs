using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommonUtil.Logging {
    public enum LogLevel { Verbose = 0, Debug = 1, Info = 2, Warn = 3, Critical = 4 }
    public class Logger<T> {
        public LogLevel LogLevel { get; set; } = LogLevel.Info;

        private string GetTsStr() {
            return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff");
        }

        private string GetEntryPrefix(LogLevel level) {
            return GetTsStr() + " " + typeof(T).Name + " " + level.ToString() + ": ";
        }

        private void WriteToLog(string logEntry) {
            Console.WriteLine(logEntry);
        }

        public void Log(LogLevel level, string message) {
            string logEntry = GetEntryPrefix(level) + message;
            Task.Run(() => WriteToLog(logEntry));
        }

        public void Info(string message) {
            Log(LogLevel.Info, message);
        }

        public void Warn(string message) {
            Log(LogLevel.Warn, message);
        }

        public void Debug(string message) {
            Log(LogLevel.Debug, message);
        }

        public void Critical(string message) {
            Log(LogLevel.Critical, message);
        }

        public void LogException(LogLevel level, Exception ex) {
            string msg = $"Error: {ex.GetType().Name} - {ex.Message}{Environment.NewLine}{ex.StackTrace}";
            if (ex.InnerException != null)
                msg += $"{Environment.NewLine}Caused by: "
                    + $"{ex.InnerException.GetType().Name} - {ex.InnerException.Message}"
                    + $"{Environment.NewLine}{ex.InnerException.StackTrace}";
            Log(level, msg);
        }
    }
}
