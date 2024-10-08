﻿using RUSConvert.Models;
using RUSConvert.Shared;
using System.Xml.Linq;
using static RUSConvert.UBL.AccountService;

namespace RUSConvert.UBL
{
    internal class Twizzit2UBL
    {
        public static Result Convert(string fileName)
        {
            var rulesResult = GetRules();
            if (!rulesResult.Succeeded) return Result<List<InvoiceSource>>.Fail("ATTENTION: fichier Rules inaccessible, conversion impossible");
            List<AccountRules> rules = rulesResult.Data; 

            var sourceLines = DataService.GetData(fileName);
            if (!sourceLines.Succeeded)
            {
                return Result<List<InvoiceSource>>.Fail(sourceLines.Messages);
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
                    VCS = h.First().VCS,
                };

            int count = 0;
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
                                new XElement(cbc + "InstructionID", CommStructService.Sanitize(header.VCS ?? ""))
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
                                new XElement(cbc + "AccountingCost", AccountService.GetAccount(rules, line.Ledger_Account, line.Description, line.Total)),
                                new XElement(cac + "Item",
                                    new XElement(cbc + "Name", line.Description)
                                )
                            )
                        )
                    );
                string fileNameUBL = Path.Combine(Properties.Settings.Default.InvoicesDestFolder, header.Number + ".xml");
                string archiveFileNameUBL = Path.Combine(Properties.Settings.Default.InvoicesDestFolder, "ARCHIVES", header.Number + ".xml");
                if (File.Exists(archiveFileNameUBL))
                {
                    return Result<List<InvoiceSource>>.Fail("ATTENTION: vous traitez des fichiers déjà importés, conversion impossible");
                }
                else if (File.Exists(fileNameUBL))
                {
                    return Result<List<InvoiceSource>>.Fail("ATTENTION: vous traitez des fichiers déjà importés, conversion impossible");
                }
                else
                {
                    xmlInvoice.Save(fileNameUBL);
                    count++;
                }
            };
            return Result<List<InvoiceSource>>.Success($"{count} fichiers traités");
        }
    }
}