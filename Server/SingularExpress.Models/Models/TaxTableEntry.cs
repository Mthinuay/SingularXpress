namespace SingularExpress.Models.Models
{
    public class TaxTableEntry
    {
        public int Id { get; set; }
        public int TaxTableId { get; set; }
        public string Remuneration { get; set; } = string.Empty;
        public decimal AnnualEquivalent { get; set; }
        public decimal TaxUnder65 { get; set; }
    }
}