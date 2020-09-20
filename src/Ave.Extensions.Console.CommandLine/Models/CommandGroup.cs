using System.Collections.Generic;

namespace Ave.Extensions.Console.CommandLine.Models
{
    public class CommandGroup : CommandBase
    {
        public CommandGroup(string name, IReadOnlyCollection<CommandBase> commands)
            : base(name)
        {
            Commands = commands;
        }

        public IReadOnlyCollection<CommandBase> Commands { get; }
    }
}
