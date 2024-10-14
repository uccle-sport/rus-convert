using RUSConvert.Models;

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

            string envelopDate = string.Format("{0:s}", DateTime.Now);
            string pmtDate = date.AddDays(0).ToString("yyyy-MM-dd");
            string envelopRef = "UCCL/" + envelopDate;

            // XML
            var xmlPayments = Pain_001_001_03.GetXML(envelopRef, envelopDate, pmtDate, communication, payments);
            Directory.CreateDirectory(Properties.Settings.Default.PaymentsDestFolder);
            string fileNameCODA = Path.Combine(Properties.Settings.Default.PaymentsDestFolder, DateTime.Now.ToString("yyyy-MM-dd-hhmm") + ".xml");
            xmlPayments.Save(fileNameCODA);

            // PDF
            var document = new RegistrationReport();
            document.CreateDocuments(sourceLines.Data, pmtDate);

            return Result<List<PaymentSource>>.Success($"{payments.Count()} fichiers traités");
        }
    }
}
