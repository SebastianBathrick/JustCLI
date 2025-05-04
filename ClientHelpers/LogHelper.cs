using System.Reflection.Metadata;

namespace JustCLI.ClientHelpers
{
    public static class LogHelper
    {
        /// <summary> Logs a header enclosed in square braces. </summary>
        public static void LogHeader(string header)
        {
            Log.Info("[{Header}]", header);
        }

        /// <summary> Logs a message between two blank lines. </summary>
        public static void LogWithMargin(string message, params object[] properties) =>
            Log.Info($"\n{message}\n\n", properties);

        /// <summary> Logs a blank line. </summary>
        public static void LogExtraLine(int numLines = 1)
        {
            for (int i = 0; i < numLines; i++)
            {
                Log.Info("");
            }
        }
    }
}
