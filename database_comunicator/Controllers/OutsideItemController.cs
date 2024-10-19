using database_communicator.Models;
using database_communicator.Models.DTOs;
using database_communicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System;
using database_comunicator.FilterClass;

namespace database_communicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class OutsideItemController : ControllerBase
    {
        private readonly IOutsideItemServices _outsideItemServices;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;
        private readonly INotificationServices _notificationServices;
        public OutsideItemController(IOutsideItemServices outsideItemServices, IUserServices userServices, ILogServices logServices, INotificationServices notificationServices)
        {
            _logServices = logServices;
            _outsideItemServices = outsideItemServices;
            _notificationServices = notificationServices;
            _userServices = userServices;
        }
        [HttpGet]
        [Route("get/items")]
        public async Task<IActionResult> GetOutsideItems(string? search, string? sort, int? qtyL, int? qtyG, int? priceL, int? priceG, int? source, string? currency)
        {
            var filters = new OutsideItemFiltersTemplate { 
                QtyL = qtyL,
                QtyG = qtyG,
                PriceL = priceL,
                PriceG = priceG,
                Source = source,
                Currency = currency
            };
            if (search != null)
            {
                var sResult = await _outsideItemServices.GetItems(search, sort: sort, filters);
                return Ok(sResult);
            }
            var result = await _outsideItemServices.GetItems(sort: sort, filters);
            return Ok(result);
        }
        [HttpDelete]
        [Route("delete/org/{orgId}/item/{itemId}/user/{userId}")]
        public async Task<IActionResult> DeleteOutsideItem(int itemId, int orgId, int userId)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound("User not found.");
            var exist = await _outsideItemServices.ItemExist(itemId, orgId);
            if (!exist) return NotFound("Outside item not found.");
            var owners = await _outsideItemServices.GetItemOwners(itemId, orgId);
            await _outsideItemServices.DeleteItem(itemId, orgId);
            var logId = await _logServices.getLogTypeId("Delete");
            var desc = $"User with id {userId} has deleted the outside item with id {itemId} and org id {orgId}.";
            await _logServices.CreateActionLog(desc, userId, logId);
            var userFull = await _userServices.GetUserFullName(userId);
            foreach (var user in owners)
            {
                if (user == userId) continue;
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = user,
                    Info = $"{userFull} has deleted a outside item with id {itemId} and org id {orgId}.",
                    ObjectType = "Item",
                    Referance = $"{itemId}"
                });

            }
            return Ok();
        }
        [HttpPost]
        [Route("add/{userId}")]
        public async Task<IActionResult> AddOutsideItems(CreateOutsideItems data, int userId)
        {
            var date = DateTime.Now;
            await _notificationServices.CreateNotification(new CreateNotification
            {
                UserId = userId,
                Info = $"Your import of outside items has started {date:dd/MM/yyyy H:mm}.",
                ObjectType = "User",
                Referance = $"{userId}"
            });
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            var errorItems = await _outsideItemServices.AddItems(data);
            if (errorItems.Count() == data.Items.Count())
            {
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = userId,
                    Info = $"Your import of outside items started {date:dd/MM/yyyy H:mm} has failed.",
                    ObjectType = "User",
                    Referance = $"{userId}"
                });
                return Ok();
            }
            if (errorItems.Any())
            {
                var message = $"Your import of outside items started {date:dd/MM/yyyy H:mm} has succeeded with errors. Errors: " + String.Join(", ", errorItems);
                if (message.Length > 250)
                {
                    message = string.Concat(message.AsSpan(0, 247), "...");
                }

                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = userId,
                    Info = message,
                    ObjectType = "User",
                    Referance = $"{userId}"
                });
                return Ok();
            }
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"User with id {userId} has imported outside products.";
            await _logServices.CreateActionLog(desc, userId, logId);
            await _notificationServices.CreateNotification(new CreateNotification
            {
                UserId = userId,
                Info = $"Your import of outside items started {date:dd/MM/yyyy H:mm} has succeeded.",
                ObjectType = "User",
                Referance = $"{userId}"
            });
            return Ok();
        }
        [HttpPost]
        [Route("add/error/notification/{userId}")]
        public async Task<IActionResult> AddNotification(AddImportErrorNotification data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            await _notificationServices.CreateNotification(new CreateNotification
            {
                UserId = userId,
                Info = data.Info,
                ObjectType = "User",
                Referance = $"{userId}"
            });
            return Ok();
        }
    }
}
