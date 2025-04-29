using Xunit;
using JustCLI.BuiltInCommands;

namespace JustCLI.Tests
{
    public class CLITests
    {
        public CLITests()
        {
            CLI.ClearCommands();
            CLI.SetExiting(false);
        }

        [Fact]
        public void Start_WhenNoArgumentsProvided_ShouldExecuteDefaultCommand()
        {
            var executed = false;

            var defaultCommand = new ActionCommand(
                name: "default",
                description: "default",
                executeAction: _ => executed = true
            );

            var dummyCommand = new ActionCommand(
                name: "placeholder",
                description: "placeholder",
                executeAction: _ => { }
            );

            CLI.ClearCommands();
            CLI.AddCommand(dummyCommand);          // Required!
            CLI.AddDefaultCommand(defaultCommand);

            CLI.Start(requireCommand: false, allowMoreCommands: false, argOverride: new string[] { });

            Assert.True(executed);
        }

        [Fact]
        public void Start_WhenInvalidCommandProvided_ShouldNotThrow()
        {
            var exception = Record.Exception(() =>
                CLI.Start(requireCommand: true, allowMoreCommands: false, argOverride: new[] { "--invalid" })
            );

            Assert.Null(exception);
        }

        [Fact]
        public void Start_WhenValidCommandProvided_ShouldExecute()
        {
            var ran = false;
            var myCommand = new ActionCommand(
                name: "do",
                description: "do",
                executeAction: _ => ran = true
            );

            CLI.AddCommand(myCommand);

            CLI.Start(requireCommand: true, allowMoreCommands: false, argOverride: new[] { "--do" });

            Assert.True(ran);
        }

        [Fact]
        public void Start_WhenHelpCommandIsCalled_ShouldNotThrow()
        {
            CLI.AddCommand(new HelpCommand());

            var exception = Record.Exception(() =>
                CLI.Start(requireCommand: true, allowMoreCommands: false, argOverride: new[] { "--help" })
            );

            Assert.Null(exception);
        }

        [Fact]
        public void Start_WhenExitCommandIsCalled_ShouldNotThrow()
        {
            CLI.AddCommand(new ExitCommand());

            var exception = Record.Exception(() =>
                CLI.Start(requireCommand: true, allowMoreCommands: false, argOverride: new[] { "--exit" })
            );

            Assert.Null(exception);
        }
    }
}
