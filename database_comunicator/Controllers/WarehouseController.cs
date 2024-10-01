﻿using database_comunicator.Models;
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
        private readonly INotificationServices _notificationServices;
        public WarehouseController(IUserServices userServices, IItemServices itemServices, ILogServices logServices, INotificationServices notificationServices)
        {
            _itemServices = itemServices;
            _userServices = userServices;
            _logServices = logServices;
            _notificationServices = notificationServices;
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
        public async Task<IActionResult> GetRestInfo(int itemId, int userId, string currency)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound();
            var exist = await _itemServices.ItemExist(itemId);
            if (!exist) return NotFound(itemId);
            var result = await _itemServices.GetRestOfItem(itemId, userId, currency);
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
        [HttpGet]
        [Route("getItemOwners/{itemId}")]
        public async Task<IActionResult> GetItemOwners(int itemId)
        {
            var exist = await _itemServices.ItemExist(itemId);
            if (!exist) return NotFound(itemId);
            var result = await _itemServices.GetItemOwners(itemId);
            return Ok(result);
        }
        [HttpPost]
        [Route("changeBindings")]
        public async Task<IActionResult> ChangeBindings(ChangeBindings data)
        {
            var userExist = await _userServices.UserExist(data.UserId);
            if (!userExist) return NotFound();
            var result = await _itemServices.ChangeBindings(data.Bindings);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Modify");
            var desc = $"The bindings for item with id {data.Bindings.Select(e => e.ItemId).First()} has been modified, by user with id {data.UserId}.";
            await _logServices.CreateActionLog(desc, data.UserId, logId);
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
                    Info = $"The item with id {data.Bindings.Select(e => e.ItemId).First()} has been binded to you by {userFull}.",
                    ObjectType = "Item",
                    Referance = $"{data.Bindings.Select(e => e.ItemId).First()}"
                });
            }
            return Ok();
        }
    }
}
