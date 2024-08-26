using database_comunicator.Data;
using database_comunicator.Models;
using Microsoft.EntityFrameworkCore;

namespace database_comunicator.Services
{
    public interface ILogServices
    {
        public Task<int> getLogTypeId(string name);
        public Task CreateActionLog(string description, int userId, int typeId);
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

            await _handlerContext.AddAsync<ActionLog>(newLog);
            await _handlerContext.SaveChangesAsync();
        }

        public async Task<int> getLogTypeId(string name)
        {
            var result = await _handlerContext.LogTypes.Where(e => e.LogTypeName.Equals(name)).Select(e => e.LogTypeId).ToListAsync();
            return result[0];
        }
    }
}
