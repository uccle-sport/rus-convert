namespace RUSConvert.CODAPmt
{
    public class RegistrationModel
    {
        public DateTime? IssueDate { get; set; }
        public string? Name { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? IBAN { get; set; }

        public List<PaymentSource>? Lines { get; set; }
    }
}