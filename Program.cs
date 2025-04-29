using JustCLI.BuiltInCommands;
using JustCLI;

internal class Program
{
    static void Main(string[] args)
    {
        // Register real commands
        CLI.AddCommands([
            new VersionCommand(),
            new ExitCommand(),
            new ClearTerminalCommand()
        ]);

        // Set a default command
        CLI.AddDefaultCommand(new ActionCommand(
            name: "default",
            description: "Runs when no arguments are provided.",
            executeAction: flags => Console.WriteLine("No command provided. Use --help.")
        ));

        // Start CLI
        CLI.Start(
            requireCommand: false,
            allowMoreCommands: false,
            argOverride: args,
            useHelp: true
        );
    }
}
