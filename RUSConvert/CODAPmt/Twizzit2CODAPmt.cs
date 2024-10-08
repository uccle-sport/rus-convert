﻿using RUSConvert.Models;
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

            var payments =
                from sourceline in sourceLines.Data
                group sourceline by sourceline.Numéro_de_compte into h
                select new PaymentLine
                {
                    Name = h.First().Nom ?? "",
                    IBAN = h.First().Numéro_de_compte ?? "",
                    Amount = h.Sum(l => l.Montant),
                };

            int count = 0;
            string envelopDate = string.Format("{0:s}", DateTime.Now);
            string pmtDate = date.AddDays(0).ToString("yyyy-MM-dd");
            string envelopRef = "UCCL/" + envelopDate;

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

            foreach (PaymentLine line in payments)
            {
                count++;
                // Add to file
            }
            string fileNameCODA = Path.Combine(Properties.Settings.Default.PaymentsDestFolder, DateTime.Now.ToString("yyyy-MM-dd-hhmm") + ".xml");
            xmlPayments.Save(fileNameCODA);
            return Result<List<PaymentSource>>.Success($"{count} fichiers traités");
        }
    }
}
