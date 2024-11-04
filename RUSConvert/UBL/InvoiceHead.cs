namespace RUSConvert.UBL
{
    internal class InvoiceHead
    {
        public string Number = string.Empty;
        public string Recipient = string.Empty;
        public string Recipient_Id = string.Empty;
        public DateOnly Creation_date;
        public DateOnly Due_date;
        public decimal Total_exc;
        public decimal Tax;
        public decimal Total_inc;
        public string? Communication_structurée;
    }
}