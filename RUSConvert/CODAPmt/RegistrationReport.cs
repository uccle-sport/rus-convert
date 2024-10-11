﻿using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data.Common;

namespace RUSConvert.CODAPmt
{
    internal class RegistrationReport
    {
        byte[]? LogoRUS;

        public void CreateDocuments(List<PaymentSource> sourceLines, string FileName)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            LogoRUS = File.ReadAllBytes("Files/BlasonRUS.png");
            var reports =
                from sourceline in sourceLines
                group sourceline by sourceline.TwizzitId into reportlines
                select new RegistrationModel
                {
                    Name = reportlines.First().Name?.ToUpper() ?? "",
                    IBAN = reportlines.First().IBAN?.ToUpper() ?? "",
                    BirthDate = reportlines.First().BirthDate,
                    Lines = [.. reportlines]
                };
            foreach (var report in reports)
            {
                report.IssueDate = DateTime.Now;
                var document = CreateDocument(report);
                document.GeneratePdf(Path.Combine(Properties.Settings.Default.PaymentsDestFolder, report.Name + " PRESTATIONS " + FileName + ".pdf"));
            }
        }

        private Document CreateDocument(RegistrationModel model)
        {
            void ComposeHeader(IContainer container)
            {
                var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                container.Row(row =>
                {
                    if (LogoRUS != null) row.ConstantItem(200).Height(100).Image(LogoRUS);
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text($"{model.Name}").Style(titleStyle);

                        column.Item().Text(text =>
                        {
                            text.Span("Date: ").SemiBold();
                            text.Span($"{model.IssueDate:d}");
                        });

                        column.Item().Text(text =>
                        {
                            text.Span("Compte: ").SemiBold();
                            text.Span($"{model.IBAN}");
                        });

                    });

                    //if (LogoRUS != null) row.ConstantItem(200).Height(100).Image(LogoRUS);
                });
            }


            void ComposeComments(IContainer container)
            {
                container.Background(Colors.Grey.Lighten3).Padding(5).Column(column =>
                {
                    column.Item().Text("FICHE DE PRESTATIONS").FontSize(16);
                });
            }
            void ComposeContent(IContainer container)
            {
                container.Column(column =>
                {
                    column.Item().PaddingTop(25).Element(ComposeComments);

                    column.Spacing(5);

                    column.Item().Row(row =>
                    {
                        row.ConstantItem(50);
                    });

                    if (model.Lines is not null) column.Item().Element(ComposeTable);

                    var totalAmount = model.Lines?.Sum(x => x.Amount);
                    column.Item().AlignRight().Text($"Total: {totalAmount}€").FontSize(14);
                });
            }

            void ComposeTable(IContainer container)
            {
                container.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1);
                        columns.RelativeColumn();
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Heure");
                        header.Cell().Element(CellStyle).Text("Prestation");
                        header.Cell().Element(CellStyle).AlignRight().Text("Durée");
                        header.Cell().Element(CellStyle).AlignRight().Text("Total");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    foreach (var item in model.Lines.OrderBy(item => item.From))
                    {
                        table.Cell().Element(CellStyle).AlignLeft().Text($"{item.From:d}");
                        table.Cell().Element(CellStyle).AlignLeft().Text(item.Title);
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.Duration}h");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.Amount}€");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3);
                        }
                    }
                });
            }

            return Document.Create(container =>
            {

                container
                    .Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(50);
                        page.DefaultTextStyle(TextStyle.Default.FontSize(10));

                        page.Header().Element(ComposeHeader);
                        page.Content().Element(ComposeContent);
                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                    });
            });
        }

    }
}
