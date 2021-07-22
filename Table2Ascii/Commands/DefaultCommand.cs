using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace TableToAscii
{
    public sealed class DefaultCommand : Command<DefaultCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [CommandArgument(0, "<INPUT>")]
            public string Input { get; set; }

            [CommandOption("-o|--output <OUTPUT>")]
            public string Output { get; set; }

            [DefaultValue(80)]
            [CommandOption("-w|--width <COLUMNS>")]
            public int Width { get; set; }

            [CommandOption("-e|--expand")]
            public bool Expand { get; set; }
        }

        public override ValidationResult Validate(CommandContext context, Settings settings)
        {
            if (!File.Exists(settings.Input))
            {
                return ValidationResult.Error("File does not exist");
            }

            if (!string.IsNullOrWhiteSpace(settings.Output))
            {
                settings.Output = Path.GetFullPath(settings.Output);
            }

            return base.Validate(context, settings);
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            // Parse the table
            Table table = TableParser.Parse(
                File.ReadAllText(settings.Input),
                settings.Expand);

            // Write the table to the console
            AnsiConsole.Render(table);

            // Export the table?
            if (settings.Output != null)
            {
                ExportTable(settings, table);
                AnsiConsole.MarkupLine($"Wrote table to [yellow]{settings.Output}[/]");
            }

            return 0;
        }

        private static void ExportTable(Settings settings, Table table)
        {
            var writer = new StringWriter();
            var console = AnsiConsole.Create(new AnsiConsoleSettings
            {
                Out = new AnsiConsoleOutput(writer)
            });

            console.Profile.Width = settings.Width;
            console.Write(table);
            File.WriteAllText(settings.Output, writer.ToString());
        }
    }
}
