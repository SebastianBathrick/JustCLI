namespace JustCLI.Logging.Interface
{
    /// <summary>Defines a logger capable of writing messages with different severity levels.</summary>
    public interface IJustCLILogger
    {
        /// <summary>Logs an informational message.</summary>
        void Info(string message, params object[] properties);

        /// <summary>Logs a warning message.</summary>
        void Warning(string message, params object[] properties);

        /// <summary>Logs an error message.</summary>
        void Error(string message, params object[] properties);

        /// <summary>Logs an error message with an associated exception.</summary>
        void Error(Exception ex, string message, params object[] properties);

        /// <summary>Logs a fatal error message with an associated exception.</summary>
        public void Fatal(Exception ex, string message, params object[] properties);

        /// <summary>Logs a fatal error message.</summary>
        void Fatal(string message, params object[] properties);

        /// <summary>Logs a debug message.</summary>
        void Debug(string message, params object[] properties);

        /// <summary>Sets the minimum level of messages to be logged.</summary>
        MessageLevel GetMinimumLogLevel();

        /// <summary>Sets the minimum level of messages to be logged.</summary>
        void SetMinimumLogLevel(MessageLevel level);
    }
}
