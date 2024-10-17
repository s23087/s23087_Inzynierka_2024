using database_communicator.Data;
using database_communicator.Models;
using database_communicator.Models.DTOs;
using database_communicator.Utils;
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
        public Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, string? sort,
            string? estimatedL, string? estimatedG, string? deliveredL, string? deliveredG, int? recipient, int? status, int? company, string? waybill);
        public Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, int userId, string? sort,
            string? estimatedL, string? estimatedG, string? deliveredL, string? deliveredG, int? recipient, int? status, int? company, string? waybill);
        public Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, string search, string? sort,
            string? estimatedL, string? estimatedG, string? deliveredL, string? deliveredG, int? recipient, int? status, int? company, string? waybill);
        public Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, int userId, string search, string? sort,
            string? estimatedL, string? estimatedG, string? deliveredL, string? deliveredG, int? recipient, int? status, int? company, string? waybill);
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
    public class DeliveryServices : IDeliveryServices
    {
        private readonly HandlerContext _handlerContext;
        public DeliveryServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }
        public async Task<bool> DoesDeliveryCompanyExist(string companyName)
        {
            return await _handlerContext.DeliveryCompanies.AnyAsync(x => x.DeliveryCompanyName.ToLower() == companyName.ToLower());
        }
        public async Task<bool> AddDeliveryCompany(string companyName)
        {
            try
            {
                await _handlerContext.DeliveryCompanies.AddAsync(new DeliveryCompany
                {
                    DeliveryCompanyName = companyName,
                });
                await _handlerContext.SaveChangesAsync();
                return true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
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
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
                return 0;
            }
        }
        public async Task<bool> DeliveryProformaExist(int proformaId)
        {
            return await _handlerContext.Deliveries.AnyAsync(x => x.ProformaId == proformaId);
        }
        public async Task<bool> DeliveryExist(int deliveryId)
        {
            return await _handlerContext.Deliveries.AnyAsync(x => x.DeliveryId == deliveryId);
        }
        public async Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, string? sort,
            string? estimatedL, string? estimatedG, string? deliveredL, string? deliveredG, int? recipient, int? status, int? company, string? waybill)
        {
            var sortFunc = SortFilterUtils.GetDeliverySort(sort, IsDeliveryToUser);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith("D");
            }
            Expression<Func<Delivery, bool>> estimatedLCond = estimatedL == null ?
                e => true
                : e => e.EstimatedDeliveryDate <= DateTime.Parse(estimatedL);

            Expression<Func<Delivery, bool>> estimatedGCond = estimatedG == null ?
                e => true
                : e => e.EstimatedDeliveryDate >= DateTime.Parse(estimatedG);

            Expression<Func<Delivery, bool>> deliveredLCond = deliveredL == null ?
                e => true
                : e => e.DeliveryDate <= DateTime.Parse(deliveredL);

            Expression<Func<Delivery, bool>> deliveredGCond = deliveredG == null ?
                e => true
                : e => e.DeliveryDate >= DateTime.Parse(deliveredG);

            Expression<Func<Delivery, bool>> recipientCond = recipient == null ?
                e => true
                : e => IsDeliveryToUser ? e.Proforma.Seller == recipient : e.Proforma.Buyer == recipient;

            Expression<Func<Delivery, bool>> statusCond = status == null ?
                e => true
                : e => e.DeliveryStatusId == status;

            Expression<Func<Delivery, bool>> companyCond = company == null ?
                e => true
                : e => e.DeliveryCompanyId == company;

            Expression<Func<Delivery, bool>> waybillCond = waybill == null ?
                e => true
                : e => e.Waybills.Any(x => x.WaybillValue == waybill);
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
        public async Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, int userId, string? sort,
            string? estimatedL, string? estimatedG, string? deliveredL, string? deliveredG, int? recipient, int? status, int? company, string? waybill)
        {
            var sortFunc = SortFilterUtils.GetDeliverySort(sort, IsDeliveryToUser);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith("D");
            }
            Expression<Func<Delivery, bool>> estimatedLCond = estimatedL == null ?
                e => true
                : e => e.EstimatedDeliveryDate <= DateTime.Parse(estimatedL);

            Expression<Func<Delivery, bool>> estimatedGCond = estimatedG == null ?
                e => true
                : e => e.EstimatedDeliveryDate >= DateTime.Parse(estimatedG);

            Expression<Func<Delivery, bool>> deliveredLCond = deliveredL == null ?
                e => true
                : e => e.DeliveryDate <= DateTime.Parse(deliveredL);

            Expression<Func<Delivery, bool>> deliveredGCond = deliveredG == null ?
                e => true
                : e => e.DeliveryDate >= DateTime.Parse(deliveredG);

            Expression<Func<Delivery, bool>> recipientCond = recipient == null ?
                e => true
                : e => IsDeliveryToUser ? e.Proforma.Seller == recipient : e.Proforma.Buyer == recipient;

            Expression<Func<Delivery, bool>> statusCond = status == null ?
            e => true
            : e => e.DeliveryStatusId == status;

            Expression<Func<Delivery, bool>> companyCond = company == null ?
                e => true
                : e => e.DeliveryCompanyId == company;

            Expression<Func<Delivery, bool>> waybillCond = waybill == null ?
                e => true
                : e => e.Waybills.Any(x => x.WaybillValue == waybill);
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
        public async Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, string search, string? sort,
            string? estimatedL, string? estimatedG, string? deliveredL, string? deliveredG, int? recipient, int? status, int? company, string? waybill)
        {
            var sortFunc = SortFilterUtils.GetDeliverySort(sort, IsDeliveryToUser);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith("D");
            }
            Expression<Func<Delivery, bool>> estimatedLCond = estimatedL == null ?
                e => true
                : e => e.EstimatedDeliveryDate <= DateTime.Parse(estimatedL);

            Expression<Func<Delivery, bool>> estimatedGCond = estimatedG == null ?
                e => true
                : e => e.EstimatedDeliveryDate >= DateTime.Parse(estimatedG);

            Expression<Func<Delivery, bool>> deliveredLCond = deliveredL == null ?
                e => true
                : e => e.DeliveryDate <= DateTime.Parse(deliveredL);

            Expression<Func<Delivery, bool>> deliveredGCond = deliveredG == null ?
                e => true
                : e => e.DeliveryDate >= DateTime.Parse(deliveredG);

            Expression<Func<Delivery, bool>> recipientCond = recipient == null ?
                e => true
                : e => IsDeliveryToUser ? e.Proforma.Seller == recipient : e.Proforma.Buyer == recipient;

            Expression<Func<Delivery, bool>> statusCond = status == null ?
                e => true
                : e => e.DeliveryStatusId == status;

            Expression<Func<Delivery, bool>> companyCond = company == null ?
                e => true
                : e => e.DeliveryCompanyId == company;

            Expression<Func<Delivery, bool>> waybillCond = waybill == null ?
                e => true
                : e => e.Waybills.Any(x => x.WaybillValue == waybill);

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
        public async Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, int userId, string search, string? sort,
            string? estimatedL, string? estimatedG, string? deliveredL, string? deliveredG, int? recipient, int? status, int? company, string? waybill)
        {
            var sortFunc = SortFilterUtils.GetDeliverySort(sort, IsDeliveryToUser);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith("D");
            }
            Expression<Func<Delivery, bool>> estimatedLCond = estimatedL == null ?
                e => true
                : e => e.EstimatedDeliveryDate <= DateTime.Parse(estimatedL);

            Expression<Func<Delivery, bool>> estimatedGCond = estimatedG == null ?
                e => true
                : e => e.EstimatedDeliveryDate >= DateTime.Parse(estimatedG);

            Expression<Func<Delivery, bool>> deliveredLCond = deliveredL == null ?
                e => true
                : e => e.DeliveryDate <= DateTime.Parse(deliveredL);

            Expression<Func<Delivery, bool>> deliveredGCond = deliveredG == null ?
                e => true
                : e => e.DeliveryDate >= DateTime.Parse(deliveredG);

            Expression<Func<Delivery, bool>> recipientCond = recipient == null ?
                e => true
                : e => IsDeliveryToUser ? e.Proforma.Seller == recipient : e.Proforma.Buyer == recipient;

            Expression<Func<Delivery, bool>> statusCond = status == null ?
                e => true
                : e => e.DeliveryStatusId == status;

            Expression<Func<Delivery, bool>> companyCond = company == null ?
                e => true
                : e => e.DeliveryCompanyId == company;

            Expression<Func<Delivery, bool>> waybillCond = waybill == null ?
                e => true
                : e => e.Waybills.Any(x => x.WaybillValue == waybill);

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
        public async Task<IEnumerable<GetDeliveryCompany>> GetDeliveryCompanies()
        {
            return await _handlerContext.DeliveryCompanies.Select(e => new GetDeliveryCompany
            {
                Id = e.DeliveryCompanyId,
                Name = e.DeliveryCompanyName
            }).ToListAsync();
        }
        public async Task<IEnumerable<GetDeliveryStatus>> GetDeliveryStatuses()
        {
            return await _handlerContext.DeliveryStatuses.Select(e => new GetDeliveryStatus
            {
                Id = e.DeliveryStatusId,
                Name = e.StatusName
            }).ToListAsync();
        }
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
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
                return false;
            }
        }
        public async Task<int> GetDeliveryOwnerId(int deliveryId)
        {
            return await _handlerContext.Deliveries
                .Where(e => e.DeliveryId == deliveryId)
                .Select(e => e.Proforma.UserId).FirstAsync();
        }
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
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
                return false;
            }
        }
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
                Console.WriteLine(ex.Message);
                await trans.RollbackAsync();
                return false;
            }
        }
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
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
                return false;
            }
        }
        public async Task<IEnumerable<int>> GetWarehouseManagerIds()
        {
            return await _handlerContext.OrgUsers.Where(e => e.Role.RoleName == "Warehouse Manager").SelectMany(e => e.AppUsers).Select(e => e.IdUser).ToListAsync();
        }
    }
}
