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
                    Nom = l.Field<string>("Nom"),
                    Numéro_de_compte = l.Field<string>("Numéro de compte"),
                    Montant = (decimal)l.Field<double>("Montant"),
                })];
                return Result<List<PaymentSource>>.Success(sourceLines);
            }
        }
    }
}
