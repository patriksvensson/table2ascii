using Spectre.Console.Cli;

namespace TableToAscii
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandApp<DefaultCommand>();
            app.Configure(config =>
            {
                config.AddExample(new[] 
                { 
                    "input.html", "--output", "output.txt",
                    "--column", "5", "--column", "10"
                });

                config.AddExample(new[]
                {
                    "input.html", "--output", "output.txt", "--width", "80"
                });

                config.ValidateExamples();
            });
            return app.Run(args);
        }
    }
}
