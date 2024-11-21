using database_communicator.Data;
using database_communicator.Models;
using database_communicator.Utils;
using database_communicator.FilterClass;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using database_communicator.Models.DTOs.Modify;
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
        public Task<bool> DeleteRequest(int requestId);
        public Task<bool> ModifyRequest(ModifyRequest data);
        public Task<bool> SetRequestStatus(int requestId, int statusId, SetRequestStatus data);
        public Task<string?> GetRequestPath(int requestId);
        public Task<int> GetReceiverId(int requestId);
        public Task<GetRestModifyRequest> GetRestModifyRequest(int requestId);

        public Task<IEnumerable<GetRequestStatus>> GetRequestStatuses();
    }
    /// <summary>
    /// Class that interact with database and contains functions allowing to work on request.
    /// </summary>
    public class RequestServices : IRequestServices
    {
        private readonly HandlerContext _handlerContext;
        private readonly ILogger<CreditNoteServices> _logger;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handlerContext">Database context</param>
        /// <param name="logger">Log interface</param>
        public RequestServices(HandlerContext handlerContext, ILogger<CreditNoteServices> logger)
        {
            _handlerContext = handlerContext;
            _logger = logger;

        }
        /// <summary>
        /// Do select query to receive status id with given name.
        /// </summary>
        /// <param name="status">Status name.</param>
        /// <returns>Id of status or 0 if do not exist.</returns>
        public async Task<int> GetStatusId(string status)
        {
            return await _handlerContext.RequestStatuses
                .Where(e => e.StatusName == status)
                .Select(e => e.RequestStatusId)
                .FirstOrDefaultAsync();
        }
        /// <summary>
        /// Using transactions add new request to database.
        /// </summary>
        /// <param name="data">New request data</param>
        /// <returns>True if success, false if not.</returns>
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
                _logger.LogError(ex, "Create request error.");
                await trans.RollbackAsync();
                return 0;
            }
        }
        /// <summary>
        /// Do select query to receive sorted and filtered requests information that was created by given user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="dateL">Filter that search for date that is lower then given value.</param>
        /// <param name="dateG">Filter that search for date that is greater then given value.</param>
        /// <param name="type">Filter that search for type with given value</param>
        /// <param name="status">Filter that search for status with given value</param>
        /// <returns>List of request that was created by given user.</returns>
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
                direction = sort.StartsWith('D');
            }
            var dateLCond = RequestFilters.GetDateLowerFilter(dateL);
            var dateGCond = RequestFilters.GetDateGreaterFilter(dateG);
            var typeCond = RequestFilters.GetTypeFilter(type);
            var statusCond = RequestFilters.GetStatusFilter(status);

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
        /// <summary>
        /// Do select query to receive searched, sorted and filtered requests information that was created by given user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="search">The phrase searched in request information. It will check if phrase exist in title.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="dateL">Filter that search for date that is lower then given value.</param>
        /// <param name="dateG">Filter that search for date that is greater then given value.</param>
        /// <param name="type">Filter that search for type with given value</param>
        /// <param name="status">Filter that search for status with given value</param>
        /// <returns>List of request that was created by given user.</returns>
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
                direction = sort.StartsWith('D');
            }
            var dateLCond = RequestFilters.GetDateLowerFilter(dateL);
            var dateGCond = RequestFilters.GetDateGreaterFilter(dateG);
            var typeCond = RequestFilters.GetTypeFilter(type);
            var statusCond = RequestFilters.GetStatusFilter(status);

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
        /// <summary>
        /// Do select query to receive sorted and filtered requests information that was assigned to given user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="dateL">Filter that search for date that is lower then given value.</param>
        /// <param name="dateG">Filter that search for date that is greater then given value.</param>
        /// <param name="type">Filter that search for type with given value</param>
        /// <param name="status">Filter that search for status with given value</param>
        /// <returns>List of request that was assigned to give user.</returns>
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
                direction = sort.StartsWith('D');
            }
            var dateLCond = RequestFilters.GetDateLowerFilter(dateL);
            var dateGCond = RequestFilters.GetDateGreaterFilter(dateG);
            var typeCond = RequestFilters.GetTypeFilter(type);
            var statusCond = RequestFilters.GetStatusFilter(status);

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
        /// <summary>
        /// Do select query to receive searched, sorted and filtered requests information that was assigned to given user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="search">The phrase searched in request information. It will check if phrase exist in title.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="dateL">Filter that search for date that is lower then given value.</param>
        /// <param name="dateG">Filter that search for date that is greater then given value.</param>
        /// <param name="type">Filter that search for type with given value</param>
        /// <param name="status">Filter that search for status with given value</param>
        /// <returns>List of request that was assigned to given user.</returns>
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
                direction = sort.StartsWith('D');
            }
            var dateLCond = RequestFilters.GetDateLowerFilter(dateL);
            var dateGCond = RequestFilters.GetDateGreaterFilter(dateG);
            var typeCond = RequestFilters.GetTypeFilter(type);
            var statusCond = RequestFilters.GetStatusFilter(status);

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
        /// <summary>
        /// Do select query using given id to receive request information that was not given in bulk query.
        /// </summary>
        /// <param name="requestId">Request id.</param>
        /// <returns>Object containing request note and file path.</returns>
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
        /// <summary>
        /// Checks if request with given id exist.
        /// </summary>
        /// <param name="requestId">Request id.</param>
        /// <returns>True if exist, false if not.</returns>
        public async Task<bool> RequestExist(int requestId)
        {
            return await _handlerContext.Requests.AnyAsync(x => x.RequestId == requestId);
        }
        /// <summary>
        /// Using transactions delete given request from database.
        /// </summary>
        /// <param name="requestId">Id of request to delete.</param>
        /// <returns>True if success or false if not.</returns>
        public async Task<bool> DeleteRequest(int requestId)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                await _handlerContext.Requests.Where(e => e.RequestId == requestId).ExecuteDeleteAsync();
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Delete request error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Using transactions overwrite old request values with new ones.
        /// </summary>
        /// <param name="data">New request values.</param>
        /// <returns>True if succes, false if not.</returns>
        public async Task<bool> ModifyRequest(ModifyRequest data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                if (data.ReceiverId != null)
                {
                    await _handlerContext.Requests
                        .Where(e => e.RequestId == data.RequestId)
                        .ExecuteUpdateAsync(setters =>
                            setters.SetProperty(s => s.IdUserReciver, data.ReceiverId)
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
                _logger.LogError(ex, "Modify request error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Using transactions change given request status to new one. Also add new text to note if was given in data.
        /// </summary>
        /// <param name="requestId">Request id.</param>
        /// <param name="statusId">Id of new request status.</param>
        /// <param name="data">Additional data for adding text to note.</param>
        /// <returns>true if success or false if failure.</returns>
        public async Task<bool> SetRequestStatus(int requestId, int statusId, SetRequestStatus data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
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
                        .SetProperty(s => s.Note, s => s.Note + toAdd)
                    );
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Set request status error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Do select query to receive request file path.
        /// </summary>
        /// <param name="requestId">Request id.</param>
        /// <returns>String containing file path or null.</returns>
        public async Task<string?> GetRequestPath(int requestId)
        {
            return await _handlerContext.Requests
                .Where (e => e.RequestId == requestId)
                .Select(e => e.FilePath)
                .FirstOrDefaultAsync();
        }
        /// <summary>
        /// Do select query to receive id of user request was assigned to.
        /// </summary>
        /// <param name="requestId">Request id.</param>
        /// <returns>Id of user or 0 if request not found.</returns>
        public async Task<int> GetReceiverId(int requestId)
        {
            return await _handlerContext.Requests
                .Where ((e) => e.RequestId == requestId)
                .Select(e => e.IdUserReciver)
                .FirstOrDefaultAsync();
        }
        /// <summary>
        /// Do select query using given id to receive request information that was not given in bulk query and is needed for object modification.
        /// </summary>
        /// <param name="requestId">Request id.</param>
        /// <returns>Object containing receiver id and request note.</returns>
        public async Task<GetRestModifyRequest> GetRestModifyRequest(int requestId)
        {
            return await _handlerContext.Requests
                .Where(((e) => e.RequestId == requestId))
                .Select(e => new GetRestModifyRequest
                {
                    ReceiverId = e.IdUserReciver,
                    Note = e.Note,
                }).FirstAsync();
        }
        /// <summary>
        /// Do select query to receive list of request statuses.
        /// </summary>
        /// <returns>List of object containing id and name of request statuses.</returns>
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
