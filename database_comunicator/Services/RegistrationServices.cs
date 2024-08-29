using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace database_comunicator.Services
{
    public interface IRegistrationServices
    {
        public Task<IEnumerable<Country>> getCountries();
        public Task<bool> CreateNewDatabase(string orgName);
        public Task<bool> SetupDatabase();
    }
    public class RegistrationServices : IRegistrationServices
    {
        private readonly HandlerContext _handlerContext;
        private readonly IConfiguration _configuration;
        public RegistrationServices(HandlerContext handlerContext, IConfiguration configuration)
        {
            _handlerContext = handlerContext;
            _configuration = configuration;
        }
        public async Task<IEnumerable<Country>> getCountries()
        {
            return await _handlerContext.Countries.ToListAsync();
        }

        public async Task<bool> CreateNewDatabase(string orgName)
        {
            string createDbScript = await File.ReadAllTextAsync(_configuration["script:createDb"]);

            if (createDbScript == null)
            {
                return false;
            }

            var formatedScript = FormattableStringFactory.Create(createDbScript.Replace("template", orgName));

            await _handlerContext.Database.ExecuteSqlAsync(formatedScript);

            return true;
        }

        public async Task<bool> SetupDatabase()
        {
            string setupDbScript = await File.ReadAllTextAsync(_configuration["script:setupDb"]);

            if (setupDbScript == null)
            {
                return false;
            }

            var formatedScript = FormattableStringFactory.Create(setupDbScript);

            await _handlerContext.Database.ExecuteSqlAsync(formatedScript);
            await _handlerContext.SaveChangesAsync();

            return true;
        }
    }
}
