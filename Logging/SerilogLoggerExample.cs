/*using JustCLI.Logging.Interface;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace JustCLI.Logging
{
    /// <summary>
    /// A simple Serilog-based logger that writes output to the console.
    /// </summary>
    public class SerilogConsoleLogger : IJustCLILogger
    {
        // The Serilog logger instance used internally
        private Logger _logger;

        // Allows the minimum log level to be changed at runtime
        private LoggingLevelSwitch _levelSwitch;

        // Stores the current minimum log level for this application
        private MessageLevel _minimumLogLevel = MessageLevel.Info;

        public SerilogConsoleLogger()
        {
            // Create a level switch to control Serilog's filtering dynamically
            _levelSwitch = new LoggingLevelSwitch();

            // Build the logger configuration
            _logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(_levelSwitch) // Connect level switch to Serilog
                .WriteTo.Console()                      // Log output to the console
                .CreateLogger();                        // Instantiate the logger

            // Set the initial log level to Info
            SetMinimumLogLevel(_minimumLogLevel);
        }

        public void Debug(string message, params object[] properties)
        {
            // Log a debug-level message
            _logger.Debug(message, properties);
        }

        public void Error(string message, params object[] properties)
        {
            // Log an error-level message
            _logger.Error(message, properties);
        }

        public void Error(Exception ex, string message, params object[] properties)
        {
            // Log an error message along with an exception
            _logger.Error(ex, message, properties);
        }

        public void Fatal(Exception ex, string message, params object[] properties)
        {
            // Log a fatal message along with an exception
            _logger.Fatal(ex, message, properties);
        }

        public void Fatal(string message, params object[] properties)
        {
            // Log a fatal message without an exception
            _logger.Fatal(message, properties);
        }

        public MessageLevel GetMinimumLogLevel()
        {
            // Return the currently stored minimum log level
            return _minimumLogLevel;
        }

        public void Info(string message, params object[] properties)
        {
            // Log an informational message
            _logger.Information(message, properties);
        }

        public void SetMinimumLogLevel(MessageLevel level)
        {
            // Store the new minimum level internally
            _minimumLogLevel = level;

            // Map MessageLevel to Serilog's LogEventLevel
            _levelSwitch.MinimumLevel = level switch
            {
                MessageLevel.Debug => LogEventLevel.Debug,
                MessageLevel.Info => LogEventLevel.Information,
                MessageLevel.Warning => LogEventLevel.Warning,
                MessageLevel.Error => LogEventLevel.Error,
                MessageLevel.Fatal => LogEventLevel.Fatal,
                _ => LogEventLevel.Information
            };
        }

        public void Warning(string message, params object[] properties)
        {
            // Log a warning-level message
            _logger.Warning(message, properties);
        }
    }
}
*/