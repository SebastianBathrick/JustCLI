/*
using System;
using JustCLI;
using Serilog;


internal class Program
{
    static void Main(string[] args)
    {
        CLI.Start(requireCommand:true, allowMoreCommands:true);
        CLI.AddCommands(new ICommand[]
            {
                new ActionCommand(
                    "sayhi",
                    "Says hi to the user.",
                    flags => Log.Information("Hi there!")
                )
            });
        CLI.Start(requireCommand: true, allowMoreCommands: true, argOverride: ["--sayhi"]);

        CLI.RemoveCommand("sayhi");
        CLI.Start(requireCommand: false, allowMoreCommands:true);
    }
}
*/