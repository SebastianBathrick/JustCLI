using JustCLI;
using JustCLI.Utilities;
using Serilog;

// Setup logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

// Add commands using the ActionCommand class
CLI.AddCommands([
    new ActionCommand(
        "hello",
        "Prints hello world.",
        (flagEntries) => {
            Log.Information("Hello, world!");
        }
    ),
    new ActionCommand(
        "greet",
        "Greets a user by name.",
        (flagEntries) => {
            if (flagEntries.TryGetValue("name", out string? name))
                Log.Information("Hello, {Name}!", name);
            else
                Log.Information("Hello, anonymous user!");
        },
        [new Flag("name", "The name to greet", false, true)]
    )
]);

// Set application version
CLI.SetVersion("1.0.0");

// Add a default command (executes when no command is provided)
CLI.AddDefaultCommand(new ActionCommand(
    "default",
    "Default command",
    (flagEntries) => {
        Log.Information("Default command executed");
    }
));

// Define a custom logger (optional)
var customLogger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();
CLI.DefineLogger(customLogger);

// Start the CLI with options
CLI.Start(
    requireCommand: false,       // Error if no command is provided
    allowMoreCommands: true,    // Allow the user to enter more commands after execution
    argOverride: null,          // Override command-line arguments (optional)
    useBuiltInCommands: true    // Include built-in commands (help, version, clear, exit)
);

string input = PrimitiveIOHelper.GetStringFromUser("input name");
int number = PrimitiveIOHelper.GetIntFromUser("number");
float myNum = PrimitiveIOHelper.GetFloatFromUser("decimal");
bool flag = PrimitiveIOHelper.GetBoolFromUser("boolean");