using database_communicator.Data;
using database_communicator.Models;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using Microsoft.EntityFrameworkCore;

namespace database_communicator.Services
{
    public interface INotificationServices
    {
        public Task<int> CreateNotification(CreateNotification data);
        public Task<bool> SetIsRead(int notifId, bool isRead);
        public Task<IEnumerable<GetNotifications>> GetNotifications(int userId);
        public Task<bool> NotificationExists(int notifId);
    }
    /// <summary>
    /// Class that interact with database and contains functions allowing to work on notifications.
    /// </summary>
    public class NotificationServices : INotificationServices
    {
        private readonly HandlerContext _handlerContext;
        private readonly ILogger<NotificationServices> _logger;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handlerContext">Database context</param>
        /// <param name="logger">Log interface</param>
        public NotificationServices(HandlerContext handlerContext, ILogger<NotificationServices> logger)
        {
            _handlerContext = handlerContext;
            _logger = logger;
        }
        /// <summary>
        /// Using transactions add new notification to database.
        /// </summary>
        /// <param name="data">New notification value</param>
        /// <returns>True if success, false if failure</returns>
        public async Task<int> CreateNotification(CreateNotification data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var typeId = await _handlerContext.ObjectTypes.Where(e => e.ObjectTypeName == data.ObjectType).Select(e => e.ObjectTypeId).FirstAsync();
                var newNotif = new UserNotification
                {
                    UsersId = data.UserId,
                    Info = data.Info,
                    ObjectTypeId = typeId,
                    Referance = data.Referance,
                    IsRead = false
                };
                _handlerContext.Add<UserNotification>(newNotif);
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return newNotif.NotificationId;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Notification was not created");
                await trans.RollbackAsync();
                return -1;
            }
        }
        /// <summary>
        /// Modify notification isRead property to given one.
        /// </summary>
        /// <param name="notifId">Id of notification that property will change</param>
        /// <param name="isRead">New isRead value</param>
        /// <returns>True if success or false if failure</returns>
        public async Task<bool> SetIsRead(int notifId, bool isRead)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                await _handlerContext.UserNotifications
                .Where(e => e.NotificationId == notifId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(s => s.IsRead, isRead)
                );
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Change notification status error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Do select query to receive notifications information for given user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>List of object containing notification data.</returns>
        public async Task<IEnumerable<GetNotifications>> GetNotifications(int userId)
        {
            return await _handlerContext.UserNotifications
                .Where(e => e.UsersId == userId)
                .Select(e => new GetNotifications
                {
                    NotificationId = e.NotificationId,
                    Info = e.Info,
                    ObjectType = e.ObjectType.ObjectTypeName,
                    Reference = e.Referance,
                    IsRead = e.IsRead
                })
                .OrderBy(e => e.IsRead)
                .ToListAsync();
        }
        /// <summary>
        /// Checks if notification with this id exist.
        /// </summary>
        /// <param name="notifId">Notification id</param>
        /// <returns>True if exist, otherwise false.</returns>
        public async Task<bool> NotificationExists(int notifId)
        {
            return await _handlerContext.UserNotifications.Where(e => e.NotificationId == notifId).AnyAsync();
        }
    }
}
