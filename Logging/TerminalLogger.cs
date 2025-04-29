using JustCLI.Logging.Interface;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace JustCLI.Logging.Default
{
    /// <summary>
    /// Provides a console-based implementation of <see cref="ILogger"/> that supports:
    /// structured message templates with placeholders, ANSI-colored output,
    /// severity-based message filtering, and cross-platform ANSI support.
    /// Includes specialized handling for exceptions and configurable minimum log level.
    /// </summary>
    internal class TerminalLogger : IJustCLILogger
    {
        private const int PROPERTY_COLOR = 36; // Cyan for inserted values

        #region Variables
        private readonly Regex _propertyRegex = new(@"\{[^}]+\}");
        private bool _useAnsi { get; set; } = TryEnableAnsi();
        private MessageLevel _minLevel = MessageLevel.Debug;
        #endregion

        #region Getter and Setter Methods
        public MessageLevel GetMinimumLogLevel() =>
            _minLevel;

        public void SetMinimumLogLevel(MessageLevel level) =>
            _minLevel = level;
        #endregion

        #region Message Methods
        public void Debug(string message, params object[] properties)
            => WriteLog(MessageLevel.Debug, DEBUG_LABEL_TEXT, DEBUG_COLOR, message, properties);

        public void Error(string message, params object[] properties)
            => WriteLog(MessageLevel.Error, ERROR_LABEL_TEXT, ERROR_COLOR, message, properties);

        public void Error(Exception ex, string message, params object[] properties)
        {
            Error(message, properties);
            if (MessageLevel.Error <= _minLevel)
            {
                Console.WriteLine("Exception thrown:");
                Console.WriteLine(ex.ToString());
            }
        }

        public void Fatal(Exception ex, string message, params object[] properties)
        {
            Fatal(message, properties);
            if (MessageLevel.Error <= _minLevel)
            {
                Console.WriteLine("Exception thrown:");
                Console.WriteLine(ex.ToString());
            }
        }

        public void Fatal(string message, params object[] properties)
            => WriteLog(MessageLevel.Fatal, FATAL_LABEL_TEXT, FATAL_COLOR, message, properties);

        public void Info(string message, params object[] properties)
            => WriteLog(MessageLevel.Info, INFO_LABEL_TEXT, INFO_COLOR, message, properties);

        public void Warning(string message, params object[] properties)
            => WriteLog(MessageLevel.Warning, WARNING_LABEL_TEXT, WARNING_COLOR, message, properties);
        #endregion

        #region Helper Methods
        private void WriteColor(string text, int fgColorCode)
        {
            if (_useAnsi)
                Console.Write($"\x1b[{fgColorCode}m{text}\x1b[0m");
            else
                Console.Write(text);
        }

        private void WriteLabel(string label, int colorCode)
        {
            if (string.IsNullOrEmpty(label))
                return;

            Console.Write("[");
            WriteColor(label, colorCode);
            Console.Write("]: ");
        }

        /// <summary>
        /// Writes a template string to the console, replacing placeholders with colored values.
        /// </summary>
        /// <param name="template">The message template containing placeholders like {Name}.</param>
        /// <param name="colorCode">ANSI color code to use for inserted values.</param>
        /// <param name="values">Values to substitute into the template in order.</param>
        private void WriteTemplate(string template, int colorCode, params object[] values)
        {
            int currentIndex = 0, valueIndex = 0;

            foreach (Match match in _propertyRegex.Matches(template))
            {
                Console.Write(template.Substring(currentIndex, match.Index - currentIndex));

                if (valueIndex < values.Length)
                    WriteColor(values[valueIndex++]?.ToString() ?? "", colorCode);

                currentIndex = match.Index + match.Length;
            }

            if (currentIndex < template.Length)
                Console.Write(template.Substring(currentIndex));
        }

        /// <summary>
        /// Writes a log entry to the console if it meets the current minimum log level.
        /// </summary>
        /// <param name="level">The severity level of the message.</param>
        /// <param name="label">The label (e.g., "Error") to display with the message.</param>
        /// <param name="labelColor">ANSI color code to apply to the label.</param>
        /// <param name="message">The message template to log.</param>
        /// <param name="values">Values to substitute into the message template.</param>
        private void WriteLog(MessageLevel level, string label, int labelColor, string message, object[] values)
        {
            if (level > _minLevel)
                return;

            WriteLabel(label, labelColor);
            WriteTemplate(message, PROPERTY_COLOR, values);
            Console.WriteLine();
        }

        /// <summary>
        /// Attempts to enable ANSI escape sequence processing on Windows consoles.
        /// </summary>
        /// <returns><c>true</c> if ANSI processing is supported or enabled; otherwise, <c>false</c>.</returns>
        private static bool TryEnableAnsi()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return true;

            const int STD_OUTPUT_HANDLE = -11;
            const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

            IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (!GetConsoleMode(handle, out uint mode))
                return false;

            return SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
        }

        /// <summary>
        /// Gets the handle for the specified standard device (used for enabling ANSI support).
        /// </summary>
        /// <param name="nStdHandle">The standard device handle (-11 for output).</param>
        /// <returns>A handle to the standard device.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        /// <summary>
        /// Retrieves the current console mode settings.
        /// </summary>
        /// <param name="hConsoleHandle">A handle to the console.</param>
        /// <param name="lpMode">Receives the current mode flags.</param>
        /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        /// <summary>
        /// Sets the console mode flags for the specified console handle.
        /// </summary>
        /// <param name="hConsoleHandle">A handle to the console.</param>
        /// <param name="dwMode">The mode flags to set.</param>
        /// <returns><c>true</c> if the operation succeeds; otherwise, <c>false</c>.</returns>
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        #endregion

        #region Constants
        // Label texts
        private const string FATAL_LABEL_TEXT = "Fatal";
        private const string ERROR_LABEL_TEXT = "Error";
        private const string WARNING_LABEL_TEXT = "Warning";
        private const string INFO_LABEL_TEXT = "";
        private const string DEBUG_LABEL_TEXT = "Debug";

        // Label colors
        private const int FATAL_COLOR = 31;
        private const int ERROR_COLOR = 91;
        private const int WARNING_COLOR = 33;
        private const int INFO_COLOR = 32;
        private const int DEBUG_COLOR = 90;
        #endregion
    }
}
