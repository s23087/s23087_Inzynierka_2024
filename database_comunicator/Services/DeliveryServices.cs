using database_communicator.Data;
using database_communicator.Models;
using database_communicator.Utils;
using database_communicator.FilterClass;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using database_communicator.Models.DTOs.Modify;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace database_communicator.Services
{
    public interface IDeliveryServices
    {
        public Task<bool> DoesDeliveryCompanyExist(string companyName);
        public Task<bool> AddDeliveryCompany(string companyName);
        public Task<int> AddDelivery(AddDelivery data);
        public Task<bool> DeliveryProformaExist(int proformaId);
        public Task<bool> DeliveryExist(int deliveryId);
        public Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, string? sort, DeliveryFiltersTemplate filters);
        public Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, int userId, string? sort, DeliveryFiltersTemplate filters);
        public Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, string search, string? sort, DeliveryFiltersTemplate filters);
        public Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, int userId, string search, string? sort, DeliveryFiltersTemplate filters);
        public Task<IEnumerable<GetDeliveryCompany>> GetDeliveryCompanies();
        public Task<IEnumerable<GetDeliveryStatus>> GetDeliveryStatuses();
        public Task<IEnumerable<GetProformaList>> GetProformaListWithoutDelivery(bool IsDeliveryToUser, int userId);
        public Task<bool> DeleteDelivery(int deliveryId);
        public Task<int> GetDeliveryOwnerId(int deliveryId);
        public Task<GetRestDelivery> GetRestDelivery(int deliveryId);
        public Task<bool> AddNote(AddNote data);
        public Task<bool> SetDeliveryStatus(SetDeliveryStatus data);
        public Task<bool> ModifyDelivery(ModifyDelivery data);
        public Task<IEnumerable<int>> GetWarehouseManagerIds();
    }
    /// <summary>
    /// Class that interact with database and contains functions allowing to work on deliveries.
    /// </summary>
    public class DeliveryServices : IDeliveryServices
    {
        private readonly HandlerContext _handlerContext;
        private readonly ILogger<CreditNoteServices> _logger;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handlerContext">Database context</param>
        /// <param name="logger">Log interface</param>
        public DeliveryServices(HandlerContext handlerContext, ILogger<CreditNoteServices> logger)
        {
            _handlerContext = handlerContext;
            _logger = logger;

        }
        /// <summary>
        /// Check if company with given name exist in Delivery companies table.
        /// </summary>
        /// <param name="companyName">Name of searched company.</param>
        /// <returns>True if exist, false if do not exist.</returns>
        public async Task<bool> DoesDeliveryCompanyExist(string companyName)
        {
            return await _handlerContext.DeliveryCompanies.AnyAsync(x => x.DeliveryCompanyName.ToLower() == companyName.ToLower());
        }
        /// <summary>
        /// Using transaction add new delivery company to database.
        /// </summary>
        /// <param name="companyName">Name of new delivery company.</param>
        /// <returns>True if success or false if fails.</returns>
        public async Task<bool> AddDeliveryCompany(string companyName)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                await _handlerContext.DeliveryCompanies.AddAsync(new DeliveryCompany
                {
                    DeliveryCompanyName = companyName,
                });
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Add delivery company error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Using transactions add new delivery to database.
        /// </summary>
        /// <param name="data">New delivery data wrapped in <see cref="Models.DTOs.Create.AddDelivery"/></param>
        /// <returns>New delivery id or 0 when action was unsuccessful.</returns>
        public async Task<int> AddDelivery(AddDelivery data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var deliveryStatusId = await _handlerContext.DeliveryStatuses
                .Where(e => data.IsDeliveryToUser ? e.StatusName == "In transport" : e.StatusName == "Preparing")
                .Select(e => e.DeliveryStatusId).FirstAsync();
                var newDelivery = new Delivery
                {
                    EstimatedDeliveryDate = data.EstimatedDeliveryDate,
                    DeliveryDate = null,
                    DeliveryStatusId = deliveryStatusId,
                    ProformaId = data.ProformaId,
                    DeliveryCompanyId = data.CompanyId
                };
                await _handlerContext.Deliveries.AddAsync(newDelivery);
                await _handlerContext.SaveChangesAsync();
                var waybills = data.Waybills.Select(e => new Waybill
                {
                    WaybillValue = e,
                    DeliveriesId = newDelivery.DeliveryId
                }).ToList();
                await _handlerContext.Waybills.AddRangeAsync(waybills);
                if (data.Note != null)
                {
                    var newNote = new Note
                    {
                        NoteDescription = data.Note,
                        NoteDate = DateTime.Now,
                        UsersId = data.UserId
                    };
                    await _handlerContext.Notes.AddAsync(newNote);
                    await _handlerContext.SaveChangesAsync();
                    await _handlerContext.Database.ExecuteSqlAsync($"insert into Notes_Delivery (delivery_id, note_id) Values ({newDelivery.DeliveryId}, {newNote.NoteId})");
                }
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return newDelivery.DeliveryId;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Add delivery error.");
                await trans.RollbackAsync();
                return 0;
            }
        }
        /// <summary>
        /// Checks if delivery with given proforma exist.
        /// </summary>
        /// <param name="proformaId">Proforma id.</param>
        /// <returns>True if exist, false if not.</returns>
        public async Task<bool> DeliveryProformaExist(int proformaId)
        {
            return await _handlerContext.Deliveries.AnyAsync(x => x.ProformaId == proformaId);
        }
        /// <summary>
        /// Checks if delivery with given id exist.
        /// </summary>
        /// <param name="deliveryId">Delivery id.</param>
        /// <returns>True if exist, false if do not exist.</returns>
        public async Task<bool> DeliveryExist(int deliveryId)
        {
            return await _handlerContext.Deliveries.AnyAsync(x => x.DeliveryId == deliveryId);
        }
        /// <summary>
        /// Do select query with given filter and sort to receive all deliveries information.
        /// </summary>
        /// <param name="IsDeliveryToUser">True if delivery destination is user warehouse, otherwise false.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="DeliveryFiltersTemplate"/></param>
        /// <returns>List of <see cref="GetDelivery"/>.</returns>
        public async Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, string? sort, DeliveryFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetDeliverySort(sort, IsDeliveryToUser);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var estimatedLCond = DeliveryFilters.GetEstimatedLowerFilter(filters.EstimatedL);
            var estimatedGCond = DeliveryFilters.GetEstimatedGreaterFilter(filters.EstimatedG);
            var deliveredLCond = DeliveryFilters.GetDeliveredLowerFilter(filters.DeliveredL);
            var deliveredGCond = DeliveryFilters.GetDeliveredGreaterFilter(filters.DeliveredG);
            var recipientCond = DeliveryFilters.GetRecipientFilter(filters.Recipient, IsDeliveryToUser);
            var statusCond = DeliveryFilters.GetStatusFilter(filters.Status);
            var companyCond = DeliveryFilters.GetCompanyFilter(filters.Company);
            var waybillCond = DeliveryFilters.GetWaybillFilter(filters.Waybill);

            return await _handlerContext.Deliveries
                .Where(e => IsDeliveryToUser ? e.Proforma.ProformaFutureItems.Any() : e.Proforma.ProformaOwnedItems.Any())
                .Where(estimatedLCond)
                .Where(estimatedGCond)
                .Where(deliveredLCond)
                .Where(deliveredGCond)
                .Where(recipientCond)
                .Where(statusCond)
                .Where(companyCond)
                .Where(waybillCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(e => new GetDelivery
                {
                    User = e.Proforma.User.Username + " " + e.Proforma.User.Surname,
                    DeliveryId = e.DeliveryId,
                    Status = e.DeliveryStatus.StatusName,
                    Waybill = e.Waybills.Select(d => d.WaybillValue),
                    DeliveryCompany = e.DeliveryCompany.DeliveryCompanyName,
                    Estimated = e.EstimatedDeliveryDate,
                    Proforma = e.Proforma.ProformaNumber,
                    ClientName = IsDeliveryToUser ? e.Proforma.SellerNavigation.OrgName : e.Proforma.BuyerNavigation.OrgName,
                    Delivered = e.DeliveryDate
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query with given filter and sort to receive deliveries information for chosen user.
        /// </summary>
        /// <param name="IsDeliveryToUser">True if delivery destination is user warehouse, otherwise false.</param>
        /// <param name="userId">Id of user</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="DeliveryFiltersTemplate"/></param>
        /// <returns>List of <see cref="GetDelivery"/>.</returns>
        public async Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, int userId, string? sort, DeliveryFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetDeliverySort(sort, IsDeliveryToUser);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var estimatedLCond = DeliveryFilters.GetEstimatedLowerFilter(filters.EstimatedL);
            var estimatedGCond = DeliveryFilters.GetEstimatedGreaterFilter(filters.EstimatedG);
            var deliveredLCond = DeliveryFilters.GetDeliveredLowerFilter(filters.DeliveredL);
            var deliveredGCond = DeliveryFilters.GetDeliveredGreaterFilter(filters.DeliveredG);
            var recipientCond = DeliveryFilters.GetRecipientFilter(filters.Recipient, IsDeliveryToUser);
            var statusCond = DeliveryFilters.GetStatusFilter(filters.Status);
            var companyCond = DeliveryFilters.GetCompanyFilter(filters.Company);
            var waybillCond = DeliveryFilters.GetWaybillFilter(filters.Waybill);

            return await _handlerContext.Deliveries
                .Where(e => IsDeliveryToUser ? e.Proforma.ProformaFutureItems.Any() : e.Proforma.ProformaOwnedItems.Any())
                .Where(e => e.Proforma.UserId == userId)
                .Where(estimatedLCond)
                .Where(estimatedGCond)
                .Where(deliveredLCond)
                .Where(deliveredGCond)
                .Where(recipientCond)
                .Where(statusCond)
                .Where(companyCond)
                .Where(waybillCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(obj => new GetDelivery
                {
                    DeliveryId = obj.DeliveryId,
                    Status = obj.DeliveryStatus.StatusName,
                    Waybill = obj.Waybills.Select(e => e.WaybillValue),
                    DeliveryCompany = obj.DeliveryCompany.DeliveryCompanyName,
                    Estimated = obj.EstimatedDeliveryDate,
                    Proforma = obj.Proforma.ProformaNumber,
                    ClientName = IsDeliveryToUser ? obj.Proforma.SellerNavigation.OrgName : obj.Proforma.BuyerNavigation.OrgName,
                    Delivered = obj.DeliveryDate
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query with given search, filter and sort to receive all deliveries information.
        /// </summary>
        /// <param name="IsDeliveryToUser">True if delivery destination is user warehouse, otherwise false.</param>
        /// <param name="search">The phrase searched in deliveries information. It will check if phrase exist in proforma number or delivery id.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="DeliveryFiltersTemplate"/></param>
        /// <returns>List of <see cref="GetDelivery"/>.</returns>
        public async Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, string search, string? sort, DeliveryFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetDeliverySort(sort, IsDeliveryToUser);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var estimatedLCond = DeliveryFilters.GetEstimatedLowerFilter(filters.EstimatedL);
            var estimatedGCond = DeliveryFilters.GetEstimatedGreaterFilter(filters.EstimatedG);
            var deliveredLCond = DeliveryFilters.GetDeliveredLowerFilter(filters.DeliveredL);
            var deliveredGCond = DeliveryFilters.GetDeliveredGreaterFilter(filters.DeliveredG);
            var recipientCond = DeliveryFilters.GetRecipientFilter(filters.Recipient, IsDeliveryToUser);
            var statusCond = DeliveryFilters.GetStatusFilter(filters.Status);
            var companyCond = DeliveryFilters.GetCompanyFilter(filters.Company);
            var waybillCond = DeliveryFilters.GetWaybillFilter(filters.Waybill);

            int searchById;

            try
            {
                searchById = Int32.Parse(search);
            } catch
            {
                searchById = -1;
            }

            return await _handlerContext.Deliveries
                .Where(e => IsDeliveryToUser ? e.Proforma.ProformaFutureItems.Any() : e.Proforma.ProformaOwnedItems.Any())
                .Where(e => e.Proforma.ProformaNumber.ToLower().Contains(search.ToLower()) || (searchById == -1 || e.DeliveryId == searchById))
                .Where(estimatedLCond)
                .Where(estimatedGCond)
                .Where(deliveredLCond)
                .Where(deliveredGCond)
                .Where(recipientCond)
                .Where(statusCond)
                .Where(companyCond)
                .Where(waybillCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(ent => new GetDelivery
                {
                    User = ent.Proforma.User.Username + " " + ent.Proforma.User.Surname,
                    DeliveryId = ent.DeliveryId,
                    Status = ent.DeliveryStatus.StatusName,
                    Waybill = ent.Waybills.Select(e => e.WaybillValue),
                    DeliveryCompany = ent.DeliveryCompany.DeliveryCompanyName,
                    Estimated = ent.EstimatedDeliveryDate,
                    Proforma = ent.Proforma.ProformaNumber,
                    ClientName = IsDeliveryToUser ? ent.Proforma.SellerNavigation.OrgName : ent.Proforma.BuyerNavigation.OrgName,
                    Delivered = ent.DeliveryDate
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query with given search, filter and sort to receive deliveries information for chosen user.
        /// </summary>
        /// <param name="IsDeliveryToUser">True if delivery destination is user warehouse, otherwise false.</param>
        /// <param name="userId">Id of user</param>
        /// <param name="search">The phrase searched in deliveries information. It will check if phrase exist in proforma number or delivery id.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="DeliveryFiltersTemplate"/></param>
        /// <returns>List of <see cref="GetDelivery"/>.</returns>
        public async Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, int userId, string search, string? sort, DeliveryFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetDeliverySort(sort, IsDeliveryToUser);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var estimatedLCond = DeliveryFilters.GetEstimatedLowerFilter(filters.EstimatedL);
            var estimatedGCond = DeliveryFilters.GetEstimatedGreaterFilter(filters.EstimatedG);
            var deliveredLCond = DeliveryFilters.GetDeliveredLowerFilter(filters.DeliveredL);
            var deliveredGCond = DeliveryFilters.GetDeliveredGreaterFilter(filters.DeliveredG);
            var recipientCond = DeliveryFilters.GetRecipientFilter(filters.Recipient, IsDeliveryToUser);
            var statusCond = DeliveryFilters.GetStatusFilter(filters.Status);
            var companyCond = DeliveryFilters.GetCompanyFilter(filters.Company);
            var waybillCond = DeliveryFilters.GetWaybillFilter(filters.Waybill);

            int searchById;

            try
            {
                searchById = Int32.Parse(search);
            }
            catch
            {
                searchById = -1;
            }

            return await _handlerContext.Deliveries
                .Where(e => IsDeliveryToUser ? e.Proforma.ProformaFutureItems.Any() : e.Proforma.ProformaOwnedItems.Any())
                .Where(e => e.Proforma.UserId == userId)
                .Where(e => e.Proforma.ProformaNumber.ToLower().Contains(search.ToLower()) || (searchById == -1 || e.DeliveryId == searchById))
                .Where(estimatedLCond)
                .Where(estimatedGCond)
                .Where(deliveredLCond)
                .Where(deliveredGCond)
                .Where(recipientCond)
                .Where(statusCond)
                .Where(companyCond)
                .Where(waybillCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(deliv => new GetDelivery
                {
                    DeliveryId = deliv.DeliveryId,
                    Status = deliv.DeliveryStatus.StatusName,
                    Waybill = deliv.Waybills.Select(e => e.WaybillValue),
                    DeliveryCompany = deliv.DeliveryCompany.DeliveryCompanyName,
                    Estimated = deliv.EstimatedDeliveryDate,
                    Proforma = deliv.Proforma.ProformaNumber,
                    ClientName = IsDeliveryToUser ? deliv.Proforma.SellerNavigation.OrgName : deliv.Proforma.BuyerNavigation.OrgName,
                    Delivered = deliv.DeliveryDate
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query on Delivery Companies table.
        /// </summary>
        /// <returns>List of delivery companies with name and id.</returns>
        public async Task<IEnumerable<GetDeliveryCompany>> GetDeliveryCompanies()
        {
            return await _handlerContext.DeliveryCompanies.Select(e => new GetDeliveryCompany
            {
                Id = e.DeliveryCompanyId,
                Name = e.DeliveryCompanyName
            }).ToListAsync();
        }
        /// <summary>
        /// Do select query on Delivery Statuses table.
        /// </summary>
        /// <returns>List of delivery statuses with id and name.</returns>
        public async Task<IEnumerable<GetDeliveryStatus>> GetDeliveryStatuses()
        {
            return await _handlerContext.DeliveryStatuses.Select(e => new GetDeliveryStatus
            {
                Id = e.DeliveryStatusId,
                Name = e.StatusName
            }).ToListAsync();
        }
        /// <summary>
        /// Do select query on Proforma table.
        /// </summary>
        /// <param name="IsDeliveryToUser">True if delivery destination is user warehouse, otherwise false.</param>
        /// <param name="userId">Id of user.</param>
        /// <returns>List of Proformas with id and proforma number that belongs to chosen user.</returns>
        public async Task<IEnumerable<GetProformaList>> GetProformaListWithoutDelivery(bool IsDeliveryToUser, int userId)
        {
            return await _handlerContext.Proformas
                .Where(e => !e.Deliveries.Any() && (IsDeliveryToUser ? e.ProformaFutureItems.Any() : e.ProformaOwnedItems.Any()) && e.UserId == userId)
                .Select(e => new GetProformaList
                {
                    Id = e.ProformaId,
                    ProformaNumber = e.ProformaNumber,
                }).ToListAsync();
        }
        /// <summary>
        /// Using transaction delete delivery from database.
        /// </summary>
        /// <param name="deliveryId">Id of delivery to delete.</param>
        /// <returns>True if success or false if failure.</returns>
        public async Task<bool> DeleteDelivery(int deliveryId)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var notesIds = await _handlerContext.Notes.Where(e => e.Deliveries.Any(x => x.DeliveryId == deliveryId)).Select(e => e.NoteId).ToListAsync();
                foreach (var noteId in notesIds)
                {
                    await _handlerContext.Database.ExecuteSqlAsync($"Delete from Notes_Delivery where delivery_id={deliveryId} and note_id={noteId}");
                    await _handlerContext.Notes.Where(e => e.NoteId == noteId).ExecuteDeleteAsync();
                }
                await _handlerContext.Waybills
                    .Where(e => e.DeliveriesId == deliveryId)
                    .ExecuteDeleteAsync();
                await _handlerContext.Deliveries
                    .Where(e => e.DeliveryId == deliveryId)
                    .ExecuteDeleteAsync();
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Delete delivery error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Do select query on Delivery table for owner of delivery.
        /// </summary>
        /// <param name="deliveryId"></param>
        /// <returns>User id that's a owner of delivery.</returns>
        public async Task<int> GetDeliveryOwnerId(int deliveryId)
        {
            return await _handlerContext.Deliveries
                .Where(e => e.DeliveryId == deliveryId)
                .Select(e => e.Proforma.UserId).FirstAsync();
        }
        /// <summary>
        /// Do select query using given id to receive delivery information that was not given in bulk query.
        /// </summary>
        /// <param name="deliveryId">Id of delivery</param>
        /// <returns>Object containing delivery items, note, note date and name of user.</returns>
        public async Task<GetRestDelivery> GetRestDelivery(int deliveryId)
        {
            var isYourProforma = await _handlerContext.Deliveries
                .Where(e => e.DeliveryId == deliveryId)
                .AnyAsync(e => e.Proforma.ProformaFutureItems.Any());
            var restInfo = new GetRestDelivery
            {
                Notes = await _handlerContext.Notes
                .Where(e => e.Deliveries.Any(d => d.DeliveryId == deliveryId))
                .Select(e => new GetNote
                {
                    NoteDate = e.NoteDate,
                    Username = e.Users.Username + " " + e.Users.Surname,
                    Note = e.NoteDescription
                }).ToListAsync()
            };
            if (isYourProforma)
            {
                restInfo.Items = await _handlerContext.Deliveries
                     .Where(e => e.DeliveryId == deliveryId)
                     .SelectMany(e => e.Proforma.ProformaFutureItems)
                     .Select(e => new GetItemForDeliveryTable
                     {
                         ItemName = e.Item.ItemName,
                         Partnumber = e.Item.PartNumber,
                         Qty = e.Qty
                     }).ToListAsync();
            } else
            {
                restInfo.Items = await _handlerContext.Deliveries
                     .Where(e => e.DeliveryId == deliveryId)
                     .SelectMany(e => e.Proforma.ProformaOwnedItems)
                     .Select(e => new GetItemForDeliveryTable
                     {
                         ItemName = e.Item.OwnedItem.OriginalItem.ItemName,
                         Partnumber = e.Item.OwnedItem.OriginalItem.PartNumber,
                         Qty = e.Qty
                     }).ToListAsync();
            }
            return restInfo;
        }
        /// <summary>
        /// Using transactions add delivery note.
        /// </summary>
        /// <param name="data">New note data wrapped in <see cref="Models.DTOs.Create.AddNote"/>.</param>
        /// <returns>True if success, false if failure.</returns>
        public async Task<bool> AddNote(AddNote data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var newNote = new Note
                {
                    NoteDescription = data.Note,
                    NoteDate = DateTime.Now,
                    UsersId = data.UserId
                };
                await _handlerContext.Notes.AddAsync(newNote);
                await _handlerContext.SaveChangesAsync();
                await _handlerContext.Database.ExecuteSqlAsync($"insert into Notes_Delivery (delivery_id, note_id) Values ({data.DeliveryId}, {newNote.NoteId})");
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Add delivery note error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Using transactions change delivery status. If status is equal to Fulfilled, Rejected or Delivered with issues then it's sets delivery date to Now.
        /// </summary>
        /// <param name="data">Object containing new delivery status</param>
        /// <returns>True if success or false if failure.</returns>
        public async Task<bool> SetDeliveryStatus(SetDeliveryStatus data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var statusId = await _handlerContext.DeliveryStatuses
                .Where(e => e.StatusName == data.StatusName)
                .Select(e => e.DeliveryStatusId).FirstAsync();
                await _handlerContext.Deliveries
                    .Where(e => e.DeliveryId == data.DeliveryId)
                    .ExecuteUpdateAsync(setter =>
                        setter.SetProperty(s => s.DeliveryStatusId, statusId)
                    );
                string[] statusDateChange = { "Fulfilled", "Rejected", "Delivered with issues" };
                if (statusDateChange.Contains(data.StatusName))
                {
                    await _handlerContext.Deliveries
                    .Where(e => e.DeliveryId == data.DeliveryId)
                    .ExecuteUpdateAsync(setter =>
                        setter.SetProperty(s => s.DeliveryDate, DateTime.Now)
                    );
                }
                else
                {
                    await _handlerContext.Deliveries
                    .Where(e => e.DeliveryId == data.DeliveryId)
                    .ExecuteUpdateAsync(setter =>
                        setter.SetProperty(s => s.DeliveryDate, (DateTime?)null)
                    );
                }
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Set delivery status error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Using transactions overwrites chosen delivery properties with new ones.
        /// </summary>
        /// <param name="data">New delivery properties values.</param>
        /// <returns>True if success or false if failure.</returns>
        public async Task<bool> ModifyDelivery(ModifyDelivery data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                if (data.Estimated != null)
                {
                    await _handlerContext.Deliveries
                        .Where(e => e.DeliveryId == data.DeliveryId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.EstimatedDeliveryDate, data.Estimated)
                        );
                }
                if (data.CompanyId != null)
                {
                    await _handlerContext.Deliveries
                        .Where(e => e.DeliveryId == data.DeliveryId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.DeliveryCompanyId, data.CompanyId)
                        );
                }
                if (data.Waybills != null)
                {
                    await _handlerContext.Waybills
                        .Where(e => e.DeliveriesId == data.DeliveryId && !data.Waybills.Contains(e.WaybillValue))
                        .ExecuteDeleteAsync();
                    var restWaybills = await _handlerContext.Waybills.Where(e => e.DeliveriesId == data.DeliveryId).Select(e => e.WaybillValue).ToListAsync();
                    foreach ( var waybill in data.Waybills.Where(e => !restWaybills.Contains(e)) )
                    {
                        await _handlerContext.Waybills.AddAsync(new Waybill
                        {
                            DeliveriesId = data.DeliveryId,
                            WaybillValue = waybill
                        });
                    }
                }
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Modify delivery error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Do select query to receive ids of warehouse managers.
        /// </summary>
        /// <returns>List of warehouse managers ids.</returns>
        public async Task<IEnumerable<int>> GetWarehouseManagerIds()
        {
            return await _handlerContext.OrgUsers.Where(e => e.Role.RoleName == "Warehouse Manager").SelectMany(e => e.AppUsers).Select(e => e.IdUser).ToListAsync();
        }
    }
}
