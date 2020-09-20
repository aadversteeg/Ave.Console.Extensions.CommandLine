using System.Collections.Generic;
using Ave.Extensions.Console.CommandLine.Models;

namespace Ave.Extensions.Console.CommandLine
{
    public class CommandLineParseResult
    {
        private CommandLineParseResult(string error)
        {
            Error = error;
        }

        private CommandLineParseResult(Command command, IReadOnlyCollection<ParameterValue> globalParameterValues, IReadOnlyCollection<ParameterValue> parameterValues)
        {
            Command = command;
            GlobalParameterValues = globalParameterValues;
            ParameterValues = parameterValues;
        }

        public static CommandLineParseResult Failure(string error)
        {
            return new CommandLineParseResult(error);
        }

        public static CommandLineParseResult Success(Command command, IReadOnlyCollection<ParameterValue> globalParameterValues, IReadOnlyCollection<ParameterValue> parameterValues)
        {
            return new CommandLineParseResult(command, globalParameterValues, parameterValues);
        }

        public bool IsFailure => ! IsSuccess;

        public bool IsSuccess => string.IsNullOrEmpty(Error);


        public string Error { get; }

        public Command Command { get; }
        public IReadOnlyCollection<ParameterValue> ParameterValues { get; }

        public IReadOnlyCollection<ParameterValue> GlobalParameterValues { get; }
    }
}
