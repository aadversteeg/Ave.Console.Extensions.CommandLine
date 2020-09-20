using Ave.Extensions.Console.CommandLine;
using Ave.Extensions.Console.CommandLine.Models;
using FluentAssertions;
using Xunit;

namespace UnitTests.Extensions.Console.CommandLine
{
    public class CommandLineParseResultTests
    {
        [Fact(DisplayName= "CLPR-001: Result should indicate success after calling Success.")]
        public void CLPR001()
        {
            // arrange
            var command = new Command("test");

            // act
            var result = CommandLineParseResult.Success( command, new ParameterValue[0], new ParameterValue[0]);

            // assert
            result.IsFailure.Should().BeFalse();
            result.IsSuccess.Should().BeTrue();
            result.Command.Should().BeSameAs(command);
        }

        [Fact(DisplayName = "CLPR-002: Result should indicate failure after calling Failure.")]
        public void CLPR002()
        {
            // arrange
            // act
            var result = CommandLineParseResult.Failure("Error message");

            // assert
            result.IsFailure.Should().BeTrue();
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Error message");
        }
    }
}
