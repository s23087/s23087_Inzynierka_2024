using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using System;
using System.Linq.Expressions;

namespace database_comunicator.Utils
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
            else
            {
                var changedSort = sort[1..];
                orderBy = changedSort switch
                {
                    "Number" => e => e.ProformaNumber,
                    "Date" => e => e.ProformaDate,
                    "Recipient" => e => isYourProforma ? e.SellerNavigation.OrgName : e.BuyerNavigation.OrgName,
                    "Qty" => e => isYourProforma ?  e.ProformaFutureItems.Count : e.ProformaOwnedItems.Count,
                    "Total" => e => isYourProforma ? e.ProformaFutureItems.Select(d => d.PurchasePrice).Sum() : e.ProformaOwnedItems.Select(d => d.SellingPrice).Sum(),
                    _ => e => e.ProformaId,
                };
                return orderBy;
            }
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
