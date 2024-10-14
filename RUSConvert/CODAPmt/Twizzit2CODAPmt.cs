using RUSConvert.Models;
using System.Xml.Linq;

namespace RUSConvert.CODAPmt
{
    internal class Twizzit2CODAPmt
    {
        public static Result Convert(string fileName, DateTime date, string communication)
        {
            var sourceLines = DataService.GetData(fileName);
            if (!sourceLines.Succeeded)
            {
                return Result<List<PaymentSource>>.Fail(sourceLines.Messages);
            }

            var countMissingIBAN =
                (from sourceline in sourceLines.Data
                 where sourceline.IBAN is null
                 select sourceline).Count();
            if (countMissingIBAN > 0)
            {
                return Result<List<PaymentSource>>.Fail($"{countMissingIBAN} IBAN manquants, conversion annulée");
            }

            var payments =
                from sourceline in sourceLines.Data
                where !sourceline.IBAN?.StartsWith("COMPENSATION") ?? false
                group sourceline by sourceline.TwizzitId into h
                select new PaymentLine
                {
                    TwizzitId = h.First().TwizzitId,
                    Name = h.First().Name ?? "",
                    IBAN = h.First().IBAN ?? "",
                    Amount = h.Sum(l => l.Amount),
                };

            int count = 0;
            string envelopDate = string.Format("{0:s}", DateTime.Now);
            string pmtDate = date.AddDays(0).ToString("yyyy-MM-dd");
            string envelopRef = "UCCL/" + envelopDate;

            // XML
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            var xmlPayments = new XDocument(
                 new XElement(xsi + "Document",
                        new XAttribute("xmlns", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.03"),
                        new XAttribute(XNamespace.Xmlns + "xsi", xsi.ToString()),
                    new XElement("CstmrCdtTrfInitn",
                        new XElement("GrpHdr",
                            new XElement("MsgId", envelopRef),
                            new XElement("CreDtTm", envelopDate),
                            new XElement("NbOfTxs", payments.Count().ToString()),
                            new XElement("InitgPty",
                                new XElement("Nm", Properties.Settings.Default.CompanyName),
                                new XElement("Id",
                                    new XElement("OrgId",
                                        new XElement("Othr",
                                            new XElement("Id", Properties.Settings.Default.CompanyVAT),
                                            new XElement("Issr", "KBO-BCE")
                                            )
                                        )
                                    )
                                )
                            ),
                            from pmt in payments
                            select new XElement("PmtInf",
                                new XElement("PmtInfId", envelopRef + "/" + count.ToString("000")),
                                new XElement("PmtMtd", "TRF"),
                                new XElement("BtchBookg", "false"),
                                new XElement("PmtTpInf",
                                    new XElement("SvcLvl",
                                        new XElement("Cd", "SEPA")
                                        )
                                    ),
                                new XElement("ReqdExctnDt", pmtDate),
                                new XElement("Dbtr",
                                    new XElement("Nm", Properties.Settings.Default.CompanyName)
                                        ),
                                new XElement("DbtrAcct",
                                    new XElement("Id",
                                        new XElement("IBAN", Properties.Settings.Default.CompanyIBAN)
                                            )
                                        ),
                                new XElement("CdtTrfTxInf",
                                    new XElement("PmtId",
                                        new XElement("EndToEndId", envelopRef + "/" + count.ToString("000"))
                                        ),
                                    new XElement("Amt",
                                        new XElement("InstdAmt", new XAttribute("Ccy", "EUR"), pmt.Amount)
                                        ),
                                    new XElement("Cdtr",
                                        new XElement("Nm", pmt.Name)
                                        ),
                                    new XElement("CdtrAcct",
                                        new XElement("Id",
                                            new XElement("IBAN", pmt.IBAN)
                                            )
                                        ),
                                    new XElement("RmtInf",
                                        new XElement("Ustrd", communication)
                                        )

                                    // Structurée
                                    //new XElement("RmtInf",
                                    //    new XElement("Strd",
                                    //        new XElement("CdtrRefInf",
                                    //            new XElement("Tp",
                                    //                new XElement("CdOrPrtry",
                                    //                    new XElement("Cd", "SCOR")
                                    //                    ),
                                    //                new XElement("Issr", "BBA/ISO")
                                    //                ),
                                    //                new XElement("Ref", pmt.Communication)
                                    //            )
                                    //        )
                                    //    )
                                    )
                                )
                            )
                        )
                    );
            string fileNameCODA = Path.Combine(Properties.Settings.Default.PaymentsDestFolder, DateTime.Now.ToString("yyyy-MM-dd-hhmm") + ".xml");
            xmlPayments.Save(fileNameCODA);

            // PDF
            var document = new RegistrationReport();
            document.CreateDocuments(sourceLines.Data, pmtDate);

            return Result<List<PaymentSource>>.Success($"{payments.Count()} fichiers traités");
        }
    }
}
