using database_communicator.Models;
using database_communicator.Models.DTOs;
using System;
using System.Linq.Expressions;

namespace database_communicator.Utils
{
    public static class SortFilterUtils
    {
        public static Func<GetManyItems, Object> GetItemSort(string? sort)
        {
            Func <GetManyItems, Object > orderBy;
            if (sort == null)
            {
                orderBy = e => e.Qty!;
                return orderBy;
            } else
            {
                var changedSort = sort[1..];
                orderBy = changedSort switch
                {
                    "Id" => e => e.ItemId!,
                    "Partnumber" => e => e.PartNumber!,
                    "Price" => e => e.PurchasePrice!,
                    "Name" => e => e.ItemName!,
                    _ => e => e.Qty!,
                };
                return orderBy;
            }
        }
        public static Expression<Func<Request, Object>> GetRequestSort(string? sort)
        {
            Expression<Func<Request, Object>> orderBy;
            if (sort == null)
            {
                orderBy = e => e.RequestId;
                return orderBy;
            }
            else
            {
                var changedSort = sort[1..];
                orderBy = changedSort switch
                {
                    "Title" => e => e.Title,
                    "Date" => e => e.CreationDate,
                    _ => e => e.RequestId,
                };
                return orderBy;
            }
        }
        public static Expression<Func<CreditNote, Object>> GetCreditNoteSort(string? sort)
        {
            Expression<Func<CreditNote, Object>> orderBy;
            if (sort == null)
            {
                orderBy = e => e.IdCreditNote;
                return orderBy;
            }
            else
            {
                var changedSort = sort[1..];
                orderBy = changedSort switch
                {
                    "Number" => e => e.CreditNoteNumber,
                    "Date" => e => e.CreditNoteDate,
                    "Qty" => e => e.CreditNoteItems.Any(d => d.Qty > 0) ? e.CreditNoteItems.Where(d => d.Qty > 0).Select(d => d.Qty).Sum() : e.CreditNoteItems.Sum(d => d.Qty),
                    "Total" => e => e.CreditNoteItems.Sum(d => d.NewPrice * d.Qty),
                    _ => e => e.InvoiceId,
                };
                return orderBy;
            }
        }
        public static Expression<Func<Invoice, Object>> GetInvoiceSort(string? sort, bool isYourInvoice)
        {
            Expression<Func<Invoice, Object>> orderBy;
            if (sort == null)
            {
                orderBy = e => e.InvoiceId;
                return orderBy;
            }
            else
            {
                var changedSort = sort[1..];
                orderBy = changedSort switch
                {
                    "Number" => e => e.InvoiceNumber,
                    "Date" => e => e.InvoiceDate,
                    "Qty" => e => isYourInvoice ? e.OwnedItems.SelectMany(d => d.PurchasePrices).Select(d => d.Qty).Sum() : e.SellingPrices.Select(d => d.Qty).Sum(),
                    "Total" => e => isYourInvoice ? e.OwnedItems.SelectMany(d => d.PurchasePrices).Select(d => d.Qty * d.Price).Sum() : e.SellingPrices.Select(d => d.Qty * d.Price).Sum(),
                    "Due" => e => e.DueDate,
                    _ => e => e.InvoiceId,
                };
                return orderBy;
            }
        }
        public static Expression<Func<OutsideItem, Object>> GetOutsideItemSort(string? sort)
        {
            Expression<Func<OutsideItem, Object>> orderBy;
            if (sort == null)
            {
                orderBy = e => e.ItemId;
                return orderBy;
            }
            else
            {
                var changedSort = sort[1..];
                orderBy = changedSort switch
                {
                    "Id" => e => e.ItemId,
                    "Source" => e => e.Organization.OrgName,
                    "Qty" => e => e.Qty,
                    "Price" => e => e.PurchasePrice,
                    "Partnumber" => e => e.Item.PartNumber,
                    _ => e => e.ItemId,
                };
                return orderBy;
            }
        }
        public static Expression<Func<Delivery, Object>> GetDeliverySort(string? sort, bool isDeliveryToUser)
        {
            Expression<Func<Delivery, Object>> orderBy;
            if (sort == null)
            {
                orderBy = e => e.DeliveryId;
                return orderBy;
            }
            else
            {
                var changedSort = sort[1..];
                orderBy = changedSort switch
                {
                    "Id" => e => e.DeliveryId,
                    "Estimated" => e => e.EstimatedDeliveryDate,
                    "Recipient" => e => isDeliveryToUser ? e.Proforma.SellerNavigation.OrgName : e.Proforma.BuyerNavigation.OrgName,
                    "Proforma_Number" => e => e.Proforma.ProformaNumber,
                    _ => e => e.DeliveryId,
                };
                return orderBy;
            }
        }
        public static Expression<Func<Organization, Object>> GetClientSort(string? sort)
        {
            Expression<Func<Organization, Object>> orderBy;
            if (sort == null)
            {
                orderBy = e => e.OrganizationId;
                return orderBy;
            }
            else
            {
                var changedSort = sort[1..];
                orderBy = changedSort switch
                {
                    "Name" => e => e.OrgName,
                    "Country" => e => e.Country.CountryName,
                    _ => e => e.OrganizationId,
                };
                return orderBy;
            }
        }
        public static Expression<Func<Proforma, Object>> GetProformaSort(string? sort, bool isYourProforma)
        {
            Expression<Func<Proforma, Object>> orderBy;
            if (sort == null)
            {
                orderBy = e => e.ProformaId;
                return orderBy;
            }
            var changedSort = sort[1..];
            orderBy = changedSort switch
            {
                "Number" => e => e.ProformaNumber,
                "Date" => e => e.ProformaDate,
                "Recipient" => e => isYourProforma ? e.SellerNavigation.OrgName : e.BuyerNavigation.OrgName,
                "Qty" => e => isYourProforma ? e.ProformaFutureItems.Count : e.ProformaOwnedItems.Count,
                "Total" => e => isYourProforma ? e.ProformaFutureItems.Select(d => d.PurchasePrice).Sum() : e.ProformaOwnedItems.Select(d => d.SellingPrice).Sum(),
                _ => e => e.ProformaId,
            };
            return orderBy;
        }
        public static Expression<Func<Offer, Object>> GetPricelistSort(string? sort)
        {
            Expression<Func<Offer, Object>> orderBy;
            if (sort == null)
            {
                orderBy = e => e.OfferId!;
                return orderBy;
            }
            else
            {
                var changedSort = sort[1..];
                orderBy = changedSort switch
                {
                    "Created" => e => e.CreationDate!,
                    "Name" => e => e.OfferName!,
                    "Modified" => e => e.ModificationDate!,
                    "Products" => e => e.OfferItems.Count!,
                    _ => e => e.OfferId!,
                };
                return orderBy;
            }
        }
        public static Func<GetManyItems, bool> GetFilterStatus(string? status)
        {
            Func<GetManyItems, bool> result;
            switch (status)
            {
                case "war":
                    result = e => e.StatusName == "In warehouse";
                    break;
                case "deli":
                    result = e => e.StatusName == "In delivery";
                    break;
                case "wardeli":
                    result = e => e.StatusName == "In warehouse | In delivery";
                    break;
                case "req":
                    result = e => e.StatusName == "On request";
                    break;
                case "unavail":
                    result = e => e.StatusName == "Unavailable";
                    break;
                default:
                    result = e => true; 
                    break;
            }
            return result;
        }
    }
}
