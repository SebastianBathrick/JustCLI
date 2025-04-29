using JustCLI.Logging;

namespace JustCLI.Helpers
{
    public class PrimitiveIOHelper
    {
        /// <summary> Gets a int literal from the user. </summary>
        /// <remarks> Loops continously until the user makes a valid entry. </remarks>
        public static int GetIntFromUser(string valueName)
        {
            Log.Info("Please enter a value for {ValueName} (int):", valueName);

            while (true)
            {
                string? input = Console.ReadLine();
                if (int.TryParse(input, out int result))
                    return result;
                Log.Error("Invalid argument. Please enter a valid integer:");
            }
        }

        /// <summary> Gets a float literal from the user. </summary>
        /// <remarks> Loops continously until the user makes a valid entry. </remarks>
        public static float GetFloatFromUser(string valueName)
        {
            Log.Info("Please enter a value for {ValueName} (float):", valueName);
            while (true)
            {
                string? input = Console.ReadLine();
                if (float.TryParse(input, out float result))
                    return result;
                Log.Error("Invalid argument. Please enter a valid float:");
            }
        }

        /// <summary> Gets a string literal from the user. </summary>
        /// <remarks> Loops continously until the user makes a valid entry. </remarks>
        public static string GetStringFromUser(string valueName)
        {
            Log.Info("Please enter a value for {ValueName} (string):", valueName);
            while (true)
            {
                string? input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                    return input;
                Log.Error("Invalid argument. Please enter a valid string:");
            }
        }

        /// <summary> Gets a boolean literal from the user. </summary>
        /// <remarks> Loops continously until the user makes a valid entry. </remarks>
        public static bool GetBoolFromUser(string valueName)
        {
            Log.Info("Please enter a value for {ValueName} (true or false):", valueName);
            while (true)
            {
                string? input = Console.ReadLine();
                if (bool.TryParse(input, out bool result))
                    return result;
                Log.Error("Invalid argument. Please enter a valid boolean:");
            }
        }
    }
}
