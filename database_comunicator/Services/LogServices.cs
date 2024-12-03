using database_communicator.Data;
using database_communicator.Models;
using database_communicator.Models.DTOs.Get;
using Microsoft.EntityFrameworkCore;

namespace database_communicator.Services
{
    public interface ILogServices
    {
        public Task<int> getLogTypeId(string name);
        public Task CreateActionLog(string description, int userId, int typeId);
        public Task<IEnumerable<GetLogs>> GetLogs();
    }
    /// <summary>
    /// Class that interact with database and contains functions allowing to work on logs.
    /// </summary>
    public class LogServices : ILogServices
    {
        private readonly HandlerContext _handlerContext;
        private readonly ILogger<LogServices> _logger;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handlerContext">Database context</param>
        /// <param name="logger">Log interface</param>
        public LogServices(HandlerContext handlerContext, ILogger<LogServices> logger)
        {
            _handlerContext = handlerContext;
            _logger = logger;
        }
        /// <summary>
        /// Using transactions add new log to database.
        /// </summary>
        /// <param name="description">Log description</param>
        /// <param name="userId">Id of user that generated this log</param>
        /// <param name="typeId">Id of log type that will be created</param>
        /// <returns>Void, if error occurred it will be logged in files using logger.</returns>
        public async Task CreateActionLog(string description, int userId, int typeId)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
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
                await trans.CommitAsync();
            } catch (Exception ex)
            {
                _logger.LogCritical(ex, "Action log was not created.");
                await trans.RollbackAsync();
            }
        }
        /// <summary>
        /// Do select query to receive id of given log type.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<int> getLogTypeId(string name)
        {
            return await _handlerContext.LogTypes.Where(e => e.LogTypeName.Equals(name)).Select(e => e.LogTypeId).FirstAsync();
        }
        /// <summary>
        /// Do select query to download all the logs from database.
        /// </summary>
        /// <returns>List of object describing logs.</returns>
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
