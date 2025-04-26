using Serilog;

namespace JustCLI.Utilities
{
    public enum InfoProps
    {
        Enclosed,
        Labeled,
        LabeledWithExtraSpace,
    }

    public enum LabelProps
    {
        Enclosed,
        Highlighted,
        Breakline,
        BreaklineWithExtraSpace,
        ColonLabel,
    }

    public static class LogHelper
    {


        private const string OPEN_ENCLOSE = "[";
        private const string CLOSE_ENCLOSE = "]";

        public static void PrintHeader(string header)
        {
            header = header.ToUpper();
            Log.Information("[{Header}]", header);
        }

        public static void LogExtraLine(int numLines = 1)
        {
            for (int i = 0; i < numLines; i++)
            {
                Log.Information("");
            }
        }

        public static void LogLineVisual()
        {
            Log.Information("--------------------------------------------------");
        }
    }
}
