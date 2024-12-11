namespace database_communicator.Models;

public partial class CurrencyValue
{
    public string CurrencyName { get; set; } = null!;

    public DateTime UpdateDate { get; set; }

    public decimal CurrencyValue1 { get; set; }

    public virtual ICollection<CalculatedCreditNotePrice> CalculatedCreditNotePrices { get; set; } = new List<CalculatedCreditNotePrice>();

    public virtual ICollection<CalculatedPrice> CalculatedPrices { get; set; } = new List<CalculatedPrice>();

    public virtual CurrencyName CurrencyNameNavigation { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Proforma> Proformas { get; set; } = new List<Proforma>();
}
