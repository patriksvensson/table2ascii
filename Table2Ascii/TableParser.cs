using HtmlAgilityPack;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq;

namespace TableToAscii
{
    public static class TableParser
    {
        public static Table Parse(string html, int[] columnWidths, bool expand)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var htmlTable = doc.DocumentNode.SelectNodes("//table").Cast<HtmlNode>().FirstOrDefault();
            if (htmlTable == null)
            {
                return null;
            }

            var columns = htmlTable.SelectNodes("tr/th").Cast<HtmlNode>();
            var widths = new Queue<int>(columnWidths);

            var table = new Table().MinimalBorder();
            table.UseSafeBorder = false;
            table.Expand = expand;
            foreach (var column in columns)
            {
                table.AddColumn(column.InnerText, c =>
                {
                    if (widths.Count > 0)
                    {
                        c.Width = widths.Dequeue();
                    }
                });
            }

            var rows = htmlTable.SelectNodes("tr/td").Cast<HtmlNode>();
            foreach (var row in rows.Batch(table.Columns.Count))
            {
                table.AddRow(row.Select(x => x.InnerText).ToArray());
            }

            return table;
        }
    }
}
