using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace database_comunicator.Services
{
    public interface IDeliveryServices
    {
        public Task<bool> DoesDeliveryCompanyExist(string companyName);
        public Task AddDeliveryCompany(string companyName);
        public Task<int> AddDelivery(AddDelivery data);
        public Task<bool> DeliveryExist(int proformaId);
        public Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser);
        public Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, int userId);
        public Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, string search);
        public Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, int userId, string search);
        public Task<IEnumerable<GetDeliveryCompany>> GetDeliveryCompanies();
        public Task<IEnumerable<GetProformaList>> GetProformaListWithoutDelivery(bool IsDeliveryToUser, int userId);
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
        public async Task AddDeliveryCompany(string companyName)
        {
            await _handlerContext.DeliveryCompanies.AddAsync(new DeliveryCompany
            {
                DeliveryCompanyName = companyName,
            });
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
                var waybils = data.Waybills.Select(e => new Waybill
                {
                    WaybillValue = e,
                    DeliveriesId = newDelivery.DeliveryId
                }).ToList();
                await _handlerContext.Waybills.AddRangeAsync(waybils);
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
        public async Task<bool> DeliveryExist(int proformaId)
        {
            return await _handlerContext.Deliveries.AnyAsync(x => x.ProformaId == proformaId);
        }
        public async Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser)
        {
            return await _handlerContext.Deliveries
                .Where(e => IsDeliveryToUser ? e.Proforma.ProformaFutureItems.Any() : e.Proforma.ProformaOwnedItems.Any())
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
        public async Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, int userId)
        {
            return await _handlerContext.Deliveries
                .Where(e => IsDeliveryToUser ? e.Proforma.ProformaFutureItems.Any() : e.Proforma.ProformaOwnedItems.Any())
                .Where(e => e.Proforma.UserId == userId)
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
        public async Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, string search)
        {
            return await _handlerContext.Deliveries
                .Where(e => IsDeliveryToUser ? e.Proforma.ProformaFutureItems.Any() : e.Proforma.ProformaOwnedItems.Any())
                .Where(e => e.Proforma.ProformaNumber.ToLower().Contains(search.ToLower()) || search.Contains((char)e.DeliveryId))
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
        public async Task<IEnumerable<GetDelivery>> GetDeliveries(bool IsDeliveryToUser, int userId, string search)
        {
            return await _handlerContext.Deliveries
                .Where(e => IsDeliveryToUser ? e.Proforma.ProformaFutureItems.Any() : e.Proforma.ProformaOwnedItems.Any())
                .Where(e => e.Proforma.UserId == userId)
                .Where(e => e.Proforma.ProformaNumber.ToLower().Contains(search.ToLower()) || search.Contains((char)e.DeliveryId))
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
    }
}
