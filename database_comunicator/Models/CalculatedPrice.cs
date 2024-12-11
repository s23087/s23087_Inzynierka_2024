namespace database_communicator.Models;

public partial class CalculatedPrice
{
    public int PurchasePriceId { get; set; }

    public DateTime UpdateDate { get; set; }

    public string CurrencyName { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual CurrencyValue CurrencyValue { get; set; } = null!;

    public virtual PurchasePrice PurchasePrice { get; set; } = null!;
}
