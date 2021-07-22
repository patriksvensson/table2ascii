using Spectre.Console.Cli;

namespace TableToAscii
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandApp<DefaultCommand>();
            return app.Run(args);
        }
    }
}
