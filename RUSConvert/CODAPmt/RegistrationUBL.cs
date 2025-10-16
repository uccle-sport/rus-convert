using RUSConvert.Shared;
using System.Xml.Linq;

namespace RUSConvert.CODAPmt
{
    internal class RegistrationUBL(IProgress<JobProgress> progress)
    {
        private readonly byte[]? LogoRUS;
        private readonly IProgress<JobProgress> progress = progress;

        public void CreateDocuments(DateTime envelopDate, string pmtDate, string communication, IEnumerable<PaymentLine> payments)
        {
            JobProgress jobProgress = new() { Value = 0, Min = 0, Max = payments.Count(), Text = "Conversion en cours" };

            int countOK = 0;

            XNamespace xNamespace = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
            XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
            XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";

            foreach (var header in payments)
            {
                var pdfFileName = Path.Combine(Properties.Settings.Default.PaymentsDestFolder, header.Name + " PRESTATIONS " + pmtDate + ".pdf");
                var pdfFileBytes = File.ReadAllBytes(pdfFileName);
                var pdfFile64 = Convert.ToBase64String(pdfFileBytes);

                XDocument xmlInvoice = new(
                    new XElement(xNamespace + "Invoice",
                            new XAttribute("xmlns", xNamespace.NamespaceName),
                            new XAttribute(XNamespace.Xmlns + "cac", cac.ToString()),
                            new XAttribute(XNamespace.Xmlns + "cbc", cbc.ToString()),
                        new XElement(cbc + "CustomizationID", "urn:cen.eu:en16931:2017#conformant#urn:UBL.BE:1.0.0.20180214"),
                        new XElement(cbc + "ProfileID", "urn:fdc:peppol.eu:2017:poacc:billing:01:1.0"),
                        new XElement(cbc + "ID", communication),
                        new XElement(cbc + "IssueDate", envelopDate.ToString("yyyy-MM-dd")),
                        new XElement(cbc + "DueDate", envelopDate.ToString("yyyy-MM-dd")),
                        new XElement(cbc + "InvoiceTypeCode", new XAttribute("listID", "UNCL1001"), "380"),
                        new XElement(cbc + "DocumentCurrencyCode", new XAttribute("listID", "ISO4217"), "EUR"),
                        new XElement(cac + "InvoicePeriod",
                            new XElement(cbc + "StartDate", new DateTime(envelopDate.Year, envelopDate.Month, 1).ToString("yyyy-MM-dd")),
                            new XElement(cbc + "EndDate", envelopDate.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"))
                        ),
                        new XElement(cac + "OrderReference",
                            new XElement(cbc + "ID", "NA")
                        ),
                        new XElement(cac + "AdditionalDocumentReference",
                            new XElement(cbc + "ID", "UBL.BE"),
                            new XElement(cbc + "DocumentDescription", "UBL.BE Compatible software Version 5.21"),
                            new XElement(cac + "Attachment",
                                new XElement(cbc + "EmbeddedDocumentBinaryObject",
                                    new XAttribute("filename", Path.GetFileName(pdfFileName)),
                                    new XAttribute("mimeCode", "application/pdf"), pdfFile64)
                            )
                        ),

                        new XElement(cac + "AccountingSupplierParty",
                            new XElement(cac + "Party",
                                new XElement(cac + "PartyIdentification",
                                    new XElement(cbc + "ID", header.TwizzitId)
                                    ),
                                new XElement(cac + "PartyName",
                                    new XElement(cbc + "Name", header.Name)
                                    ),
                                new XElement(cac + "PostalAddress",
                                    new XElement(cbc + "StreetName", ""),
                                    new XElement(cbc + "CityName", ""),
                                    new XElement(cbc + "PostalZone", ""),
                                    new XElement(cac + "Country",
                                        new XElement(cbc + "IdentificationCode", "BE")
                                        )
                                    )
                                /*new XElement(cac + "PartyTaxScheme",
                                     new XElement(cbc + "CompanyID", header.Recipient_Id),
                                    new XElement(cac + "TaxScheme",
                                        new XElement(cbc + "ID", "VAT")
                                        )
                                    ),
                                new XElement(cac + "PartyLegalEntity",
                                    new XElement(cbc + "RegistrationName", header.Recipient + " (" + header.Recipient_Id + ")")
                                    )*/
                                )
                            ),

                        /*new XElement(cac + "PaymentMeans",
                            new XElement(cbc + "PaymentMeansCode", "58"),
                            new XElement(cbc + "PaymentID", CommStructService.Sanitize(header.Communication_structurée ?? "")),
                            new XElement(cac + "PayeeFinancialAccount",
                                new XElement(cbc + "ID", Properties.Settings.Default.CompanyIBAN)
                                )
                            ),*/

                        new XElement(cac + "TaxTotal",
                            new XElement(cbc + "TaxAmount", new XAttribute("currencyID", "EUR"), 0),
                                new XElement(cac + "TaxSubtotal",
                                    new XElement(cbc + "TaxableAmount", new XAttribute("currencyID", "EUR"), header.Amount),
                                    new XElement(cbc + "TaxAmount", new XAttribute("currencyID", "EUR"), 0),
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
                            new XElement(cbc + "LineExtensionAmount", new XAttribute("currencyID", "EUR"), header.Amount),
                            new XElement(cbc + "TaxExclusiveAmount", new XAttribute("currencyID", "EUR"), header.Amount),
                            new XElement(cbc + "TaxInclusiveAmount", new XAttribute("currencyID", "EUR"), header.Amount),
                            new XElement(cbc + "PrepaidAmount", new XAttribute("currencyID", "EUR"), "0"),
                            new XElement(cbc + "PayableAmount", new XAttribute("currencyID", "EUR"), header.Amount)
                            ),

                        new XElement(cac + "InvoiceLine",
                            new XElement(cbc + "ID", 1),
                            new XElement(cbc + "InvoicedQuantity", new XAttribute("unitCode", "ZZ"), "1"),
                            new XElement(cbc + "LineExtensionAmount", new XAttribute("currencyID", "EUR"), header.Amount),
                            new XElement(cbc + "AccountingCost", ""),
                            new XElement(cac + "Item",
                                new XElement(cbc + "Description", "Prestations Hockey Jeunes"),
                                new XElement(cbc + "Name", "Prestations Hockey Jeunes"),
                                new XElement(cac + "ClassifiedTaxCategory",
                                    new XElement(cbc + "ID", new XAttribute("schemeID", "UNCL5305"), "Z"),
                                    new XElement(cbc + "Percent", "0"),
                                    new XElement(cac + "TaxScheme",
                                        new XElement(cbc + "ID", "VAT")
                                    )
                                )
                            ),
                            new XElement(cac + "Price",
                                new XElement(cbc + "PriceAmount", new XAttribute("currencyID", "EUR"), header.Amount),
                                new XElement(cbc + "BaseQuantity", "1")
                            )
                        )
                    )
                );
                Directory.CreateDirectory(Properties.Settings.Default.PaymentsDestFolder);
                string fileNameUBL = Path.Combine(Properties.Settings.Default.PaymentsDestFolder, envelopDate.ToString("yyyy-MM-dd") + " " + header.Name + ".xml");
                xmlInvoice.Save(fileNameUBL);
                countOK++;
                jobProgress.Value++;
                progress.Report(jobProgress);
            }
        }
    }
}