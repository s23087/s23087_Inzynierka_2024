using database_communicator.FilterClass;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator.Controllers
{
    /// <summary>
    /// This controller works on Item table and few other related tables. Used for receiving, modifying and creating data 
    /// that holds items information. Use db_name parameter to pass the name of database that you want ot connect.
    /// </summary>
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class WarehouseController(IUserServices userServices, IItemServices itemServices, ILogServices logServices, INotificationServices notificationServices) : ControllerBase
    {
        private readonly IUserServices _userServices = userServices;
        private readonly IItemServices _itemServices = itemServices;
        private readonly ILogServices _logServices = logServices;
        private readonly INotificationServices _notificationServices = notificationServices;

        /// <summary>
        /// Add new item to database. This action will also create new log entry.
        /// </summary>
        /// <param name="newItem">New item data wrapped in <see cref="Models.DTOs.Create.AddItem"/> object.</param>
        /// <returns>200 when success, 500 when failure, 404 when user not found or 400 when item with that ean or partnumber already exist.</returns>
        [HttpPost]
        [Route("add/item")]
        public async Task<IActionResult> AddItem(AddItem newItem)
        {
            var userExist = await _userServices.UserExist(newItem.UserId);
            if (!userExist) return NotFound("User not found.");
            var exist = await _itemServices.EanExist(newItem.Eans) || await _itemServices.ItemExist(newItem.PartNumber);
            if (exist)
            {
                return BadRequest("Item with this part number or ean already exists.");
            }

            var item = await _itemServices.AddItem(newItem);
            if (item == null) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Create");
            var logDescription = $"The item with id {item} has been created, by user with id {newItem.UserId}.";
            await _logServices.CreateActionLog(logDescription, newItem.UserId, logId);
            return Ok();
        }
        /// <summary>
        /// Delete chosen item from database.
        /// </summary>
        /// <param name="itemId">Id of item to delete</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code when success, 500 when failure, 404 when item or user not found or 400 when item have relations.</returns>
        [HttpDelete]
        [Route("delete/item/{itemId}/{userId}")]
        public async Task<IActionResult> DeleteItem(int itemId, int userId)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound("User not found.");
            var exist = await _itemServices.ItemExist(itemId);
            if (!exist) return NotFound("Item not found.");
            var relationExist = await _itemServices.ItemHaveRelations(itemId);
            if (relationExist) return BadRequest("This item is included in other objects like invoice, proforma or offer.");
            var result = await _itemServices.RemoveItem(itemId);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Delete");
            var logDescription = $"The item with id {itemId} has been deleted, by user with id {userId}.";
            await _logServices.CreateActionLog(logDescription, userId, logId);

            return Ok();
        }
        /// <summary>
        /// Overwrite item data with new one. This action will also create new log entry.
        /// </summary>
        /// <param name="newItem">New item data wrapped in <see cref="UpdateItem"/> object.</param>
        /// <returns>200 code when success, 500 when failure, 404 when user or item is not found or 400 when new partnumber already exist.</returns>
        [HttpPost]
        [Route("modify")]
        public async Task<IActionResult> ModifyItem(UpdateItem newItem)
        {
            var userExist = await _userServices.UserExist(newItem.UserId);
            if (!userExist) return NotFound();
            var exist = await _itemServices.ItemExist(newItem.Id);
            if (!exist) return NotFound();
            var partNumberExist = newItem.PartNumber != null && await _itemServices.ItemExist(newItem.PartNumber);
            if (partNumberExist) return BadRequest();

            var result = await _itemServices.UpdateItem(newItem);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Modify");
            var werePartModify = newItem.PartNumber != null ? "partnumber, " : "";
            var wereDescModify = newItem.ItemDescription != null ? "description, " : "";
            var wereNameModify = newItem.ItemName != null ? "name" : "";
            var logDescription = $"The item with id {newItem.Id} has been modify, by user with id {newItem.UserId}. Modifications was made to: {werePartModify + wereDescModify + wereNameModify}";
            await _logServices.CreateActionLog(logDescription, newItem.UserId, logId);
            return Ok();
        }
        /// <summary>
        /// Tries to receive information that was not passed as basic information in <see cref="GetItems"/> 
        /// function and are needed to showcase object for chosen user.
        /// </summary>
        /// <param name="itemId">Id of item to showcase</param>
        /// <param name="userId">Id of user.</param>
        /// <param name="currency">Shortcut of currency that item price will be showed.</param>
        /// <returns>200 code with the object of <see cref="Models.DTOs.Get.GetRestInfo"/> or 404 when user or item does not exist.</returns>
        [HttpGet]
        [Route("get/rest/{itemId}/{currency}/user/{userId}")]
        public async Task<IActionResult> GetRestInfo(int itemId, int userId, string currency)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound();
            var exist = await _itemServices.ItemExist(itemId);
            if (!exist) return NotFound();
            var result = await _itemServices.GetRestOfItem(itemId, userId, currency);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information that was not passed as basic information in <see cref="GetItems"/> 
        /// function and are needed to showcase object for user.
        /// </summary>
        /// <param name="itemId">Id of item to showcase</param>
        /// <param name="currency">Shortcut of currency that item price will be showed.</param>
        /// <returns>200 code with the object of <see cref="Models.DTOs.Get.GetRestInfo"/> or 404 when user or item does not exist.</returns>
        [HttpGet]
        [Route("get/rest/org/{itemId}/{currency}")]
        public async Task<IActionResult> GetRestOrgInfo(int itemId, string currency)
        {
            var exist = await _itemServices.ItemExist(itemId);
            if (!exist) return NotFound();
            var result = await _itemServices.GetRestOfItemOrg(itemId, currency);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive basic information about items.
        /// </summary>
        /// <param name="currency">Shortcut of currency that item price will be showed.</param>
        /// <param name="userId">User id that data is specially filtered on.</param>
        /// <param name="search">The phrase searched in items information. It will check if phrase exist in partnumber or item name.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="status">Filter that search for status with given value</param>
        /// <param name="ean">Filter that search for ean with given value</param>
        /// <param name="qtyL">Filter that search for qty that is lower then given value</param>
        /// <param name="qtyG">Filter that search for qty that is greater then given value</param>
        /// <param name="priceL">Filter that search for price that is lower then given value</param>
        /// <param name="priceG">Filter that search for price that is greater then given value</param>
        /// <returns>200 code with list of <see cref="GetManyItems"/>, 400 when sort if given sort is incorrect or 404 when user do not exist.</returns>
        [HttpGet]
        [Route("get/{currency}")]
        public async Task<IActionResult> GetItems(string currency, int? userId, string? search, string? sort, string? status, string? ean,
            int? qtyL, int? qtyG, int? priceL, int? priceG)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest("Sort value is incorrect.");
            }

            IEnumerable<GetManyItems> result;
            var filters = new ItemFiltersTemplate
            {
                Status = status,
                Ean = ean,
                QtyL = qtyL,
                QtyG = qtyG,
                PriceL = priceL,
                PriceG = priceG
            };
            if (userId != null)
            {
                int correctUser = (int)userId;
                var userExist = await _userServices.UserExist(correctUser);
                if (!userExist) return NotFound();
                if (search != null)
                {
                    string correctSearch = search;
                    result = await _itemServices.GetItems(currency, correctUser, correctSearch, orderBy: sort, filters);
                }
                else
                {
                    result = await _itemServices.GetItems(currency, correctUser, orderBy: sort, filters);
                }
                return Ok(result);
            }
            if (search != null)
            {
                string correctSearch = search;
                result = await _itemServices.GetItems(currency, correctSearch, orderBy: sort, filters);
            }
            else
            {
                result = await _itemServices.GetItems(currency, orderBy: sort, filters);
            }
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive item description from database.
        /// </summary>
        /// <param name="itemId">Id of item.</param>
        /// <returns>200 code with string containing description or 404 when item not found.</returns>
        [HttpGet]
        [Route("get/description/{itemId}")]
        public async Task<IActionResult> GetDescription(int itemId)
        {
            var exist = await _itemServices.ItemExist(itemId);
            if (!exist) return NotFound();
            var logDescription = await _itemServices.GetDescription(itemId);
            return Ok(logDescription);
        }
        /// <summary>
        /// Tries to receive item bindings information from database.
        /// </summary>
        /// <param name="itemId">Id of item.</param>
        /// <param name="currency">Shortcut name of currency.</param>
        /// <returns>200 code with list of <see cref="GetBinding"/> or 404 when item is not found.</returns>
        [HttpGet]
        [Route("get/bindings/{itemId}/{currency}")]
        public async Task<IActionResult> GetBindings(int itemId, string currency)
        {
            var exist = await _itemServices.ItemExist(itemId);
            if (!exist) return NotFound();
            var logDescription = await _itemServices.GetModifyRestOfItem(itemId, currency);
            return Ok(logDescription);
        }
        /// <summary>
        /// Tries to receive list of item owners from database.
        /// </summary>
        /// <param name="itemId">Id of item.</param>
        /// <returns>200 code with list of <see cref="GetUsers"/> or 404 when item is not found.</returns>
        [HttpGet]
        [Route("get/item_owners/{itemId}")]
        public async Task<IActionResult> GetItemOwners(int itemId)
        {
            var exist = await _itemServices.ItemExist(itemId);
            if (!exist) return NotFound();
            var result = await _itemServices.GetItemOwners(itemId);
            return Ok(result);
        }
        /// <summary>
        /// Overwrite Overwrite old item bindings with new ones. This action will also create new log entry. Notification will be created for each user associated with this action and is not the user activating it.
        /// </summary>
        /// <param name="data">New bindings data wrapped in <see cref="Models.DTOs.Modify.ChangeBindings"/> object.</param>
        /// <returns>200 code when success, 500 whe failure or 404 when user is not found.</returns>
        [HttpPost]
        [Route("modify/bindings")]
        public async Task<IActionResult> ChangeBindings(ChangeBindings data)
        {
            var userExist = await _userServices.UserExist(data.UserId);
            if (!userExist) return NotFound();
            var result = await _itemServices.ChangeBindings(data.Bindings);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Modify");
            var logDescription = $"The bindings for item with id {data.Bindings.Select(e => e.ItemId).First()} has been modified, by user with id {data.UserId}.";
            await _logServices.CreateActionLog(logDescription, data.UserId, logId);
            var userFull = await _userServices.GetUserFullName(data.UserId);
            foreach (var user in data.Bindings.GroupBy(e => e.UserId).Select(e => e.Key))
            {
                if (user == data.UserId)
                {
                    continue;
                }
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = user,
                    Info = $"The item with id {data.Bindings.Select(e => e.ItemId).First()} has been bound to you by {userFull}.",
                    ObjectType = "Item",
                    Referance = $"{data.Bindings.Select(e => e.ItemId).First()}"
                });
            }
            return Ok();
        }
    }
}
