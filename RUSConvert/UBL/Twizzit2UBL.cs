﻿using RUSConvert.Models;
using RUSConvert.Shared;
using System.Xml.Linq;
using static RUSConvert.UBL.AccountService;

namespace RUSConvert.UBL
{
    internal class Twizzit2UBL(IProgress<JobProgress> progress)
    {
        private readonly IProgress<JobProgress> progress = progress;

        public Task<IResult> Convert(string fileNameXLSX)
        {
            if (string.IsNullOrEmpty(fileNameXLSX))
            {
                return Result.FailAsync($"ATTENTION: veuillez choisir un fichier, conversion impossible");
            }
            if (!File.Exists(fileNameXLSX))
            {
                return Result.FailAsync("ATTENTION: ce fichier n'existe pas, conversion impossible");
            }
            string archiveXLSX = Path.Combine(Path.GetDirectoryName(fileNameXLSX) ?? "", "ARCHIVES", Path.GetFileName(fileNameXLSX));
            if (File.Exists(archiveXLSX))
            {
                return Result.FailAsync("ATTENTION: ce fichier a déjà été importé, conversion impossible");
            }

            var rulesResult = GetRules();
            if (!rulesResult.Succeeded) return Result.FailAsync("ATTENTION: fichier Rules inaccessible, conversion impossible");
            List<AccountRules> rules = rulesResult.Data; 

            var sourceLines = DataService.GetData(fileNameXLSX);
            if (!sourceLines.Succeeded)
            {
                return Result.FailAsync(sourceLines.Messages);
            }

            var headers =
                from sourceline in sourceLines.Data
                group sourceline by sourceline.Number into h
                select new InvoiceHead
                {
                    Number = h.First().Number ?? "",
                    Recipient_Id = h.First().Recipient_Id?.ToUpper() ?? "",
                    Recipient = h.First().Recipient?.ToUpper() ?? "",
                    Creation_date = h.First().Creation_date,
                    Due_date = h.First().Due_date,
                    Total_exc = h.First().Total_exc,
                    Tax = h.First().Tax,
                    Total_inc = h.First().Total_inc,
                    Communication_structurée = h.First().Communication_structurée,
                };

            JobProgress jobProgress = new() { Value = 0, Min = 0, Max = headers.Count(), Text = "Conversion en cours" };
            progress.Report(jobProgress);

            int countOK = 0;
            int countIgnore = 0;
            XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
            XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";

            foreach (InvoiceHead header in headers)
            {
                var xmlInvoice = new XDocument(
                    new XElement(cac + "Invoice",
                            new XAttribute("xmlns", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2"),
                            new XAttribute(XNamespace.Xmlns + "cac", cac.ToString()),
                            new XAttribute(XNamespace.Xmlns + "cbc", cbc.ToString()),
                        new XElement(cbc + "ID", header.Number),
                        new XElement(cbc + "IssueDate", header.Creation_date.ToString("yyyy-MM-dd")),
                        new XElement(cbc + "InvoiceTypeCode", new XAttribute("listID", "UNCL1001"), "380"),
                        new XElement(cbc + "DocumentCurrencyCode", new XAttribute("listID", "ISO4217"), "EUR"),
                        new XElement(cac + "AccountingCustomerParty",
                            new XElement(cac + "Party",
                                new XElement(cac + "PartyIdentification",
                                    new XElement(cbc + "ID", header.Recipient_Id)
                                    ),
                                new XElement(cac + "PartyName",
                                    new XElement(cbc + "Name", header.Recipient + " (" + header.Recipient_Id + ")")
                                    )
                                )
                            ),

                        new XElement(cac + "PaymentMeans",
                            new XElement(cbc + "PaymentDueDate", header.Due_date.ToString("yyyy-MM-dd")),
                                new XElement(cbc + "InstructionID", CommStructService.Sanitize(header.Communication_structurée ?? ""))
                            ),

                        new XElement(cac + "TaxTotal",
                            new XElement(cbc + "TaxAmount", new XAttribute("currencyID", "EUR"), header.Tax),
                                new XElement(cac + "TaxSubtotal",
                                    new XElement(cbc + "TaxableAmount", new XAttribute("currencyID", "EUR"), header.Total_exc),
                                    new XElement(cbc + "TaxAmount", new XAttribute("currencyID", "EUR"), header.Tax),
                                    new XElement(cac + "TaxCategory",
                                        new XElement(cbc + "ID", new XAttribute("schemeID", "UNCL5305"), "Z"),
                                        new XElement(cbc + "Percent", "0"),
                                        new XElement(cac + "TaxScheme",
                                            new XElement(cbc + "ID", "VAT")
                                        )
                                    )
                                )
                            ),

                        new XElement(cac + "LegalMonetaryTotal",
                            new XElement(cbc + "LineExtensionAmount", new XAttribute("currencyID", "EUR"), header.Total_exc),
                            new XElement(cbc + "TaxExclusiveAmount", new XAttribute("currencyID", "EUR"), header.Total_exc),
                            new XElement(cbc + "TaxInclusiveAmount", new XAttribute("currencyID", "EUR"), header.Total_inc),
                            new XElement(cbc + "PayableAmount", new XAttribute("currencyID", "EUR"), header.Total_inc)
                            ),

                        from line in sourceLines.Data
                        where line.Number == header.Number
                        select new XElement(cac + "InvoiceLine",
                                new XElement(cbc + "LineExtensionAmount", new XAttribute("currencyID", "EUR"), line.Amount),
                                new XElement(cbc + "AccountingCost", AccountService.GetAccount(rules, line.Compte_général, line.Description, line.Total)),
                                new XElement(cac + "Item",
                                    new XElement(cbc + "Name", line.Description)
                                )
                            )
                        )
                    );
                Directory.CreateDirectory(Properties.Settings.Default.InvoicesDestFolder);
                string fileNameUBL = Path.Combine(Properties.Settings.Default.InvoicesDestFolder, header.Number + ".xml");
                string archiveFileNameUBL = Path.Combine(Properties.Settings.Default.InvoicesDestFolder, "ARCHIVES", header.Number + ".xml");
                if (File.Exists(archiveFileNameUBL))
                {
                    //Ignorer, facture déjà en compta, cas des paiements partiels
                    countIgnore++;
                }
                else
                {
                    xmlInvoice.Save(fileNameUBL);
                    countOK++;
                }
                jobProgress.Value++;
                progress.Report(jobProgress);
            };
            Directory.CreateDirectory(Path.GetDirectoryName(archiveXLSX) ?? "");
            File.Move(fileNameXLSX, archiveXLSX);
            return Result.SuccessAsync($"{countOK} fichiers traités, {countIgnore} fichiers ignorés");
        }
    }
}
