using database_comunicator.Models.DTOs;
using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace database_comunicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IItemServices _itemServices;
        public WarehouseController(IUserServices userServices, IItemServices itemServices)
        {
            _itemServices = itemServices;
            _userServices = userServices;
        }

        [HttpPost]
        [Route("addItem")]
        public async Task<IActionResult> AddItem(AddItem newItem)
        {
            var exist = await _itemServices.EanExist(newItem.Eans) || await _itemServices.ItemExist(newItem.PartNumber);
            if (exist)
            {
                return BadRequest();
            }

            var item = await _itemServices.AddItem(newItem);

            return item != null ? Ok() : BadRequest();
        }
        [HttpGet]
        [Route("items")]
        public async Task<IActionResult> GetItems(string currency, int? userId, string? search)
        {
            IEnumerable<GetManyItems> result;
            if (userId != null)
            {
                int correctUser = (int)userId;
                var userExist = await _userServices.UserExist(correctUser);
                if (!userExist) return BadRequest();
                if (search != null)
                {
                    string correctSearch = search;
                    result = await _itemServices.GetItems(currency, correctUser, correctSearch);
                } else
                {
                    result = await _itemServices.GetItems(currency, correctUser);
                }
                return Ok(result);
            }
            if (search != null)
            {
                string correctSearch = search;
                result = await _itemServices.GetItems(currency, correctSearch);
            }
            else
            {
                result = await _itemServices.GetItems(currency);
            }
            return Ok(result);
        }
    }
}
