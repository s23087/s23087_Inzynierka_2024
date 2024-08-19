using database_comunicator.Data;
using database_comunicator.Models;
using Microsoft.EntityFrameworkCore;

namespace database_comunicator.Services
{
    public interface IRegistrationServices
    {
        public Task<IEnumerable<Country>> getCountriesNames();
    }
    public class RegistrationServices : IRegistrationServices
    {
        private readonly HandlerContext _handlerContext;
        public RegistrationServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }
        public async Task<IEnumerable<Country>> getCountriesNames()
        {
            return await _handlerContext.Countries.ToListAsync();
        }
    }
}
