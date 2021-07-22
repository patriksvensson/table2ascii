using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace TableToAscii
{
    public sealed class DefaultCommand : Command<DefaultCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [CommandArgument(0, "<INPUT>")]
            [Description("The HTML file containing the table")]
            public string Input { get; set; }

            [CommandOption("-o|--output <OUTPUT>")]
            [Description("The ASCII output file")]
            public string Output { get; set; }

            [DefaultValue(80)]
            [CommandOption("-w|--width <COLUMNS>")]
            [Description("The width of the table")]
            public int Width { get; set; }

            [CommandOption("-c|--column <WIDTHS>")]
            [Description("The width of the columns")]
            public int[] Columns { get; set; }

            [CommandOption("-e|--expand")]
            [Description("Expands the table to occupy the full width")]
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
                settings.Columns,
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

            if (settings.Columns?.Length == 0 ||
                settings.Columns?.Sum() < settings.Width)
            {
                console.Profile.Width = settings.Width;
            }

            console.Write(table);
            File.WriteAllText(settings.Output, writer.ToString());
        }
    }
}
