﻿using ERHMS.Presentation.Services;
using ERHMS.Utility;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ERHMS.Presentation
{
    public class PrintService : IPrintService
    {
        private static readonly Thickness PagePadding = new Thickness(48.0);
        private static readonly FontFamily FontFamily = new FontFamily("Consolas");
        private const double FontSize = 12.0;

        public void Print(string title, string body)
        {
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                FlowDocument document = new FlowDocument
                {
                    ColumnWidth = dialog.PrintableAreaWidth,
                    PagePadding = PagePadding,
                    FontFamily = FontFamily,
                    FontSize = FontSize
                };
                Paragraph paragraph = new Paragraph();
                foreach (string line in body.SplitLines())
                {
                    paragraph.Inlines.Add(new Run(line));
                    paragraph.Inlines.Add(new LineBreak());
                }
                document.Blocks.Add(paragraph);
                dialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, title);
            }
        }
    }
}