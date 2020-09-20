using Ave.Extensions.Console.CommandLine;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace UnitTests.Extensions.Console.CommandLine
{
    public class CommandLineParseResultAssertions : ReferenceTypeAssertions<CommandLineParseResult, CommandLineParseResultAssertions>
    {
        public CommandLineParseResultAssertions(CommandLineParseResult value)
            : base(value)
        {
        }

        public AndConstraint<CommandLineParseResultAssertions> BeSuccessful()
        {
            Execute
                .Assertion
                .ForCondition(Subject.IsSuccess)
                .FailWith(() => new FailReason($"Expected successful result, but got error message(s): { GetError(Subject) }"));

            return new AndConstraint<CommandLineParseResultAssertions>(this);
        }

        public AndConstraint<CommandLineParseResultAssertions> BeFailure()
        {
            Execute
                .Assertion
                .ForCondition(Subject.IsFailure)
                .FailWith(() => new FailReason($"Expected failure result, but got success."));

            return new AndConstraint<CommandLineParseResultAssertions>(this);
        }

        protected override string Identifier => "CommandLineParseResult";

        private static string GetError(CommandLineParseResult result)
        {
            return result.Error;
        }
    }
}
