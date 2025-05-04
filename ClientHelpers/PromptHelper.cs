namespace JustCLI.ClientHelpers
{
    public class PromptHelper
    {
        /// <summary>
        /// Prompts the user for a selection from a list of options. Each option pairs with 
        /// an Action of the same index if the Action array is not null.
        /// </summary>
        /// <returns>Will display options as option array 1-N+1, but will return 0-N</returns>
        public static int PickOption(
            string[] options,
            string? prompt = null,
            Action[]? actions = null)
        {
            if (!string.IsNullOrEmpty(prompt))
                Log.Info("{Prompt}", prompt);

            for (int i = 1; i <= options.Length; i++)
                Log.Info("{Index}: {Option}", i, options[i - 1]);

            do
            {
                Log.Info("{Msg} {Min}-{Max}:", "Please select an option", 1, options.Length);

                string? input = Console.ReadLine();

                if (string.IsNullOrEmpty(input) || !int.TryParse(input, out var selection) ||
                    selection > options.Length || selection < 1)
                {
                    Log.Error("Invalid selection");
                    continue;
                }

                selection--; // Convert to 0-based index
                if (actions != null)
                {
                    actions[selection].Invoke();
                }

                return selection;
            }
            while (true);
        }

        /// <summary> Prompts the user for a yes or no response. </summary>
        public static bool YesNoPrompt(
            string promptMessage,
            bool isDefault = false,
            bool defaultTo = false,
            Action? onYes = null,
            Action? onNo = null
            )
        {
            Log.Info("{Message} (y/n):", promptMessage);
            while (true)
            {
                string? input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    if (isDefault)
                        return defaultTo;
                    else
                        continue;
                }

                if (input.ToLower() == "y")
                {
                    onYes?.Invoke();
                    return true;
                }
                else if (input.ToLower() == "n")
                {
                    onNo?.Invoke();
                    return false;
                }

                Log.Error("Invalid input. Please enter 'y' or 'n':");
            }
        }

    }
}
