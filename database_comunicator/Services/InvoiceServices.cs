﻿using database_communicator.Data;
using database_communicator.Models;
using database_communicator.Models.DTOs;
using database_communicator.Utils;
using database_comunicator.FilterClass;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace database_communicator.Services
{
    public interface IInvoiceServices
    {
        public Task<GetOrgsForInvocie> GetOrgsForInvoice(int userId);
        public Task<IEnumerable<GetTaxes>> GetTaxes();
        public Task<IEnumerable<GetPaymentMethods>> GetPaymentMethods();
        public Task<IEnumerable<GetPaymentStatuses>> GetPaymentStatuses();
        public Task<int> AddPurchaseInvoice(AddPurchaseInvoice data);
        public Task<int> AddSalesInvoice(AddSalesInvoice data);
        public Task<IEnumerable<GetInvoices>> GetPurchaseInvoices(string? sort, InvoiceFiltersTemplate  filters);
        public Task<IEnumerable<GetInvoices>> GetPurchaseInvoices(string search, string? sort, InvoiceFiltersTemplate filters);
        public Task<IEnumerable<GetInvoices>> GetPurchaseInvoices(int userId, string? sort, InvoiceFiltersTemplate filters);
        public Task<IEnumerable<GetInvoices>> GetPurchaseInvoices(int userId, string search, string? sort, InvoiceFiltersTemplate filters);
        public Task<IEnumerable<GetInvoices>> GetSalesInvoices(string? sort, InvoiceFiltersTemplate filters);
        public Task<IEnumerable<GetInvoices>> GetSalesInvoices(string search, string? sort, InvoiceFiltersTemplate filters);
        public Task<IEnumerable<GetInvoices>> GetSalesInvoices(int userId, string? sort, InvoiceFiltersTemplate filters);
        public Task<IEnumerable<GetInvoices>> GetSalesInvoices(int userId, string search, string? sort, InvoiceFiltersTemplate filters);
        public Task<IEnumerable<GetInvoicesList>> GetPurchaseInvoicesList();
        public Task<IEnumerable<GetInvoicesList>> GetSalesInvoicesList();
        public Task<IEnumerable<GetInvoiceItems>> GetInvoiceItems(int invoiceId, bool isPurchaseInvoice);
        public Task<bool> CheckIfSellingPriceExist(int invoiceId);
        public Task<bool> CheckIfCreditNoteExist(int invoiceId);
        public Task<bool> DeleteInvoice(int invoiceId);
        public Task<bool> InvoiceExist(int invoiceId);
        public Task<IEnumerable<int>> GetInvoiceUser(int invoiceId);
        public Task<string> GetInvoiceNumber(int invoiceId);
        public Task<string?> GetInvoicePath(int invoiceId);
        public Task<GetRestInvoice> GetRestPurchaseInvoice(int invoiceId);
        public Task<GetRestInvoice> GetRestSalesInvoice(int invoiceId);
        public Task<GetRestModifyInvoice> GetRestModifyInvoice(int invoiceId);
        public Task<bool> ModifyInvoice(ModifyInvoice data);
        public Task UpdateInvoiceStatus();
    }
    public class InvoiceServices : IInvoiceServices
    {
        private readonly HandlerContext _handlerContext;
        public InvoiceServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }
        public async Task<GetOrgsForInvocie> GetOrgsForInvoice(int userId)
        {
            var userOrg = await _handlerContext.AppUsers
                .Where(e => e.IdUser == userId)
                .Select(e => new RestOrgs
                {
                    OrgId = e.SoloUserId != null ? e.SoloUser!.Organizations.OrganizationId : e.OrgUser!.Organizations.OrganizationId,
                    OrgName = e.SoloUserId != null ? e.SoloUser!.Organizations.OrgName : e.OrgUser!.Organizations.OrgName,
                }).FirstAsync();
            var result = await _handlerContext.Organizations
                .Where(e => !e.SoloUsers.Any() && !e.OrgUsers.Any())
                .Select(e => new RestOrgs
                {
                    OrgId = e.OrganizationId,
                    OrgName = e.OrgName
                }).ToListAsync();
            return new GetOrgsForInvocie
            {
                UserOrgId = userOrg.OrgId,
                OrgName = userOrg.OrgName,
                RestOrgs = result
            };
        }
        public async Task<IEnumerable<GetTaxes>> GetTaxes()
        {
            return await _handlerContext.Taxes.Select(e => new GetTaxes
            {
                TaxesId = e.TaxesId,
                TaxesValue = e.TaxValue
            }).ToListAsync();
        }
        public async Task<IEnumerable<GetPaymentMethods>> GetPaymentMethods()
        {
            return await _handlerContext.PaymentMethods.Select(e => new GetPaymentMethods
            {
                PaymentMethodId = e.PaymentMethodId,
                MethodName = e.MethodName,
            }).ToListAsync();
        }
        public async Task<IEnumerable<GetPaymentStatuses>> GetPaymentStatuses()
        {
            return await _handlerContext.PaymentStatuses.Select(e => new GetPaymentStatuses
            { 
                PaymentStatusId = e.PaymentStatusId,
                StatusName = e.StatusName,
            }).ToListAsync();
        }
        public async Task<int> AddPurchaseInvoice(AddPurchaseInvoice data)
        {
            var plnData = new DateTime(2024,9,3,0,0,0,DateTimeKind.Utc);
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var currVal = new List<CurrencyValue>()
                { 
                    new CurrencyValue
                    {
                        CurrencyName = "USD",
                        UpdateDate = data.CurrencyValueDate,
                        CurrencyValue1 = data.UsdValue
                    },
                    new CurrencyValue
                    {
                        CurrencyName = "EUR",
                        UpdateDate = data.CurrencyValueDate,
                        CurrencyValue1 = data.EuroValue
                    }
                };
                foreach (var val in currVal)
                {
                    var check = await _handlerContext.CurrencyValues
                        .Where(e => e.CurrencyName == val.CurrencyName && e.UpdateDate == val.UpdateDate)
                        .AnyAsync();
                    if (check)
                    {
                        _handlerContext.CurrencyValues.Update(val);
                    } else
                    {
                        _handlerContext.CurrencyValues.Add(val);
                    }
                }
                await _handlerContext.SaveChangesAsync();
                var newInvoice = new Invoice
                {
                    InvoiceNumber = data.InvoiceNumber,
                    Seller = data.Seller,
                    Buyer = data.Buyer,
                    InvoiceDate = data.InvoiceDate,
                    DueDate = data.DueDate,
                    Note = data.Note,
                    InSystem = data.InSystem,
                    TransportCost = data.TransportCost,
                    InvoiceFilePath = data.InvoiceFilePath,
                    Taxes = data.Taxes,
                    CurrencyValueDate = data.CurrencyName == "PLN" ? plnData : data.CurrencyValueDate,
                    CurrencyName = data.CurrencyName,
                    PaymentMethodId = data.PaymentMethodId,
                    PaymentsStatusId = data.PaymentsStatusId,
                };
                _handlerContext.Add<Invoice>(newInvoice);
                await _handlerContext.SaveChangesAsync();
                var invoiceId = newInvoice.InvoiceId;
                var ownedItems = data.InvoiceItems.GroupBy(e => e.ItemId).Select(e => new OwnedItem
                {
                    InvoiceId = invoiceId,
                    OwnedItemId = e.Key,
                }).ToList();
                _handlerContext.OwnedItems.AddRange(ownedItems);
                var purchasePrices = data.InvoiceItems.Select(e => new PurchasePrice
                {
                    OwnedItemId = e.ItemId,
                    InvoiceId = invoiceId,
                    Qty = e.Qty,
                    Price = data.CurrencyName == "PLN" ? e.Price : data.CurrencyName == "USD" ? e.Price * data.UsdValue : e.Price * data.EuroValue,
                }).ToArray();
                _handlerContext.PurchasePrices.AddRange(purchasePrices);
                await _handlerContext.SaveChangesAsync();
                var calculated = new List<CalculatedPrice>();
                foreach (var price in purchasePrices)
                {
                    calculated.Add(new CalculatedPrice
                    {
                        PurchasePriceId = price.PurchasePriceId,
                        UpdateDate = data.CurrencyValueDate,
                        CurrencyName = "USD",
                        Price = price.Price / data.UsdValue,
                    });
                    calculated.Add(new CalculatedPrice
                    {
                        PurchasePriceId = price.PurchasePriceId,
                        UpdateDate = data.CurrencyValueDate,
                        CurrencyName = "EUR",
                        Price = price.Price / data.EuroValue,
                    });
                }
                _handlerContext.CalculatedPrices.AddRange(calculated);
                await _handlerContext.SaveChangesAsync();
                var itemOwner = data.InvoiceItems.GroupBy(e => e.ItemId).Select(e => new { e.Key, Qty = e.Sum(d => d.Qty) })
                    .Select(e => new ItemOwner
                    {
                        IdUser = data.UserId,
                        InvoiceId = invoiceId,
                        OwnedItemId = e.Key,
                        Qty = e.Qty
                    });
                _handlerContext.ItemOwners.AddRange(itemOwner);
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return invoiceId;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
                return 0;
            }
        }
        public async Task<int> AddSalesInvoice(AddSalesInvoice data)
        {
            var plnData = new DateTime(2024, 9, 3, 0,0,0,DateTimeKind.Utc);
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var checkCurrency = await _handlerContext.CurrencyValues.Where(e => e.CurrencyName == data.CurrencyName && e.UpdateDate.Equals(data.CurrencyValueDate)).AnyAsync();
                var currVal = new CurrencyValue
                {
                    CurrencyName = data.CurrencyName,
                    UpdateDate = data.CurrencyValueDate,
                    CurrencyValue1 = data.CurrencyValue
                };
                if (checkCurrency)
                {
                    _handlerContext.CurrencyValues.Update(currVal);
                } else
                {
                    _handlerContext.CurrencyValues.Add(currVal);
                }
                await _handlerContext.SaveChangesAsync();
                var newInvoice = new Invoice
                {
                    InvoiceNumber = data.InvoiceNumber,
                    Seller = data.Seller,
                    Buyer = data.Buyer,
                    InvoiceDate = data.InvoiceDate,
                    DueDate = data.DueDate,
                    Note = data.Note,
                    InSystem = data.InSystem,
                    TransportCost = data.TransportCost,
                    InvoiceFilePath = data.InvoiceFilePath,
                    Taxes = data.Taxes,
                    CurrencyValueDate = data.CurrencyName == "PLN" ? plnData : data.CurrencyValueDate,
                    CurrencyName = data.CurrencyName,
                    PaymentMethodId = data.PaymentMethodId,
                    PaymentsStatusId = data.PaymentsStatusId,
                };
                _handlerContext.Add<Invoice>(newInvoice);
                await _handlerContext.SaveChangesAsync();
                var invoiceId = newInvoice.InvoiceId;
                var sellingPrice = data.InvoiceItems.Select(e => new SellingPrice
                {
                    PurchasePriceId = e.PriceId,
                    SellInvoiceId = invoiceId,
                    IdUser = data.UserId,
                    Qty = e.Qty,
                    Price = e.Price
                }).ToArray();
                _handlerContext.SellingPrices.AddRange(sellingPrice);
                await _handlerContext.SaveChangesAsync();

                foreach (var item in data.InvoiceItems)
                {
                    await _handlerContext.ItemOwners.Where(e => e.IdUser == data.UserId && e.InvoiceId == item.BuyInvoiceId && e.OwnedItemId == item.ItemId).ExecuteUpdateAsync(setters => 
                        setters.SetProperty(s => s.Qty, s => s.Qty - item.Qty)
                    );
                }
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return invoiceId;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
                return 0;
            }
        }

        public async Task<IEnumerable<GetInvoices>> GetPurchaseInvoices(string? sort, InvoiceFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetInvoiceSort(sort, true);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var dateLCond = InvoiceFilters.GetDateLowerFilter(filters.DateL);
            var dateGCond = InvoiceFilters.GetDateGreaterFilter(filters.DateG);
            var dueLCond = InvoiceFilters.GetDueLowerFilter(filters.DueL);
            var dueGCond = InvoiceFilters.GetDueGreaterFilter(filters.DueG);
            var qtyLCond = InvoiceFilters.GetQtyLowerFilter(filters.QtyL, true);
            var qtyGCond = InvoiceFilters.GetQtyGreaterFilter(filters.QtyG, true);
            var totalLCond = InvoiceFilters.GetTotalLowerFilter(filters.TotalL, true);
            var totalGCond = InvoiceFilters.GetTotalGreaterFilter(filters.TotalG, true);
            var recipientCond = InvoiceFilters.GetRecipientFilter(filters.Recipient, true);
            var currencyCond = InvoiceFilters.GetCurrencyFilter(filters.Currency);
            var paymentStatusCond = InvoiceFilters.GetPaymentStatusFilter(filters.PaymentStatus);
            var statusCond = InvoiceFilters.GetStatusFilter(filters.Status);

            return await _handlerContext.Invoices
                .Where(e => !e.SellingPrices.Any())
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(dueLCond)
                .Where(dueGCond)
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(recipientCond)
                .Where(currencyCond)
                .Where(paymentStatusCond)
                .Where(statusCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(inv => new GetInvoices
                {
                    Users = inv.OwnedItems.SelectMany(d => d.ItemOwners)
                        .Select(d => d.IdUserNavigation)
                        .GroupBy(d => new { d.IdUser, d.Username, d.Surname })
                        .Select(d => d.Key.Username + " " + d.Key.Surname).ToList(),
                    InvoiceId = inv.InvoiceId,
                    InvoiceNumber = inv.InvoiceNumber,
                    ClientName = inv.SellerNavigation.OrgName,
                    InvoiceDate = inv.InvoiceDate,
                    DueDate = inv.DueDate,
                    PaymentStatus = inv.PaymentsStatus.StatusName,
                    InSystem = inv.InSystem,
                    Qty = inv.OwnedItems.SelectMany(d => d.PurchasePrices).Select(d => d.Qty).Sum(),
                    Price = inv.OwnedItems.SelectMany(d => d.PurchasePrices).Select(d => d.Price * d.Qty).Sum()
                    + (inv.CurrencyName == "PLN" ? inv.TransportCost : inv.TransportCost * inv.Currency.CurrencyValue1),
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetInvoices>> GetPurchaseInvoices(string search, string? sort, InvoiceFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetInvoiceSort(sort, true);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var dateLCond = InvoiceFilters.GetDateLowerFilter(filters.DateL);
            var dateGCond = InvoiceFilters.GetDateGreaterFilter(filters.DateG);
            var dueLCond = InvoiceFilters.GetDueLowerFilter(filters.DueL);
            var dueGCond = InvoiceFilters.GetDueGreaterFilter(filters.DueG);
            var qtyLCond = InvoiceFilters.GetQtyLowerFilter(filters.QtyL, true);
            var qtyGCond = InvoiceFilters.GetQtyGreaterFilter(filters.QtyG, true);
            var totalLCond = InvoiceFilters.GetTotalLowerFilter(filters.TotalL, true);
            var totalGCond = InvoiceFilters.GetTotalGreaterFilter(filters.TotalG, true);
            var recipientCond = InvoiceFilters.GetRecipientFilter(filters.Recipient, true);
            var currencyCond = InvoiceFilters.GetCurrencyFilter(filters.Currency);
            var paymentStatusCond = InvoiceFilters.GetPaymentStatusFilter(filters.PaymentStatus);
            var statusCond = InvoiceFilters.GetStatusFilter(filters.Status);

            return await _handlerContext.Invoices
                .Where(e => e.InvoiceNumber.ToLower().Contains(search.ToLower()))
                .Where(e => !e.SellingPrices.Any())
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(dueLCond)
                .Where(dueGCond)
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(recipientCond)
                .Where(currencyCond)
                .Where(paymentStatusCond)
                .Where(statusCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(ent => new GetInvoices
                {
                    Users = ent.OwnedItems.SelectMany(d => d.ItemOwners)
                        .Select(d => d.IdUserNavigation)
                        .GroupBy(d => new { d.IdUser, d.Username, d.Surname })
                        .Select(d => d.Key.Username + " " + d.Key.Surname).ToList(),
                    InvoiceId = ent.InvoiceId,
                    InvoiceNumber = ent.InvoiceNumber,
                    ClientName = ent.SellerNavigation.OrgName,
                    InvoiceDate = ent.InvoiceDate,
                    DueDate = ent.DueDate,
                    PaymentStatus = ent.PaymentsStatus.StatusName,
                    InSystem = ent.InSystem,
                    Qty = ent.OwnedItems.SelectMany(d => d.PurchasePrices).Select(d => d.Qty).Sum(),
                    Price = ent.OwnedItems.SelectMany(d => d.PurchasePrices).Select(d => d.Price * d.Qty).Sum()
                    + (ent.CurrencyName == "PLN" ? ent.TransportCost : ent.TransportCost * ent.Currency.CurrencyValue1),
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetInvoices>> GetPurchaseInvoices(int userId, string? sort, InvoiceFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetInvoiceSort(sort, true);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var dateLCond = InvoiceFilters.GetDateLowerFilter(filters.DateL);
            var dateGCond = InvoiceFilters.GetDateGreaterFilter(filters.DateG);
            var dueLCond = InvoiceFilters.GetDueLowerFilter(filters.DueL);
            var dueGCond = InvoiceFilters.GetDueGreaterFilter(filters.DueG);
            var qtyLCond = InvoiceFilters.GetQtyLowerFilter(filters.QtyL, true);
            var qtyGCond = InvoiceFilters.GetQtyGreaterFilter(filters.QtyG, true);
            var totalLCond = InvoiceFilters.GetTotalLowerFilter(filters.TotalL, true);
            var totalGCond = InvoiceFilters.GetTotalGreaterFilter(filters.TotalG, true);
            var recipientCond = InvoiceFilters.GetRecipientFilter(filters.Recipient, true);
            var currencyCond = InvoiceFilters.GetCurrencyFilter(filters.Currency);
            var paymentStatusCond = InvoiceFilters.GetPaymentStatusFilter(filters.PaymentStatus);
            var statusCond = InvoiceFilters.GetStatusFilter(filters.Status);

            return await _handlerContext.Invoices
                .Where(e => !e.SellingPrices.Any() && e.OwnedItems.SelectMany(d => d.ItemOwners).Any(d => d.IdUser == userId))
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(dueLCond)
                .Where(dueGCond)
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(recipientCond)
                .Where(currencyCond)
                .Where(paymentStatusCond)
                .Where(statusCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(instc => new GetInvoices
                {
                    InvoiceId = instc.InvoiceId,
                    InvoiceNumber = instc.InvoiceNumber,
                    ClientName = instc.SellerNavigation.OrgName,
                    InvoiceDate = instc.InvoiceDate,
                    DueDate = instc.DueDate,
                    PaymentStatus = instc.PaymentsStatus.StatusName,
                    InSystem = instc.InSystem,
                    Qty = instc.OwnedItems.SelectMany(d => d.PurchasePrices).Select(d => d.Qty).Sum(),
                    Price = instc.OwnedItems.SelectMany(d => d.PurchasePrices).Select(d => d.Price * d.Qty).Sum()
                    + (instc.CurrencyName == "PLN" ? instc.TransportCost : instc.TransportCost * instc.Currency.CurrencyValue1),
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetInvoices>> GetPurchaseInvoices(int userId, string search, string? sort, InvoiceFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetInvoiceSort(sort, true);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var dateLCond = InvoiceFilters.GetDateLowerFilter(filters.DateL);
            var dateGCond = InvoiceFilters.GetDateGreaterFilter(filters.DateG);
            var dueLCond = InvoiceFilters.GetDueLowerFilter(filters.DueL);
            var dueGCond = InvoiceFilters.GetDueGreaterFilter(filters.DueG);
            var qtyLCond = InvoiceFilters.GetQtyLowerFilter(filters.QtyL, true);
            var qtyGCond = InvoiceFilters.GetQtyGreaterFilter(filters.QtyG, true);
            var totalLCond = InvoiceFilters.GetTotalLowerFilter(filters.TotalL, true);
            var totalGCond = InvoiceFilters.GetTotalGreaterFilter(filters.TotalG, true);
            var recipientCond = InvoiceFilters.GetRecipientFilter(filters.Recipient, true);
            var currencyCond = InvoiceFilters.GetCurrencyFilter(filters.Currency);
            var paymentStatusCond = InvoiceFilters.GetPaymentStatusFilter(filters.PaymentStatus);
            var statusCond = InvoiceFilters.GetStatusFilter(filters.Status);

            return await _handlerContext.Invoices
                .Where(e => !e.SellingPrices.Any() && e.OwnedItems.SelectMany(d => d.ItemOwners).Any(d => d.IdUser == userId))
                .Where(e => e.InvoiceNumber.ToLower().Contains(search.ToLower()))
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(dueLCond)
                .Where(dueGCond)
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(recipientCond)
                .Where(currencyCond)
                .Where(paymentStatusCond)
                .Where(statusCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(objs => new GetInvoices
                {
                    InvoiceId = objs.InvoiceId,
                    InvoiceNumber = objs.InvoiceNumber,
                    ClientName = objs.SellerNavigation.OrgName,
                    InvoiceDate = objs.InvoiceDate,
                    DueDate = objs.DueDate,
                    PaymentStatus = objs.PaymentsStatus.StatusName,
                    InSystem = objs.InSystem,
                    Qty = objs.OwnedItems.SelectMany(d => d.PurchasePrices).Select(d => d.Qty).Sum(),
                    Price = objs.OwnedItems.SelectMany(d => d.PurchasePrices).Select(d => d.Price * d.Qty).Sum()
                    + (objs.CurrencyName == "PLN" ? objs.TransportCost : objs.TransportCost * objs.Currency.CurrencyValue1),
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetInvoices>> GetSalesInvoices(string? sort, InvoiceFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetInvoiceSort(sort, false);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var dateLCond = InvoiceFilters.GetDateLowerFilter(filters.DateL);
            var dateGCond = InvoiceFilters.GetDateGreaterFilter(filters.DateG);
            var dueLCond = InvoiceFilters.GetDueLowerFilter(filters.DueL);
            var dueGCond = InvoiceFilters.GetDueGreaterFilter(filters.DueG);
            var qtyLCond = InvoiceFilters.GetQtyLowerFilter(filters.QtyL, false);
            var qtyGCond = InvoiceFilters.GetQtyGreaterFilter(filters.QtyG, false);
            var totalLCond = InvoiceFilters.GetTotalLowerFilter(filters.TotalL, false);
            var totalGCond = InvoiceFilters.GetTotalGreaterFilter(filters.TotalG, false);
            var recipientCond = InvoiceFilters.GetRecipientFilter(filters.Recipient, false);
            var currencyCond = InvoiceFilters.GetCurrencyFilter(filters.Currency);
            var paymentStatusCond = InvoiceFilters.GetPaymentStatusFilter(filters.PaymentStatus);
            var statusCond = InvoiceFilters.GetStatusFilter(filters.Status);

            var result = await _handlerContext.Invoices
                .Where(e => e.SellingPrices.Any())
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(dueLCond)
                .Where(dueGCond)
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(recipientCond)
                .Where(currencyCond)
                .Where(paymentStatusCond)
                .Where(statusCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(obj => new GetInvoices
                {
                    Users = obj.SellingPrices.Select(e => e.User).GroupBy(e => new {e.IdUser, e.Username, e.Surname}).Select(e => e.Key.Username + " " + e.Key.Surname).ToList(),
                    InvoiceId = obj.InvoiceId,
                    InvoiceNumber = obj.InvoiceNumber,
                    ClientName = obj.BuyerNavigation.OrgName,
                    InvoiceDate = obj.InvoiceDate,
                    DueDate = obj.DueDate,
                    PaymentStatus = obj.PaymentsStatus.StatusName,
                    InSystem = obj.InSystem,
                    Qty = obj.SellingPrices.Select(d => d.Qty).Sum(),
                    Price = obj.SellingPrices.Select(d => d.Price * d.Qty).Sum() + obj.TransportCost,
                }).ToListAsync();
            return result;
        }
        public async Task<IEnumerable<GetInvoices>> GetSalesInvoices(string search, string? sort, InvoiceFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetInvoiceSort(sort, false);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var dateLCond = InvoiceFilters.GetDateLowerFilter(filters.DateL);
            var dateGCond = InvoiceFilters.GetDateGreaterFilter(filters.DateG);
            var dueLCond = InvoiceFilters.GetDueLowerFilter(filters.DueL);
            var dueGCond = InvoiceFilters.GetDueGreaterFilter(filters.DueG);
            var qtyLCond = InvoiceFilters.GetQtyLowerFilter(filters.QtyL, false);
            var qtyGCond = InvoiceFilters.GetQtyGreaterFilter(filters.QtyG, false);
            var totalLCond = InvoiceFilters.GetTotalLowerFilter(filters.TotalL, false);
            var totalGCond = InvoiceFilters.GetTotalGreaterFilter(filters.TotalG, false);
            var recipientCond = InvoiceFilters.GetRecipientFilter(filters.Recipient, false);
            var currencyCond = InvoiceFilters.GetCurrencyFilter(filters.Currency);
            var paymentStatusCond = InvoiceFilters.GetPaymentStatusFilter(filters.PaymentStatus);
            var statusCond = InvoiceFilters.GetStatusFilter(filters.Status);

            return await _handlerContext.Invoices
                .Where(e => e.InvoiceNumber.ToLower().Contains(search.ToLower()))
                .Where(e => e.SellingPrices.Any())
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(dueLCond)
                .Where(dueGCond)
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(recipientCond)
                .Where(currencyCond)
                .Where(paymentStatusCond)
                .Where(statusCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(inst => new GetInvoices
                {
                    Users = inst.SellingPrices.Select(e => e.User).GroupBy(e => new { e.IdUser, e.Username, e.Surname }).Select(e => e.Key.Username + " " + e.Key.Surname).ToList(),
                    InvoiceId = inst.InvoiceId,
                    InvoiceNumber = inst.InvoiceNumber,
                    ClientName = inst.BuyerNavigation.OrgName,
                    InvoiceDate = inst.InvoiceDate,
                    DueDate = inst.DueDate,
                    PaymentStatus = inst.PaymentsStatus.StatusName,
                    InSystem = inst.InSystem,
                    Qty = inst.SellingPrices.Select(d => d.Qty).Sum(),
                    Price = inst.SellingPrices.Select(d => d.Price * d.Qty).Sum() + inst.TransportCost,
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetInvoices>> GetSalesInvoices(int userId, string? sort, InvoiceFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetInvoiceSort(sort, false);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var dateLCond = InvoiceFilters.GetDateLowerFilter(filters.DateL);
            var dateGCond = InvoiceFilters.GetDateGreaterFilter(filters.DateG);
            var dueLCond = InvoiceFilters.GetDueLowerFilter(filters.DueL);
            var dueGCond = InvoiceFilters.GetDueGreaterFilter(filters.DueG);
            var qtyLCond = InvoiceFilters.GetQtyLowerFilter(filters.QtyL, false);
            var qtyGCond = InvoiceFilters.GetQtyGreaterFilter(filters.QtyG, false);
            var totalLCond = InvoiceFilters.GetTotalLowerFilter(filters.TotalL, false);
            var totalGCond = InvoiceFilters.GetTotalGreaterFilter(filters.TotalG, false);
            var recipientCond = InvoiceFilters.GetRecipientFilter(filters.Recipient, false);
            var currencyCond = InvoiceFilters.GetCurrencyFilter(filters.Currency);
            var paymentStatusCond = InvoiceFilters.GetPaymentStatusFilter(filters.PaymentStatus);
            var statusCond = InvoiceFilters.GetStatusFilter(filters.Status);

            return await _handlerContext.Invoices
                .Where(e => e.SellingPrices.Any())
                .Where(e => e.SellingPrices.Select(e => e.PurchasePrice).Select(d => d.OwnedItem).SelectMany(d => d.ItemOwners).Any(d => d.IdUser == userId))
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(dueLCond)
                .Where(dueGCond)
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(recipientCond)
                .Where(currencyCond)
                .Where(paymentStatusCond)
                .Where(statusCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(entity => new GetInvoices
                {
                    InvoiceId = entity.InvoiceId,
                    InvoiceNumber = entity.InvoiceNumber,
                    ClientName = entity.BuyerNavigation.OrgName,
                    InvoiceDate = entity.InvoiceDate,
                    DueDate = entity.DueDate,
                    PaymentStatus = entity.PaymentsStatus.StatusName,
                    InSystem = entity.InSystem,
                    Qty = entity.SellingPrices.Select(d => d.Qty).Sum(),
                    Price = entity.SellingPrices.Select(d => d.Price * d.Qty).Sum() + entity.TransportCost,
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetInvoices>> GetSalesInvoices(int userId, string search, string? sort, InvoiceFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetInvoiceSort(sort, false);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var dateLCond = InvoiceFilters.GetDateLowerFilter(filters.DateL);
            var dateGCond = InvoiceFilters.GetDateGreaterFilter(filters.DateG);
            var dueLCond = InvoiceFilters.GetDueLowerFilter(filters.DueL);
            var dueGCond = InvoiceFilters.GetDueGreaterFilter(filters.DueG);
            var qtyLCond = InvoiceFilters.GetQtyLowerFilter(filters.QtyL, false);
            var qtyGCond = InvoiceFilters.GetQtyGreaterFilter(filters.QtyG, false);
            var totalLCond = InvoiceFilters.GetTotalLowerFilter(filters.TotalL, false);
            var totalGCond = InvoiceFilters.GetTotalGreaterFilter(filters.TotalG, false);
            var recipientCond = InvoiceFilters.GetRecipientFilter(filters.Recipient, false);
            var currencyCond = InvoiceFilters.GetCurrencyFilter(filters.Currency);
            var paymentStatusCond = InvoiceFilters.GetPaymentStatusFilter(filters.PaymentStatus);
            var statusCond = InvoiceFilters.GetStatusFilter(filters.Status);

            return await _handlerContext.Invoices
                .Where(e => e.InvoiceNumber.ToLower().Contains(search.ToLower()))
                .Where(e => e.SellingPrices.Any())
                .Where(e => e.SellingPrices.Select(e => e.PurchasePrice).Select(d => d.OwnedItem).SelectMany(d => d.ItemOwners).Any(d => d.IdUser == userId))
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(dueLCond)
                .Where(dueGCond)
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(recipientCond)
                .Where(currencyCond)
                .Where(paymentStatusCond)
                .Where(statusCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(en => new GetInvoices
                {
                    InvoiceId = en.InvoiceId,
                    InvoiceNumber = en.InvoiceNumber,
                    ClientName = en.BuyerNavigation.OrgName,
                    InvoiceDate = en.InvoiceDate,
                    DueDate = en.DueDate,
                    PaymentStatus = en.PaymentsStatus.StatusName,
                    InSystem = en.InSystem,
                    Qty = en.SellingPrices.Select(d => d.Qty).Sum(),
                    Price = en.SellingPrices.Select(d => d.Price * d.Qty).Sum() + en.TransportCost,
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetInvoicesList>> GetPurchaseInvoicesList()
        {
            return await _handlerContext.Invoices
                .Include(e => e.OwnedItems)
                .Where(e => e.OwnedItems.SelectMany(d => d.PurchasePrices).Any())
                .Select(e => new GetInvoicesList
                {
                    InvoiceId = e.InvoiceId,
                    InvoiceNumber = e.InvoiceNumber,
                    ClientName = e.SellerNavigation.OrgName,
                    OrgName = e.BuyerNavigation.OrgName
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetInvoicesList>> GetSalesInvoicesList()
        {
            return await _handlerContext.Invoices
                .Where(e => e.SellingPrices.Any())
                .Select(e => new GetInvoicesList
                {
                    InvoiceId = e.InvoiceId,
                    InvoiceNumber = e.InvoiceNumber,
                    ClientName = e.BuyerNavigation.OrgName,
                    OrgName = e.SellerNavigation.OrgName
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetInvoiceItems>> GetInvoiceItems(int invoiceId, bool isPurchaseInvoice)
        {
            var invoiceCurrency = await _handlerContext.Invoices.Where(e => e.InvoiceId == invoiceId).Select(e => e.CurrencyName).FirstAsync();
            if (isPurchaseInvoice)
            {
                if (invoiceCurrency == "PLN")
                {
                    return await _handlerContext.OwnedItems
                        .Where(e => e.InvoiceId == invoiceId)
                        .SelectMany(e => e.PurchasePrices)
                        .Select(e => new GetInvoiceItems
                        {
                            PriceId = e.PurchasePriceId,
                            InvoiceId = e.InvoiceId,
                            ItemId = e.OwnedItemId,
                            ItemName = e.OwnedItem.OriginalItem.ItemName,
                            Partnumber = e.OwnedItem.OriginalItem.PartNumber,
                            Qty = e.Qty,
                            Price = e.Price
                        }).ToListAsync();
                } else
                {
                    return await _handlerContext.OwnedItems
                        .Where(e => e.InvoiceId == invoiceId)
                        .SelectMany(e => e.PurchasePrices)
                        .Include(e => e.CalculatedPrices)
                        .Select(e => new GetInvoiceItems
                        {
                            PriceId = e.PurchasePriceId,
                            InvoiceId = e.InvoiceId,
                            ItemId = e.OwnedItemId,
                            ItemName = e.OwnedItem.OriginalItem.ItemName,
                            Partnumber = e.OwnedItem.OriginalItem.PartNumber,
                            Qty = e.Qty,
                            Price = e.CalculatedPrices.Where(d => d.CurrencyName == invoiceCurrency).Select(d => d.Price).First()
                        }).ToListAsync();
                }
            }

            return await _handlerContext.SellingPrices
                .Where(e => e.SellInvoiceId == invoiceId)
                .Select(e => new GetInvoiceItems
                {
                    PriceId = e.PurchasePriceId,
                    InvoiceId = e.PurchasePrice.InvoiceId,
                    ItemId = e.PurchasePrice.OwnedItemId,
                    ItemName = e.PurchasePrice.OwnedItem.OriginalItem.ItemName,
                    Partnumber = e.PurchasePrice.OwnedItem.OriginalItem.PartNumber,
                    Qty = e.Qty - e.PurchasePrice.CreditNoteItems.Where(d => d.CreditNote.Invoice.SellingPrices.Any()).Select(d => d.Qty).Sum(),
                    Price = e.Price
                }).Union(
                    _handlerContext.CreditNoteItems
                    .Where(e => e.CreditNote.InvoiceId == invoiceId)
                    .Select(e => new GetInvoiceItems
                    {
                        PriceId = e.PurchasePriceId,
                        InvoiceId = e.PurchasePrice.InvoiceId,
                        ItemId = e.PurchasePrice.OwnedItemId,
                        ItemName = e.PurchasePrice.OwnedItem.OriginalItem.ItemName,
                        Partnumber = e.PurchasePrice.OwnedItem.OriginalItem.PartNumber,
                        Qty = e.Qty,
                        Price = invoiceCurrency == "PLN" ? e.NewPrice : e.CalculatedCreditNotePrices.Where(d => d.CurrencyName == invoiceCurrency).Select(d => d.Price).First()
                    })
                ).ToListAsync();
        }
        public async Task<bool> CheckIfSellingPriceExist(int invoiceId)
        {
            return await _handlerContext.SellingPrices.Where(e => e.PurchasePrice.InvoiceId == invoiceId).AnyAsync();
        }
        public async Task<bool> CheckIfCreditNoteExist(int invoiceId)
        {
            return await _handlerContext.CreditNotes.Where(e => e.InvoiceId == invoiceId).AnyAsync();
        }
        public async Task<bool> DeleteInvoice(int invoiceId)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var sellingPrices = await _handlerContext.SellingPrices.Where(e => e.SellInvoiceId == invoiceId).Select(e => new
                {
                    e.Qty, e.IdUser, e.PurchasePrice.InvoiceId, e.PurchasePrice.OwnedItemId

                }).ToListAsync();
                foreach (var price in sellingPrices)
                {
                    await _handlerContext.ItemOwners.Where(e => e.InvoiceId == price.InvoiceId && e.IdUser == price.IdUser && e.OwnedItemId == price.OwnedItemId)
                        .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.Qty, s => s.Qty + price.Qty));
                }
                await _handlerContext.PurchasePrices.Where(e => e.InvoiceId == invoiceId).SelectMany(e => e.CalculatedPrices).ExecuteDeleteAsync();
                await _handlerContext.PurchasePrices.Where(e => e.InvoiceId == invoiceId).ExecuteDeleteAsync();
                await _handlerContext.ItemOwners.Where(e => e.InvoiceId == invoiceId).ExecuteDeleteAsync();
                await _handlerContext.OwnedItems.Where(e => e.InvoiceId == invoiceId).ExecuteDeleteAsync();
                await _handlerContext.SellingPrices.Where(e => e.SellInvoiceId == invoiceId).ExecuteDeleteAsync();
                await _handlerContext.Invoices.Where(e => e.InvoiceId == invoiceId).ExecuteDeleteAsync();
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await trans.RollbackAsync();
                return false;
            }
        }
        public async Task<bool> InvoiceExist(int invoiceId)
        {
            return await _handlerContext.Invoices.Where(e => e.InvoiceId == invoiceId).AnyAsync();
        }
        public async Task<IEnumerable<int>> GetInvoiceUser(int invoiceId)
        {
            return await _handlerContext.ItemOwners.Where(e => e.InvoiceId == invoiceId)
                .GroupBy(e => e.IdUser)
                .Select(e => e.Key)
                .Union(
                    _handlerContext.SellingPrices
                    .Where(d => d.PurchasePrice.InvoiceId == invoiceId)
                    .GroupBy(d => d.IdUser)
                    .Select(d => d.Key)
                ).ToListAsync();
        }
        public async Task<string> GetInvoiceNumber(int invoiceId)
        {
            return await _handlerContext.Invoices.Where(e => e.InvoiceId == invoiceId).Select(e => e.InvoiceNumber).FirstAsync();
        }
        public async Task<string?> GetInvoicePath(int invoiceId)
        {
            return await _handlerContext.Invoices.Where(e => e.InvoiceId == invoiceId).Select(e => e.InvoiceFilePath).FirstAsync();
        }
        public async Task<GetRestInvoice> GetRestPurchaseInvoice(int invoiceId)
        {
            var invoiceInfo = await _handlerContext.Invoices
                .Where(e => e.InvoiceId == invoiceId)
                .Select(e => new GetRestInvoice
                {
                    Tax = e.TaxesNavigation.TaxValue,
                    CurrencyValue = e.Currency.CurrencyValue1,
                    CurrencyName = e.CurrencyName,
                    CurrencyDate = e.CurrencyValueDate,
                    TransportCost = e.TransportCost,
                    PaymentType = e.PaymentMethod.MethodName,
                    Note = e.Note,
                    Path = e.InvoiceFilePath,
                    CreditNotes = e.CreditNotes.Select(e => e.CreditNoteNumber).ToList(),
                }
                ).FirstAsync();
            List<GetInvoiceItemsForTable> itemsInfo;

            if (invoiceInfo.CurrencyName == "PLN")
            {
                itemsInfo = await _handlerContext.PurchasePrices
                .Where(e => e.InvoiceId == invoiceId)
                .Select(e => new GetInvoiceItemsForTable
                {
                    Partnumber = e.OwnedItem.OriginalItem.PartNumber,
                    ItemName = e.OwnedItem.OriginalItem.ItemName,
                    Users = e.OwnedItem.ItemOwners.Select(d => d.IdUserNavigation.Username + " " + d.IdUserNavigation.Surname).ToList(),
                    Qty = e.Qty,
                    Price = e.Price
                }).ToListAsync();
            } else
            {
                itemsInfo = await _handlerContext.PurchasePrices
                .Where(e => e.InvoiceId == invoiceId)
                .Select(e => new GetInvoiceItemsForTable
                {
                    Partnumber = e.OwnedItem.OriginalItem.PartNumber,
                    ItemName = e.OwnedItem.OriginalItem.ItemName,
                    Users = e.OwnedItem.ItemOwners.Select(d => d.IdUserNavigation.Username + " " + d.IdUserNavigation.Surname).ToList(),
                    Qty = e.Qty,
                    Price = e.CalculatedPrices.Where(d => d.CurrencyName == invoiceInfo.CurrencyName).Select(d => d.Price).First()
                }).ToListAsync();
            }

            invoiceInfo.Items = itemsInfo;

            return invoiceInfo;
        }
        public async Task<GetRestInvoice> GetRestSalesInvoice(int invoiceId)
        {
            var invoiceInfo = await _handlerContext.Invoices
                .Where(e => e.InvoiceId == invoiceId)
                .Select(e => new GetRestInvoice
                {
                    Tax = e.TaxesNavigation.TaxValue,
                    CurrencyValue = e.Currency.CurrencyValue1,
                    CurrencyName = e.CurrencyName,
                    CurrencyDate = e.CurrencyValueDate,
                    TransportCost = e.TransportCost,
                    PaymentType = e.PaymentMethod.MethodName,
                    Note = e.Note,
                    Path = e.InvoiceFilePath,
                    CreditNotes = e.CreditNotes.Select(e => e.CreditNoteNumber).ToList(),
                }
                ).FirstAsync();
            var itemsInfo = await _handlerContext.SellingPrices
                .Where(e => e.SellInvoiceId == invoiceId)
                .Select(e => new GetInvoiceItemsForTable
                {
                    Partnumber = e.PurchasePrice.OwnedItem.OriginalItem.PartNumber,
                    ItemName = e.PurchasePrice.OwnedItem.OriginalItem.ItemName,
                    Users = e.PurchasePrice.OwnedItem.ItemOwners.Select(d => d.IdUserNavigation.Username + " " + d.IdUserNavigation.Surname).ToList(),
                    Qty = e.Qty,
                    Price = e.Price
                }).ToListAsync();

            invoiceInfo.Items = itemsInfo;

            return invoiceInfo;
        }
        public async Task<GetRestModifyInvoice> GetRestModifyInvoice(int invoiceId)
        {
            return await _handlerContext.Invoices
                .Where(e => e.InvoiceId == invoiceId)
                .Select(e => new GetRestModifyInvoice
                {
                    Transport = e.TransportCost,
                    PaymentMethod = e.PaymentMethod.MethodName,
                    Note = e.Note
                }).FirstAsync();
        }
        public async Task<bool> ModifyInvoice(ModifyInvoice data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                if (data.PaymentMethod != null)
                {
                    await _handlerContext.Invoices.Where(e => e.InvoiceId == data.InvoiceId).ExecuteUpdateAsync(setter =>
                        setter.SetProperty(s => s.PaymentMethodId, data.PaymentMethod)
                    );
                }
                if (data.PaymentStatus != null)
                {
                    await _handlerContext.Invoices.Where(e => e.InvoiceId == data.InvoiceId).ExecuteUpdateAsync(setter =>
                        setter.SetProperty(s => s.PaymentsStatusId, data.PaymentStatus)
                    );
                }
                if (data.TransportCost != null)
                {
                    await _handlerContext.Invoices.Where(e => e.InvoiceId == data.InvoiceId).ExecuteUpdateAsync(setter =>
                        setter.SetProperty(s => s.TransportCost, data.TransportCost)
                    );
                }
                if (data.InSystem != null)
                {
                    await _handlerContext.Invoices.Where(e => e.InvoiceId == data.InvoiceId).ExecuteUpdateAsync(setter =>
                        setter.SetProperty(s => s.InSystem, data.InSystem)
                    );
                }
                if (data.Note != null)
                {
                    await _handlerContext.Invoices.Where(e => e.InvoiceId == data.InvoiceId).ExecuteUpdateAsync(setter =>
                        setter.SetProperty(s => s.Note, data.Note)
                    );
                }
                if (data.InvoiceNumber != null)
                {
                    await _handlerContext.Invoices.Where(e => e.InvoiceId == data.InvoiceId).ExecuteUpdateAsync(setter =>
                        setter.SetProperty(s => s.InvoiceNumber, data.InvoiceNumber)
                    );
                }
                if (data.Path != null)
                {
                    await _handlerContext.Invoices.Where(e => e.InvoiceId == data.InvoiceId).ExecuteUpdateAsync(setter =>
                        setter.SetProperty(s => s.InvoiceFilePath, data.Path)
                    );
                }
                if (data.ClientId != null)
                {
                    if (data.IsYourInvoice)
                    {
                        await _handlerContext.Invoices.Where(e => e.InvoiceId == data.InvoiceId).ExecuteUpdateAsync(setter =>
                        setter.SetProperty(s => s.Seller, data.ClientId)
                        );
                    } else
                    {
                        await _handlerContext.Invoices.Where(e => e.InvoiceId == data.InvoiceId).ExecuteUpdateAsync(setter =>
                        setter.SetProperty(s => s.Buyer, data.ClientId)
                        );
                    }
                }
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            } catch (Exception ex)
            {
                Console.Write(ex.ToString());
                await trans.RollbackAsync();
                return false;
            }

        }
        public async Task UpdateInvoiceStatus()
        {
            var dueToStatusId = await _handlerContext.PaymentStatuses.Where(e => e.StatusName == "Due to").Select(e => e.PaymentStatusId).FirstAsync();
            await _handlerContext.Invoices
                .Where(e => DateTime.Now > e.DueDate && e.PaymentsStatus.StatusName == "Unpaid")
                .ExecuteUpdateAsync(setter =>
                    setter.SetProperty(s => s.PaymentsStatusId, dueToStatusId)
                );
        }
    }
}
