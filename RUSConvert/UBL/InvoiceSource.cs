namespace RUSConvert.UBL
{
    internal class InvoiceSource
    {
        public string? Register;
        public string? Type;
        public string? Number;
        public string? Recipient;
        public string? Recipient_Id;
        public string? Address;
        public string? Header;
        public string? Sender_details;
        public string? Title;
        public DateOnly Creation_date;
        public DateOnly Due_date;
        public DateOnly Delivery_date;
        public string? Status;
        public string? Étiquette;
        public decimal Total_exc;
        public decimal Tax;
        public decimal Total_inc;
        public decimal Amount_paid;
        public string? Date_last_payment;
        public string? ID_last_payment;
        public decimal Amount_credited;
        public string? Communication_structurée;
        public decimal Quantity;
        public string Description = string.Empty;
        public decimal Amount;
        public decimal TaxLine;
        public decimal Total;
        public string? Compte_général;
        public string? Référence;
    }
}