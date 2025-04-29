using JustCLI.Logging.Default;

namespace JustCLI.Logging
{
    /// <summary>Provides a static, global interface for logging messages with configurable output and severity filtering.</summary>
    public static class Log
    {
        private static ILogger _logger = new TerminalLogger();

        #region Getter and Setter Methods
        /// <summary>Sets the minimum message level that will be logged.</summary>
        public static void SetMinimumLevel(MessageLevel messageLevel) =>
            _logger.SetMinimumLogLevel(messageLevel);

        /// <summary>Gets the current minimum log level.</summary>
        public static MessageLevel GetMinimumLevel() =>
            _logger.GetMinimumLogLevel();

        /// <summary>Replaces the current logger with a custom implementation.</summary>
        public static void SetLogger(ILogger logger)
            => _logger = logger;
        #endregion

        #region Message Methods
        /// <summary>Logs an informational message.</summary>
        public static void Info(string message, params object[] properties)
            => _logger.Info(message, properties);

        /// <summary>Logs a warning message.</summary>
        public static void Warning(string message, params object[] properties)
            => _logger.Warning(message, properties);

        /// <summary>Logs an error message with an exception.</summary>
        public static void Error(Exception ex, string message, params object[] properties)
            => _logger.Error(ex, message, properties);

        /// <summary>Logs an error message.</summary>
        public static void Error(string message, params object[] properties)
            => _logger.Error(message, properties);

        /// <summary>Logs a fatal error with an exception.</summary>
        public static void Fatal(Exception ex, string message, params object[] properties)
            => _logger.Fatal(message, properties);

        /// <summary>Logs a fatal error message.</summary>
        public static void Fatal(string message, params object[] properties)
            => _logger.Fatal(message, properties);

        /// <summary>Logs a debug-level diagnostic message.</summary>
        public static void Debug(string message, params object[] properties)
            => _logger.Debug(message, properties);
        #endregion
    }
}
