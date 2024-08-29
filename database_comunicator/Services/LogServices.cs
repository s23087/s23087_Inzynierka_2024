using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace database_comunicator.Services
{
    public interface ILogServices
    {
        public Task<int> getLogTypeId(string name);
        public Task CreateActionLog(string description, int userId, int typeId);
        public Task<IEnumerable<GetLogs>> GetLogs();
    }
    public class LogServices : ILogServices
    {
        private readonly HandlerContext _handlerContext;
        public LogServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }

        public async Task CreateActionLog(string description, int userId, int typeId)
        {
            var newLog = new ActionLog
            {
                LogDescription = description,
                LogDate = DateTime.Now,
                UsersId = userId,
                LogTypeId = typeId
            };

            _handlerContext.Add<ActionLog>(newLog);
            await _handlerContext.SaveChangesAsync();
        }

        public async Task<int> getLogTypeId(string name)
        {
            var result = await _handlerContext.LogTypes.Where(e => e.LogTypeName.Equals(name)).Select(e => e.LogTypeId).ToListAsync();
            return result[0];
        }

        public async Task<IEnumerable<GetLogs>> GetLogs()
        {
            return await _handlerContext.ActionLogs
                .Include(e => e.LogType)
                .Include(e => e.Users)
                .Select(e => new GetLogs
                {
                    LogType = e.LogType.LogTypeName,
                    LogDescription = e.LogDescription,
                    LogDate = e.LogDate,
                    Surname = e.Users.Surname,
                    Username = e.Users.Username
                }).ToListAsync();
        }
    }
}
