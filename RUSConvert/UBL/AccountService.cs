using ExcelDataReader;
using RUSConvert.Models;
using System.Data;

namespace RUSConvert.UBL
{
    internal class AccountService
    {
        internal class AccountRules
        {
            public string Description = string.Empty;
            public decimal AmountMin = decimal.MinValue;
            public decimal AmountMax = decimal.MaxValue;
            public string Account = string.Empty;
        }

        public static string GetAccount(List<AccountRules> rules, string? account, string? description, decimal total)
        {
            if (!string.IsNullOrEmpty(account)) return account;
            if (string.IsNullOrEmpty(description)) return "700000";

            if (rules is not null)
            {
                foreach (var rule in rules)
                {
                    if ((description.Contains(rule.Description))
                        && ((total > rule.AmountMin) || rule.AmountMin == 0)
                        && ((total < rule.AmountMax) || rule.AmountMin == 0))
                        return rule.Account;
                }
            }
            return "700000";
        }
        public static Result<List<AccountRules>> GetRules()
        {
            if (!File.Exists("Files/Rules.xlsx")) return Result<List<AccountRules>>.Success([]);

            DataSet result;
            try
            {
                using var stream = File.Open("Files/Rules.xlsx", FileMode.Open, FileAccess.Read);
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
                return Result<List<AccountRules>>.Fail("Fichier inaccessible, probablement ouvert dans Excel");
            }
            catch
            {
                return Result<List<AccountRules>>.Fail("Fichier inaccessible");
            }
            DataTable? lines;
            lines = result?.Tables["Sheet1"] ?? null;
            if (lines is null)
            {
                return Result<List<AccountRules>>.Fail("Fichier vide");
            }
            else
            {
                List<AccountRules> sourceLines = [.. lines.AsEnumerable().Select(l => new AccountRules()
                {
                    Description = l.Field<string>("Description") ?? "",
                    AmountMin = (decimal)l.Field<double>("AmountMin"),
                    AmountMax =(decimal)l.Field<double>("AmountMax"),
                    Account = l.Field<string>("Account") ?? "",
                })];

                foreach (var line in sourceLines)
                {
                    string[] parts = line.Description.Split(" - ");
                    if (parts.Length > 1) line.Description = parts[1];
                }

                return Result<List<AccountRules>>.Success(sourceLines);
            }
        }
    }
}