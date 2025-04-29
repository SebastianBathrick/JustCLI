using JustCLI.Logging;

namespace JustCLI.Helpers
{
    public static class LogHelper
    {
        /// <summary> Logs a header enclosed in square braces. </summary>
        public static void PrintHeader(string header)
        {
            header = header.ToUpper();
            Log.Info("[{Header}]", header);
        }

        /// <summary> Logs a blank line. </summary>
        public static void LogExtraLine(int numLines = 1)
        {
            for (int i = 0; i < numLines; i++)
            {
                Log.Info("");
            }
        }

        /// <summary> Logs a line using subtract symbols. </summary>
        public static void LogLineVisual()
        {
            Log.Info("--------------------------------------------------");
        }
    }
}
