namespace database_communicator.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public int Seller { get; set; }

    public int Buyer { get; set; }

    public DateTime InvoiceDate { get; set; }

    public DateTime DueDate { get; set; }

    public string Note { get; set; } = null!;

    public bool InSystem { get; set; }

    public decimal TransportCost { get; set; }

    public string? InvoiceFilePath { get; set; }

    public int Taxes { get; set; }

    public DateTime CurrencyValueDate { get; set; }

    public string CurrencyName { get; set; } = null!;

    public int PaymentMethodId { get; set; }

    public int PaymentsStatusId { get; set; }

    public virtual Organization BuyerNavigation { get; set; } = null!;

    public virtual ICollection<CreditNote> CreditNotes { get; set; } = new List<CreditNote>();

    public virtual CurrencyValue Currency { get; set; } = null!;

    public virtual ICollection<OwnedItem> OwnedItems { get; set; } = new List<OwnedItem>();

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual PaymentStatus PaymentsStatus { get; set; } = null!;

    public virtual Organization SellerNavigation { get; set; } = null!;

    public virtual ICollection<SellingPrice> SellingPrices { get; set; } = new List<SellingPrice>();

    public virtual Taxis TaxesNavigation { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
