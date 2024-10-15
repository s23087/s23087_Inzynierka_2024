using database_communicator.Data;
using database_communicator.Models;
using database_communicator.Models.DTOs;
using database_communicator.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace database_communicator.Services
{
    public interface IRequestServices
    {
        public Task<int> GetStatusId(string status);
        public Task<int> AddRequest(AddRequest data);
        public Task<IEnumerable<GetRequest>> GetMyRequests(int userId, string? sort, string? dateL, string? dateG,
            string? type, int? status);
        public Task<IEnumerable<GetRequest>> GetMyRequests(int userId, string search, string? sort, string? dateL, string? dateG,
            string? type, int? status);
        public Task<IEnumerable<GetRequest>> GetReceivedRequests(int userId, string? sort, string? dateL, string? dateG,
            string? type, int? status);
        public Task<IEnumerable<GetRequest>> GetReceivedRequests(int userId, string search, string? sort, string? dateL, string? dateG,
            string? type, int? status);
        public Task<GetRestRequest> GetRestRequest(int requestId);
        public Task<bool> RequestExist(int requestId);
        public Task DeleteRequest(int requestId);
        public Task<bool> ModifyRequest(ModifyRequest data);
        public Task<bool> SetRequestStatus(int requestId, int statusId, SetRequestStatus data);
        public Task<string?> GetRequestPath(int requestId);
        public Task<int> GetReceiverId(int requestId);
        public Task<GetRestModifyRequest> GetRestModifyRequest(int requestId);

        public Task<IEnumerable<GetRequestStatus>> GetRequestStatuses();
    }
    public class RequestServices : IRequestServices
    {
        private readonly HandlerContext _handlerContext;
        public RequestServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }
        public async Task<int> GetStatusId(string status)
        {
            return await _handlerContext.RequestStatuses
                .Where(e => e.StatusName == status)
                .Select(e => e.RequestStatusId)
                .FirstAsync();
        }
        public async Task<int> AddRequest(AddRequest data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var typeId = await _handlerContext.ObjectTypes.Where(e => e.ObjectTypeName == data.ObjectType).Select(e => e.ObjectTypeId).FirstAsync();
                var statusId = await _handlerContext.RequestStatuses.Where(e => e.StatusName == "In progress").Select(e => e.RequestStatusId).FirstAsync();
                var toAdd = new Request
                {
                    IdUserCreator = data.CreatorId,
                    IdUserReciver = data.ReceiverId,
                    RequestStatusId = statusId,
                    ObjectTypeId = typeId,
                    FilePath = data.Path,
                    Note = data.Note,
                    Title = data.Title,
                    CreationDate = DateTime.Now,
                };
                await _handlerContext.Requests.AddAsync(toAdd);
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return toAdd.RequestId;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
                return 0;
            }
        }
        public async Task<IEnumerable<GetRequest>> GetMyRequests(int userId, string? sort, string? dateL, string? dateG,
            string? type, int? status)
        {
            var sortFunc = SortFilterUtils.GetRequestSort(sort);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith("D");
            }
            Expression<Func<Request, bool>> dateLCond = dateL == null ?
                e => true
                : e => e.CreationDate <= DateTime.Parse(dateL);

            Expression<Func<Request, bool>> dateGCond = dateG == null ?
                e => true
                : e => e.CreationDate >= DateTime.Parse(dateG);

            Expression<Func<Request, bool>> typeCond = type == null ?
                e => true
                : e => e.ObjectType.ObjectTypeName == type;

            Expression<Func<Request, bool>> statusCond = status == null ?
                e => true
                : e => e.RequestStatusId == status;

            return await _handlerContext.Requests
                .Where(e => e.IdUserCreator == userId)
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(typeCond)
                .Where(statusCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(e => new GetRequest
                {
                    Id = e.RequestId,
                    Username = e.UserReciver.Username + " " + e.UserReciver.Surname,
                    Status = e.RequestStatus.StatusName,
                    ObjectType = e.ObjectType.ObjectTypeName,
                    CreationDate = e.CreationDate,
                    Title = e.Title
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetRequest>> GetMyRequests(int userId, string search, string? sort, string? dateL, string? dateG,
            string? type, int? status)
        {
            var sortFunc = SortFilterUtils.GetRequestSort(sort);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith("D");
            }
            Expression<Func<Request, bool>> dateLCond = dateL == null ?
                e => true
                : e => e.CreationDate <= DateTime.Parse(dateL);

            Expression<Func<Request, bool>> dateGCond = dateG == null ?
                e => true
                : e => e.CreationDate >= DateTime.Parse(dateG);

            Expression<Func<Request, bool>> typeCond = type == null ?
                e => true
                : e => e.ObjectType.ObjectTypeName == type;

            Expression<Func<Request, bool>> statusCond = status == null ?
                e => true
                : e => e.RequestStatusId == status;

            return await _handlerContext.Requests
                .Where(e => e.IdUserCreator == userId && e.Title.ToLower().Contains(search.ToLower()))
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(typeCond)
                .Where(statusCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(e => new GetRequest
                {
                    Id = e.RequestId,
                    Username = e.UserReciver.Username + " " + e.UserReciver.Surname,
                    Status = e.RequestStatus.StatusName,
                    ObjectType = e.ObjectType.ObjectTypeName,
                    CreationDate = e.CreationDate,
                    Title = e.Title
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetRequest>> GetReceivedRequests(int userId, string? sort, string? dateL, string? dateG,
            string? type, int? status)
        {
            var sortFunc = SortFilterUtils.GetRequestSort(sort);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith("D");
            }
            Expression<Func<Request, bool>> dateLCond = dateL == null ?
                e => true
                : e => e.CreationDate <= DateTime.Parse(dateL);

            Expression<Func<Request, bool>> dateGCond = dateG == null ?
                e => true
                : e => e.CreationDate >= DateTime.Parse(dateG);

            Expression<Func<Request, bool>> typeCond = type == null ?
                e => true
                : e => e.ObjectType.ObjectTypeName == type;

            Expression<Func<Request, bool>> statusCond = status == null ?
                e => true
                : e => e.RequestStatusId == status;

            return await _handlerContext.Requests
                .Where(e => e.IdUserReciver == userId)
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(typeCond)
                .Where(statusCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(e => new GetRequest
                {
                    Id = e.RequestId,
                    Username = e.UserCreator.Username + " " + e.UserCreator.Surname,
                    Status = e.RequestStatus.StatusName,
                    ObjectType = e.ObjectType.ObjectTypeName,
                    CreationDate = e.CreationDate,
                    Title = e.Title
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetRequest>> GetReceivedRequests(int userId, string search, string? sort, string? dateL, string? dateG,
            string? type, int? status)
        {
            var sortFunc = SortFilterUtils.GetRequestSort(sort);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith("D");
            }
            Expression<Func<Request, bool>> dateLCond = dateL == null ?
                e => true
                : e => e.CreationDate <= DateTime.Parse(dateL);

            Expression<Func<Request, bool>> dateGCond = dateG == null ?
                e => true
                : e => e.CreationDate >= DateTime.Parse(dateG);

            Expression<Func<Request, bool>> typeCond = type == null ?
                e => true
                : e => e.ObjectType.ObjectTypeName == type;

            Expression<Func<Request, bool>> statusCond = status == null ?
                e => true
                : e => e.RequestStatusId == status;
            return await _handlerContext.Requests
                .Where(e => e.IdUserReciver == userId && e.Title.ToLower().Contains(search.ToLower()))
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(typeCond)
                .Where(statusCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(e => new GetRequest
                {
                    Id = e.RequestId,
                    Username = e.UserCreator.Username + " " + e.UserCreator.Surname,
                    Status = e.RequestStatus.StatusName,
                    ObjectType = e.ObjectType.ObjectTypeName,
                    CreationDate = e.CreationDate,
                    Title = e.Title
                }).ToListAsync();
        }
        public async Task<GetRestRequest> GetRestRequest(int requestId)
        {
            return await _handlerContext.Requests
                .Where(e => e.RequestId == requestId)
                .Select(e => new GetRestRequest
                {
                    Path = e.FilePath,
                    Note = e.Note
                }).FirstAsync();
        }
        public async Task<bool> RequestExist(int requestId)
        {
            return await _handlerContext.Requests.AnyAsync(x => x.RequestId == requestId);
        }
        public async Task DeleteRequest(int requestId)
        {
            await _handlerContext.Requests.Where(e => e.RequestId == requestId).ExecuteDeleteAsync();
        }
        public async Task<bool> ModifyRequest(ModifyRequest data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                if (data.RecevierId != null)
                {
                    await _handlerContext.Requests
                        .Where(e => e.RequestId == data.RequestId)
                        .ExecuteUpdateAsync(setters =>
                            setters.SetProperty(s => s.IdUserReciver, data.RecevierId)
                        );
                }
                if (data.Title != null)
                {
                    await _handlerContext.Requests
                        .Where(e => e.RequestId == data.RequestId)
                        .ExecuteUpdateAsync(setters =>
                            setters.SetProperty(s => s.Title, data.Title)
                        );
                }
                if (data.ObjectType != null)
                {
                    var typeId = await _handlerContext.ObjectTypes
                        .Where(e => e.ObjectTypeName == data.ObjectType)
                        .Select(e => e.ObjectTypeId)
                        .FirstAsync();
                    await _handlerContext.Requests
                        .Where(e => e.RequestId == data.RequestId)
                        .ExecuteUpdateAsync(setters =>
                            setters.SetProperty(s => s.ObjectTypeId, typeId)
                        );
                }
                if (data.Note != null)
                {
                    await _handlerContext.Requests
                        .Where(e => e.RequestId == data.RequestId)
                        .ExecuteUpdateAsync(setters =>
                            setters.SetProperty(s => s.Note, data.Note)
                        );
                }
                if (data.Path != null)
                {
                    await _handlerContext.Requests
                        .Where(e => e.RequestId == data.RequestId)
                        .ExecuteUpdateAsync(setters =>
                            setters.SetProperty(s => s.FilePath, data.Path)
                        );
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
        public async Task<bool> SetRequestStatus(int requestId, int statusId, SetRequestStatus data)
        {
            var toAdd = $"\n[{data.StatusName}] {DateTime.Now:dd/MM/yyyy H:mm}";
            if (data.Note != null || data.Note != "")
            {
                toAdd += "\n" + data.Note;
            }
            var note = await _handlerContext.Requests
                .Where(e => e.RequestId == requestId)
                .Select(e => e.Note).FirstAsync();
            if (note.Length + toAdd.Length > 500)
            {
                return false;
            }
            await _handlerContext.Requests
                .Where(e => e.RequestId == requestId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(s => s.RequestStatusId, statusId)
                    .SetProperty(s => s.Note, s=> s.Note + toAdd)
                );
            await _handlerContext.SaveChangesAsync();
            return true;
        }
        public async Task<string?> GetRequestPath(int requestId)
        {
            return await _handlerContext.Requests
                .Where (e => e.RequestId == requestId)
                .Select(e => e.FilePath)
                .FirstAsync();
        }
        public async Task<int> GetReceiverId(int requestId)
        {
            return await _handlerContext.Requests
                .Where ((e) => e.RequestId == requestId)
                .Select(e => e.IdUserReciver)
                .FirstAsync();
        }
        public async Task<GetRestModifyRequest> GetRestModifyRequest(int requestId)
        {
            return await _handlerContext.Requests
                .Where(((e) => e.RequestId == requestId))
                .Select(e => new GetRestModifyRequest
                {
                    RecevierId = e.IdUserReciver,
                    Note = e.Note,
                }).FirstAsync();
        }
        public async Task<IEnumerable<GetRequestStatus>> GetRequestStatuses()
        {
            return await _handlerContext.RequestStatuses.Select(e => new GetRequestStatus
            {
                Id = e.RequestStatusId,
                Name = e.StatusName
            }).ToListAsync();
        }
    }
}
