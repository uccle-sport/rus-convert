using System.Xml.Linq;

namespace RUSConvert.CODAPmt
{
    internal static class Pain_001_001_03
    {
        public static XDocument GetXML(string envelopRef, string envelopDate, string pmtDate, string communication, IEnumerable<PaymentLine> payments)
        {
            // XML
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace ns = "urn:iso:std:iso:20022:tech:xsd:pain.001.001.03";
            return new XDocument(
                 new XElement(ns + "Document",
                        new XAttribute(XNamespace.Xmlns + "xsi", xsi.ToString()),
                    new XElement(ns + "CstmrCdtTrfInitn",
                        new XElement(ns + "GrpHdr",
                            new XElement(ns + "MsgId", envelopRef),
                            new XElement(ns + "CreDtTm", envelopDate),
                            new XElement(ns + "NbOfTxs", payments.Count().ToString()),
                            new XElement(ns + "InitgPty",
                                new XElement(ns + "Nm", Properties.Settings.Default.CompanyName),
                                new XElement(ns + "Id",
                                    new XElement(ns + "OrgId",
                                        new XElement(ns + "Othr",
                                            new XElement(ns + "Id", Properties.Settings.Default.CompanyVAT),
                                            new XElement(ns + "Issr", "KBO-BCE")
                                            )
                                        )
                                    )
                                )
                            ),
                            new XElement(ns + "PmtInf",
                                new XElement(ns + "PmtInfId", envelopRef + "/" + payments.Count().ToString("000")),
                                new XElement(ns + "PmtMtd", "TRF"),
                                new XElement(ns + "BtchBookg", "false"),
                                new XElement(ns + "PmtTpInf",
                                    new XElement(ns + "SvcLvl",
                                        new XElement(ns + "Cd", "SEPA")
                                        )
                                    ),
                                new XElement(ns + "ReqdExctnDt", pmtDate),
                                new XElement(ns + "Dbtr",
                                    new XElement(ns + "Nm", Properties.Settings.Default.CompanyName)
                                        ),
                                new XElement(ns + "DbtrAcct",
                                    new XElement(ns + "Id",
                                        new XElement(ns + "IBAN", Properties.Settings.Default.CompanyIBAN)
                                            )
                                        ),
                                new XElement(ns + "DbtrAgt",
                                    new XElement(ns + "FinInstnId",
                                        new XElement(ns + "BIC", Properties.Settings.Default.CompanyBIC)
                                            )
                                        ),
                                from pmt in payments
                                select new XElement(ns + "CdtTrfTxInf",
                                            new XElement(ns + "PmtId",
                                                new XElement(ns + "EndToEndId", envelopRef + "/" + pmt.TwizzitId.ToString())
                                                ),
                                            new XElement(ns + "Amt",
                                                new XElement(ns + "InstdAmt", new XAttribute("Ccy", "EUR"), pmt.Amount)
                                                ),
                                            new XElement(ns + "Cdtr",
                                                new XElement(ns + "Nm", pmt.Name)
                                                ),
                                            new XElement(ns + "CdtrAcct",
                                                new XElement(ns + "Id",
                                                    new XElement(ns + "IBAN", pmt.IBAN)
                                                    )
                                                ),
                                            new XElement(ns + "RmtInf",
                                                new XElement(ns + "Ustrd", communication)
                                                )
                                    )
                                )
                            )
                        )
                    );
        }
    }
}