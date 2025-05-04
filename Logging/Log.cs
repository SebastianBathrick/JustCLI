using JustCLI.Logging.Interface;
using JustCLI.Logging;

namespace JustCLI
{
    /// <summary>Provides a static, global interface for logging messages with configurable output and severity filtering.</summary>
    public static class Log
    {
        private static IJustCLILogger? _instance;

        private static IJustCLILogger Logger
        {
            get
            {
                return _instance ?? throw new InvalidOperationException("Logger not initialized. " +
                    "Call Log.SetLogger() first.");
            }

            set
            {
                _instance = value;
            }
        }

        public static bool IsLogger => _instance != null;

        #region Getter and Setter Methods
        /// <summary>Replaces the default IJustCLILogger with a client defined IJustCLILogger implementation.</summary>
        public static void SetLogger(IJustCLILogger logger) => _instance = logger;

        /// <summary>Sets the minimum message level that will be logged.</summary>
        public static void SetMinimumLevel(MessageLevel messageLevel) =>
            Logger.SetMinimumLogLevel(messageLevel);

        /// <summary>Gets the current minimum log level.</summary>
        public static MessageLevel GetMinimumLevel() =>
            Logger.GetMinimumLogLevel();
        #endregion

        #region Message Methods
        /// <summary>Logs an informational message.</summary>
        public static void Info(string message, params object[] properties)
            => Logger.Info(message, properties);

        /// <summary>Logs a warning message.</summary>
        public static void Warning(string message, params object[] properties)
            => Logger.Warning(message, properties);

        /// <summary>Logs an error message with an exception.</summary>
        public static void Error(Exception ex, string message, params object[] properties)
            => Logger.Error(ex, message, properties);

        /// <summary>Logs an error message.</summary>
        public static void Error(string message, params object[] properties)
            => Logger.Error(message, properties);

        /// <summary>Logs a fatal error with an exception.</summary>
        public static void Fatal(Exception ex, string message, params object[] properties)
            => Logger.Fatal(message, properties);

        /// <summary>Logs a fatal error message.</summary>
        public static void Fatal(string message, params object[] properties)
            => Logger.Fatal(message, properties);

        /// <summary>Logs a debug-level diagnostic message.</summary>
        public static void Debug(string message, params object[] properties)
            => Logger.Debug(message, properties);
        #endregion
    }
}
