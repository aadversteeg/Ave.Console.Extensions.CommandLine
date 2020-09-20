using Ave.Extensions.Console.CommandLine;
using FluentAssertions;
using Xunit;

namespace UnitTests.Extensions.Console.CommandLine
{
    public class CommandLineSplitterTests
    {
        [Theory(DisplayName = "Splitting valid command lines should succeed.")]
        [InlineData("c:\\bin\\todo.exe", "c:\\bin\\todo.exe", "")]
        [InlineData( "\"c:\\My Directory\\todo.exe\"", "c:\\My Directory\\todo.exe", "")]
        [InlineData("c:\\bin\\todo.exe some arguments...", "c:\\bin\\todo.exe", "some arguments...")]
        [InlineData("\"c:\\My Directory\\todo.exe\" some arguments...", "c:\\My Directory\\todo.exe", "some arguments...")]
        [InlineData("\"c:\\My Directory\\todo.exe\" some arguments...    ", "c:\\My Directory\\todo.exe", "some arguments...    ")]
        public void CLS001(string commandLine, string expectedCommand, string expectedArguments) 
        {
            // arrange, act
            var result = CommandLineSplitter.Split(commandLine);

            // assert
            result.Command.Should().Be(expectedCommand);
            result.Arguments.Should().Be(expectedArguments);
        }
    }
}
