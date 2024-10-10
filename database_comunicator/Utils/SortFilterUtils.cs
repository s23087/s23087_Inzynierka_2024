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
