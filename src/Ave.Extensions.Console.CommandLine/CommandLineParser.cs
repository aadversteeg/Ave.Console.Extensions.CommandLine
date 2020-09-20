using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Ave.Extensions.Console.CommandLine.Models;

namespace Ave.Extensions.Console.CommandLine
{
    public class CommandLineParser
    {
        private enum State
        {
            ExpectingCommand,
            ReadingCommand,
            ExpectingParameter,
            ExpectingParameterHyphenOrAbbreviation,
            ExpectingParameterName,
            ReadingParameterName,
            ExpectingParameterValue,
            ReadingParameterValue,
            ExpectingGlobalParameterValue,
            ReadingGlobalParameterValue,
            ReadingParameterAbbreviation
        }


        public enum StringState
        {
            None,
            Inside,
            Escape,
        }


        private readonly CommandGroup _root;
        private readonly IReadOnlyCollection<Parameter> _globalParameters;

        public CommandLineParser(CommandGroup root, IReadOnlyCollection<Parameter> globalParameters)
        {
            _root = root;
            _globalParameters = globalParameters;
        }

        public CommandLineParseResult Parse(string commandLine)
        {
            var tokenizer = new StringTokenizer(commandLine);

            var state = State.ExpectingCommand;
            var stringState = StringState.None;

            var currentCommandGroup = _root;
            var currentCommandGroupPath = _root.Name;
            Command currentCommand = null;
            Parameter currentParameter = null;

            var parameterValues = new List<ParameterValue>();
            var globalParameterValues = new List<ParameterValue>();

            while (tokenizer.MoveNext())
            {
                var currentChar = tokenizer.CurrentChar;

                if (currentChar == ' ' && stringState == StringState.None)  // token separator
                {
                    if (state == State.ReadingCommand)
                    {
                        var token = tokenizer.CurrentToken;
                        var commandOrGroup = currentCommandGroup.Commands.FirstOrDefault(c => c.Name == token);
                        if (commandOrGroup == null)
                        {
                            return CommandLineParseResult.Failure($"'{token}' is not in the '{currentCommandGroupPath}' command group.");
                        }

                        switch (commandOrGroup)
                        {
                            case CommandGroup commandGroup:
                                currentCommandGroup = commandGroup;
                                currentCommandGroupPath = currentCommandGroupPath + " " + commandGroup.Name;
                                state = State.ExpectingCommand;
                                break;

                            case Command command:
                                currentCommand = command;
                                state = State.ExpectingParameter;
                                break;
                        }
                    }
                    else if (state == State.ReadingParameterName)
                    {
                        var token = tokenizer.CurrentToken;
                        var parameter = _globalParameters.FirstOrDefault(p => p.Name == token);
                        if (parameter != null)
                        {
                            currentParameter = parameter;
                            state = State.ExpectingGlobalParameterValue;
                        }
                        else
                        {
                            parameter = currentCommand.Parameters.FirstOrDefault(p => p.Name == token);
                            if (parameter == null)
                            {
                                return CommandLineParseResult.Failure($"Unrecognized argument: '--{token}'");
                            }

                            currentParameter = parameter;
                            state = State.ExpectingParameterValue;
                        }
                    }
                    else if (state == State.ReadingParameterAbbreviation)
                    {
                        var token = tokenizer.CurrentToken;
                        var parameter = _globalParameters.FirstOrDefault(p => p.Abbreviation == token);
                        if (parameter != null)
                        {
                            currentParameter = parameter;
                            state = State.ExpectingGlobalParameterValue;
                        }
                        else
                        {
                            parameter = currentCommand.Parameters.FirstOrDefault(p => p.Abbreviation == token);
                            if (parameter == null)
                            {
                                return CommandLineParseResult.Failure($"Unrecognized argument: '-{token}'");
                            }

                            currentParameter = parameter;
                            state = State.ExpectingParameterValue;
                        }
                    }
                    else if(state == State.ReadingParameterValue)
                    {
                        var token = tokenizer.CurrentToken;
                        var result = currentParameter.Convert(token);
                        if (result.IsFailure)
                        {
                            return CommandLineParseResult.Failure(result.Error);
                        }

                        var parameterValue = new ParameterValue(currentParameter, result.Value);
                        parameterValues.Add(parameterValue);
                        
                        currentParameter = null;
                        state = State.ExpectingParameter;
                    }
                    else if (state == State.ReadingGlobalParameterValue)
                    {
                        var token = tokenizer.CurrentToken;
                        var result = currentParameter.Convert(token);
                        if (result.IsFailure)
                        {
                            return CommandLineParseResult.Failure(result.Error);
                        }
                        var parameterValue = new ParameterValue(currentParameter, result.Value);

                        globalParameterValues.Add(parameterValue);

                        currentParameter = null;
                        state = State.ExpectingParameter;
                    }
                }
                else if(currentChar == '"') // string start or end
                {
                    if(state == State.ExpectingGlobalParameterValue && stringState == StringState.None)
                    {
                        stringState = StringState.Inside; 
                    }
                    else if(state == State.ExpectingParameterValue && stringState == StringState.None)
                    {
                        stringState = StringState.Inside;
                    }
                    else if(state == State.ReadingGlobalParameterValue && stringState == StringState.Inside)
                    {
                        stringState = StringState.None;

                        var token = tokenizer.CurrentToken;
                        var result = currentParameter.Convert(token);
                        if (result.IsFailure)
                        {
                            return CommandLineParseResult.Failure(result.Error);
                        }
                        var parameterValue = new ParameterValue(currentParameter, result.Value);

                        globalParameterValues.Add(parameterValue);

                        currentParameter = null;
                        state = State.ExpectingParameter;
                    }
                    else if (state == State.ReadingParameterValue && stringState == StringState.Inside)
                    {
                        stringState = StringState.None;

                        var token = tokenizer.CurrentToken;
                        var result = currentParameter.Convert(token);
                        if (result.IsFailure)
                        {
                            return CommandLineParseResult.Failure(result.Error);
                        }

                        var parameterValue = new ParameterValue(currentParameter, result.Value);
                        parameterValues.Add(parameterValue);

                        currentParameter = null;
                        state = State.ExpectingParameter;
                    }
                    else
                    {
                        return CommandLineParseResult.Failure($"unexpected \".");
                    }
                }
                else
                {
                    if (state == State.ExpectingCommand)
                    {
                        tokenizer.ClearToken();
                        tokenizer.AddCurrentChar();
                        state = State.ReadingCommand;
                    }
                    else if (state == State.ExpectingParameterValue)
                    {
                        tokenizer.ClearToken();
                        tokenizer.AddCurrentChar();
                        state = State.ReadingParameterValue;
                    }
                    else if (state == State.ExpectingGlobalParameterValue)
                    {
                        tokenizer.ClearToken();
                        tokenizer.AddCurrentChar();
                        state = State.ReadingGlobalParameterValue;
                    }
                    else if (state == State.ExpectingParameter)
                    {
                        if (currentChar != '-')
                        {
                            return CommandLineParseResult.Failure("Expected -");
                        }
                        state = State.ExpectingParameterHyphenOrAbbreviation;
                    }
                    else if (state == State.ExpectingParameterHyphenOrAbbreviation)
                    {
                        if (currentChar != '-')
                        {
                            tokenizer.ClearToken();
                            tokenizer.AddCurrentChar();
                            state = State.ReadingParameterAbbreviation;
                        }
                        else
                        {
                            state = State.ExpectingParameterName;
                        }
                    }
                    else if (state == State.ExpectingParameterName)
                    {
                        tokenizer.ClearToken();
                        tokenizer.AddCurrentChar();
                        state = State.ReadingParameterName;
                    }
                    else if(state == State.ReadingCommand)
                    {
                        tokenizer.AddCurrentChar();
                    }
                    else if (state == State.ReadingParameterValue)
                    {
                        tokenizer.AddCurrentChar();
                    }
                    else if (state == State.ReadingGlobalParameterValue)
                    {
                        tokenizer.AddCurrentChar();
                    }
                    else if (state == State.ReadingParameterName)
                    {
                        tokenizer.AddCurrentChar();
                    }
                    else if (state == State.ReadingParameterAbbreviation)
                    {
                        tokenizer.AddCurrentChar();
                    }
                }
            }

            if (state == State.ReadingCommand)
            {
                var token = tokenizer.CurrentToken;
                var commandOrGroup = currentCommandGroup.Commands.FirstOrDefault(c => c.Name == token);
                if (commandOrGroup == null)
                {
                    return CommandLineParseResult.Failure($"'{token}' is not in the '{currentCommandGroupPath}' command group.");
                }

                switch (commandOrGroup)
                {
                    case Command command:
                        currentCommand = command;
                        break;
                }
            }

            if (state == State.ReadingParameterValue)
            {
                var token = tokenizer.CurrentToken;
                var result = currentParameter.Convert(token);
                if (result.IsFailure)
                {
                    return CommandLineParseResult.Failure(result.Error);
                }

                var parameterValue = new ParameterValue(currentParameter, result.Value);
                parameterValues.Add(parameterValue);
            }

            if (state == State.ReadingGlobalParameterValue)
            {
                var token = tokenizer.CurrentToken;
                var result = currentParameter.Convert(token);
                if(result.IsFailure)
                {
                    return CommandLineParseResult.Failure(result.Error);
                }

                var parameterValue = new ParameterValue(currentParameter, result.Value);
                globalParameterValues.Add(parameterValue);
            }



            // fixup default parameters
            foreach (var parameter in _globalParameters)
            {
                if (parameter.DefaultValue != null)
                {
                    if (!globalParameterValues.Any(gpv => gpv.Parameter.Name == parameter.Name))
                    {
                        var parameterValue = new ParameterValue(parameter, parameter.DefaultValue);
                        globalParameterValues.Add(parameterValue);
                    }
                }
            }

            foreach (var parameter in currentCommand.Parameters)
            {
                if (parameter.DefaultValue != null)
                {
                    if (!parameterValues.Any(gpv => gpv.Parameter.Name == parameter.Name))
                    {
                        var parameterValue = new ParameterValue(parameter, parameter.DefaultValue);
                        parameterValues.Add(parameterValue);
                    }
                }
            }

            foreach (var parameter in _globalParameters)
            {
                if (parameter.Mandatory)
                {
                    if (!globalParameterValues.Any(gpv => gpv.Parameter.Name == parameter.Name))
                    {
                        return CommandLineParseResult.Failure($"missing required parameter '{parameter.Name}'.");
                    }
                }
            }

            foreach (var parameter in currentCommand.Parameters)
            {
                if (parameter.Mandatory)
                {
                    if (!parameterValues.Any(gpv => gpv.Parameter.Name == parameter.Name))
                    {
                        return CommandLineParseResult.Failure($"missing required parameter '{parameter.Name}'.");
                    }
                }
            }

            return CommandLineParseResult.Success(currentCommand, globalParameterValues, parameterValues);
        }
    }
}
