using RUSConvert.Models;
using RUSConvert.Shared;

namespace RUSConvert.CODAPmt
{
    internal class Twizzit2CODAPmt(IProgress<JobProgress> progress)
    {
        private readonly IProgress<JobProgress> progress = progress;

        public Task<IResult> Convert(string fileName, DateTime date, string communication)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return Result.FailAsync($"ATTENTION: veuillez choisir un fichier, conversion impossible");
            }
            var sourceLines = DataService.GetData(fileName);
            if (!sourceLines.Succeeded)
            {
                return Result.FailAsync(sourceLines.Messages);
            }

            var countMissingIBAN =
                (from sourceline in sourceLines.Data
                 where sourceline.IBAN is null
                 select sourceline).Count();
            if (countMissingIBAN > 0)
            {
                return Result.FailAsync($"{countMissingIBAN} IBAN manquants, conversion annulée");
            }

            var payments =
                from sourceline in sourceLines.Data
                where !sourceline.IBAN?.StartsWith("COMPENSATION") ?? false
                group sourceline by sourceline.TwizzitId into h
                select new PaymentLine
                {
                    TwizzitId = h.First().TwizzitId,
                    Name = h.First().Name ?? "",
                    IBAN = h.First().IBAN!.ToUpper().Replace(" ", string.Empty) ?? "",
                    Amount = h.Sum(l => l.Amount),
                };

            string envelopDate = string.Format("{0:s}", DateTime.Now);
            string pmtDate = date.AddDays(0).ToString("yyyy-MM-dd");
            string envelopRef = "UCCL/" + envelopDate;

            // XML Bank
            var xmlPayments = Pain_001_001_03.GetXML(envelopRef, envelopDate, pmtDate, communication, payments);
            Directory.CreateDirectory(Properties.Settings.Default.PaymentsDestFolder);
            string fileNameCODA = Path.Combine(Properties.Settings.Default.PaymentsDestFolder, Path.GetFileNameWithoutExtension(fileName) + ".xml");
            xmlPayments.Save(fileNameCODA);

            // PDF
            var document = new RegistrationReport(progress);
            document.CreateDocuments(sourceLines.Data, pmtDate);

            // UBL
            var ubl = new RegistrationUBL(progress);
            ubl.CreateDocuments(date, pmtDate, communication, payments);

            return Result.SuccessAsync($"{payments.Count()} fichiers traités");
        }
    }
}