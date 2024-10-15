using System;
using System.Collections.Generic;
using database_communicator.Models;

namespace database_communicator.Models;

public partial class Proforma
{
    public int ProformaId { get; set; }

    public string ProformaNumber { get; set; } = null!;

    public int Seller { get; set; }

    public int Buyer { get; set; }

    public DateTime ProformaDate { get; set; }

    public decimal TransportCost { get; set; }

    public string Note { get; set; } = null!;

    public bool InSystem { get; set; }

    public string? ProformaFilePath { get; set; }

    public int Taxes { get; set; }

    public int PaymentMethodId { get; set; }

    public DateTime CurrencyValueDate { get; set; }

    public string CurrencyName { get; set; } = null!;
    public int UserId { get; set; }
    public virtual AppUser User { get; set; } = null!;
    public virtual Organization BuyerNavigation { get; set; } = null!;

    public virtual CurrencyValue Currency { get; set; } = null!;

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual ICollection<ProformaFutureItem> ProformaFutureItems { get; set; } = new List<ProformaFutureItem>();

    public virtual ICollection<ProformaOwnedItem> ProformaOwnedItems { get; set; } = new List<ProformaOwnedItem>();

    public virtual Organization SellerNavigation { get; set; } = null!;

    public virtual Taxis TaxesNavigation { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
