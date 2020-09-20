using Ave.Extensions.Console.CommandLine.Models;

namespace Ave.Extensions.Console.CommandLine
{
    public static class CommandLineSplitter
    {
        public static CommandLineSplitResult Split(string commandLine)
        {
            var length = commandLine.Length;

            char endOfCommand = commandLine[0] == '\"' ? '\"' : ' ';

            int pos = 1;
            while (pos < length)
            {
                if(commandLine[pos] == endOfCommand)
                {
                    break;
                }
                pos++;
            }

            if(pos < length)
            {
                if(commandLine[pos] == '\"')
                {
                    pos++;
                }
            }

            if(pos == length)
            {
                return new CommandLineSplitResult
                {
                    Command = commandLine[0] == '\"' ? commandLine.Substring(1, commandLine.Length-2) : commandLine,
                    Arguments = ""
                };
            }

            return new CommandLineSplitResult
            {
                Command = commandLine[0] == '\"' ? commandLine.Substring(1, pos - 2) : commandLine.Substring(0, pos),
                Arguments = commandLine.Substring(pos+1)
            };
        }
    }
}
