// See https://aka.ms/new-console-template for more information
using JustCLI;

CommandLineApp.SetVersion("1.0.0");
CommandLineApp.AddCommands([new OpenFileTest()]);
CommandLineApp.AddDefaultCommand(new OpenFileTest());
CommandLineApp.Start(requireCommand: false, isPromptBeforeExit:true);

