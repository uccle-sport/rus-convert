using System.Xml.Linq;

namespace RUSConvert.CODAPmt
{
    internal class Pain_001_001_03
    {
        public static XDocument GetXML(string envelopRef, string envelopDate, string pmtDate, string communication, IEnumerable<PaymentLine> payments)
        {
            int count = 0;

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
                            new XElement("PmtInf",
                                new XElement("PmtInfId", envelopRef + "/" + count.ToString("000")),
                                new XElement("PmtMtd", "TRF"),
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
                                new XElement("DbtrAgt",
                                    new XElement("FinInstnId",
                                        new XElement("BIC", Properties.Settings.Default.CompanyBIC)
                                            )
                                        ),
                                from pmt in payments
                                select new XElement("CdtTrfTxInf",
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
                                    )
                                )
                            )
                        )
                    );
            return xmlPayments;
        }
    }
}
