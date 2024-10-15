using database_communicator.Data;
using database_communicator.Models;
using database_communicator.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace database_communicator.Services
{
    public interface INotificationServices
    {
        public Task<int> GetObjectTypeId(string name);
        public Task<int> CreateNotification(CreateNotification data);
        public Task SetIsRead(int notifId,bool isRead);
        public Task<IEnumerable<GetNotifications>> GetNotifications(int userId);
        public Task<bool> NotifExists(int notifId);
    }
    public class NotificationServices : INotificationServices
    {
        private readonly HandlerContext _handlerContext;
        public NotificationServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }
        public async Task<int> GetObjectTypeId(string name)
        {
            return await _handlerContext.ObjectTypes.Where(e => e.ObjectTypeName == name).Select(e => e.ObjectTypeId).FirstAsync();
        }
        public async Task<int> CreateNotification(CreateNotification data)
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
            return newNotif.NotificationId;
        }
        public async Task SetIsRead(int notifId, bool isRead)
        {
            await _handlerContext.UserNotifications
                .Where(e => e.NotificationId == notifId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(s => s.IsRead, isRead)
                );
        }
        public async Task<IEnumerable<GetNotifications>> GetNotifications(int userId)
        {
            return await _handlerContext.UserNotifications
                .Where(e => e.UsersId == userId)
                .Include(e => e.ObjectType)
                .Select(e => new GetNotifications
                {
                    NotificationId = e.NotificationId,
                    Info = e.Info,
                    ObjectType = e.ObjectType.ObjectTypeName,
                    Referance = e.Referance,
                    IsRead = e.IsRead
                })
                .OrderBy(e => e.IsRead)
                .ToListAsync();
        }
        public async Task<bool> NotifExists(int notifId)
        {
            return await _handlerContext.UserNotifications.Where(e => e.NotificationId == notifId).AnyAsync();
        }
    }
}
