using database_comunicator.Models;
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
        private readonly ILogServices _logServices;
        public WarehouseController(IUserServices userServices, IItemServices itemServices, ILogServices logServices)
        {
            _itemServices = itemServices;
            _userServices = userServices;
            _logServices = logServices;
        }

        [HttpPost]
        [Route("addItem")]
        public async Task<IActionResult> AddItem(AddItem newItem)
        {
            var userExist = await _userServices.UserExist(newItem.UserId);
            if (!userExist) return NotFound();
            var exist = await _itemServices.EanExist(newItem.Eans) || await _itemServices.ItemExist(newItem.PartNumber);
            if (exist)
            {
                return BadRequest();
            }

            var item = await _itemServices.AddItem(newItem);
            if (item == null) return BadRequest();
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"The item with id {item.ItemId} has been created, by user with id {newItem.UserId}.";
            await _logServices.CreateActionLog(desc, newItem.UserId, logId);

            return Ok();
        }
        [HttpDelete]
        [Route("deleteItem")]
        public async Task<IActionResult> DeleteItem(int itemId, int userId)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound();
            var exist = await _itemServices.ItemExist(itemId);
            if (!exist) return NotFound();

            await _itemServices.RemoveItem(itemId);
            var logId = await _logServices.getLogTypeId("Delete");
            var desc = $"The item with id {itemId} has been deleted, by user with id {userId}.";
            await _logServices.CreateActionLog(desc, userId, logId);

            return Ok();
        }
        [HttpPost]
        [Route("modifyItem")]
        public async Task<IActionResult> ModifyItem(UpdateItem newItem)
        {
            var userExist = await _userServices.UserExist(newItem.UserId);
            if (!userExist) return NotFound();
            var exist = await _itemServices.ItemExist(newItem.Id);
            if (!exist) return NotFound();
            var partNumberExist = newItem.PartNumber != null && await _itemServices.ItemExist(newItem.PartNumber);
            if (partNumberExist) return BadRequest();

            await _itemServices.UpdateItem(newItem);
            var logId = await _logServices.getLogTypeId("Modify");
            var werePartModify = newItem.PartNumber != null ? "partnumber, " : "";
            var wereDescModify = newItem.ItemDescription != null ? "description, " : "";
            var wereNameModify = newItem.ItemName != null ? "name" : "";
            var desc = $"The item with id {newItem.Id} has been modify, by user with id {newItem.UserId}. Modifications was made to: {werePartModify + wereDescModify + wereNameModify}";
            await _logServices.CreateActionLog(desc, newItem.UserId, logId);

            return Ok();
        }
        [HttpGet]
        [Route("getRestInfo")]
        public async Task<IActionResult> GetRestInfo(int itemId, string currency)
        {
            var exist = await _itemServices.ItemExist(itemId);
            if (!exist) return NotFound(itemId);
            var result = await _itemServices.GetRestOfItem(itemId, currency);
            return result != null ? Ok(result) : BadRequest();
        }
        [HttpGet]
        [Route("getRestOrgInfo")]
        public async Task<IActionResult> GetRestOrgInfo(int itemId, string currency)
        {
            var exist = await _itemServices.ItemExist(itemId);
            if (!exist) return NotFound(itemId);
            var result = await _itemServices.GetRestOfItemOrg(itemId, currency);
            return result != null ? Ok(result) : BadRequest();
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
        [HttpGet]
        [Route("description")]
        public async Task<IActionResult> GetDescription(int itemId)
        {
            var exist = await _itemServices.ItemExist(itemId);
            if (!exist) return NotFound(itemId);
            var desc = await _itemServices.GetDescription(itemId);
            return Ok(desc);
        }
        [HttpGet]
        [Route("bindings")]
        public async Task<IActionResult> GetBindings(int itemId, string currency)
        {
            var exist = await _itemServices.ItemExist(itemId);
            if (!exist) return NotFound(itemId);
            var desc = await _itemServices.GetModifyRestOfItem(itemId, currency);
            return Ok(desc);
        }
    }
}
