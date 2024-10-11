using ExcelDataReader;
using RUSConvert.Models;
using System.Data;

namespace RUSConvert.CODAPmt
{
    internal class DataService
    {
        public static Result<List<PaymentSource>> GetData(string FileName)
        {
            DataSet result;
            try
            {
                using var stream = File.Open(FileName, FileMode.Open, FileAccess.Read);
                using var reader = ExcelReaderFactory.CreateReader(stream);
                result = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
            }
            catch (IOException)
            {
                return Result<List<PaymentSource>>.Fail("Fichier inaccessible, probablement ouvert dans Excel");
            }

            DataTable? lines;
            lines = result?.Tables["Worksheet"] ?? null;
            if (lines is null)
            {
                return Result<List<PaymentSource>>.Fail("Fichier vide");
            }
            else
            {
                List<PaymentSource> sourceLines = [.. lines.AsEnumerable().Select(l => new PaymentSource()
                {
                    TwizzitId  = (decimal?)l.Field<double?>("Twizzit ID"),
                    Name = l.Field<string>("Nom"),
                    BirthDate =  DateOnly.ParseExact(l.Field<string>("Date de naissance") ?? DateOnly.FromDateTime(DateTime.Now).ToString(), "dd/MM/yyyy"),
                    IBAN = l.Field<string>("Numéro de compte"),
                    From =  DateTime.ParseExact(l.Field<string>("Heure de début") ?? DateTime.Now.ToString(), "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                    To =  DateTime.ParseExact(l.Field<string>("Heure de fin") ?? DateTime.Now.ToString(), "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                    Type = l.Field<string>("Type d'enregistrement"),
                    Title = l.Field<string>("Titre"),
                    Task = l.Field<string>("Tâche / fonction exécutée"),
                    Distance =  (decimal?)l.Field<double?>("Distance"),
                    Duration = l.Field<string>("Durée"),
                    Amount = (decimal)l.Field<double>("Montant"),
                    RegistrationDate =  DateOnly.ParseExact(l.Field<string>("Enregistré le") ?? DateOnly.FromDateTime(DateTime.Now).ToString(), "dd/MM/yyyy"),
                })];
                return Result<List<PaymentSource>>.Success(sourceLines);
            }
        }
    }
}
