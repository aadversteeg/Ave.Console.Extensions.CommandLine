using Ave.Extensions.Console.CommandLine;

namespace UnitTests.Extensions.Console.CommandLine
{
    public static class CommandLineParseResultExtensions
    {
        public static CommandLineParseResultAssertions  Should(this CommandLineParseResult value) => new CommandLineParseResultAssertions (value);
    }
}
