using database_communicator.Data;
using database_communicator.Models;
using database_communicator.Models.DTOs;
using database_communicator.Models.DTOs.Get;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace database_communicator.Services
{
    public interface IRegistrationServices
    {
        public Task<IEnumerable<GetCountries>> GetCountries();
        public Task<bool> CreateNewDatabase(string orgName);
        public Task<bool> SetupDatabase();
    }
    /// <summary>
    /// Class that interact with database and contains functions allowing to work on register new organization and owner of this organization.
    /// </summary>
    public class RegistrationServices : IRegistrationServices
    {
        private readonly HandlerContext _handlerContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CreditNoteServices> _logger;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handlerContext">Database context</param>
        /// <param name="configuration">Configuration interface to recover some data from secrets</param>
        /// <param name="logger">Log interface</param>
        public RegistrationServices(HandlerContext handlerContext, IConfiguration configuration, ILogger<CreditNoteServices> logger)
        {
            _handlerContext = handlerContext;
            _configuration = configuration;
            _logger = logger;
        }
        /// <summary>
        /// Do select query to retrieve list of countries from database.
        /// </summary>
        /// <returns>List of objects containing id and name of countries</returns>
        public async Task<IEnumerable<GetCountries>> GetCountries()
        {
            return await _handlerContext.Countries.Select(e => new GetCountries
            {
                Id = e.CountryId,
                CountryName = e.CountryName
            }).ToListAsync();
        }
        /// <summary>
        /// Create new database from script.
        /// </summary>
        /// <param name="orgName">New database name that is also specially transformed organization name.</param>
        /// <returns>True if successfully created or false otherwise.</returns>
        public async Task<bool> CreateNewDatabase(string orgName)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                string createDbScript = await File.ReadAllTextAsync(_configuration["script:createDb"]);
                if (createDbScript == null)
                {
                    return false;
                }
                var formattedScript = FormattableStringFactory.Create(createDbScript.Replace("template", orgName));
                await _handlerContext.Database.ExecuteSqlAsync(formattedScript);
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Create database error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Set up new database from script.
        /// </summary>
        /// <returns>True if success or false otherwise.</returns>
        public async Task<bool> SetupDatabase()
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                string setupDbScript = await File.ReadAllTextAsync(_configuration["script:setupDb"]);
                if (setupDbScript == null)
                {
                    return false;
                }
                var formattedScript = FormattableStringFactory.Create(setupDbScript);
                await _handlerContext.Database.ExecuteSqlAsync(formattedScript);
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Setup error.");
                await trans.RollbackAsync();
                return false;
            }
        }
    }
}
