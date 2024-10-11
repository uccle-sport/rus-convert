using ExcelDataReader;
using RUSConvert.Models;
using System.Data;

namespace RUSConvert.UBL
{
    internal class DataService()
    {
        public static Result<List<InvoiceSource>> GetData(string FileName)
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
                return Result<List<InvoiceSource>>.Fail("Fichier inaccessible, probablement ouvert dans Excel");
            }
            catch
            {
                return Result<List<InvoiceSource>>.Fail("Fichier inaccessible");
            }

            DataTable? lines;
            lines = result?.Tables["Sheet1"] ?? null;
            if (lines is null)
            {
                return Result<List<InvoiceSource>>.Fail("Fichier vide");
            }
            else
            {
                List<InvoiceSource> sourceLines = [.. lines.AsEnumerable().Select(l => new InvoiceSource()
                {
                    Register = l.Field<string>("Register"),
                    Type = l.Field<string>("Type"),
                    Number = l.Field<string>("Number"),
                    Recipient = l.Field<string>("Recipient"),
                    Recipient_Id = l.Field<string>("Recipient Id"),
                    Address = l.Field<string>("Address"),
                    Header = l.Field<string>("Header"),
                    Sender_details = l.Field<string>("Sender details"),
                    Title = l.Field<string>("Title"),
                    Creation_date = DateOnly.TryParse(l.Field<string>("Creation date"), out var date) ? date : DateOnly.FromDateTime(DateTime.Now),
                    Due_date = DateOnly.TryParse(l.Field<string>("Due date"), out var duedate) ? duedate : DateOnly.FromDateTime(DateTime.Now),
                    Delivery_date = DateOnly.TryParse(l.Field<string>("Delivery date"), out var deliverydate) ? deliverydate : DateOnly.FromDateTime(DateTime.Now),
                    Status = l.Field<string>("Status"),
                    Étiquette = l.Field<string>("Étiquette"),
                    Total_exc = decimal.Parse((l.Field<string>("Total exc.") ?? "0").Replace(",", ".")),
                    Tax = decimal.Parse((l.Field<string>("Tax") ?? "0").Replace(",", ".")),
                    Total_inc = decimal.Parse((l.Field<string>("Total inc.") ?? "0").Replace(",", ".")),
                    Amount_paid = decimal.Parse((l.Field<string>("Amount paid") ?? "0").Replace(",", ".")),
                    Date_last_payment = l.Field<string>("Date last payment"),
                    ID_last_payment = l.Field<string>("ID last payment"),
                    Amount_credited = decimal.Parse((l.Field<string>("Amount credited") ?? "0").Replace(",", ".")),
                    Quantity = decimal.Parse((l.Field<string>("Quantity") ?? "0").Replace(",", ".")),
                    Description = l.Field<string>("Description") ?? "",
                    Amount = decimal.Parse((l.Field<string>("Amount") ?? "0").Replace(",", ".")),
                    TaxLine = decimal.Parse((l.Field<string>("Tax_1") ?? "0").Replace(",", ".")),
                    Total = decimal.Parse((l.Field<string>("Total") ?? "0").Replace(",", ".")),
                    Ledger_Account = l.Field<string>("Ledger Account"),
                    Référence = l.Field<string>("Référence"),
                    VCS = l.Field<string>("VCS"),
                })];

                foreach (var line in sourceLines)
                {
                    string[] parts = line.Description.Split(" - ");
                    if (parts.Length > 1) line.Description = parts[1];
                }

                return Result<List<InvoiceSource>>.Success(sourceLines);
            }
        }
       
    }
}
