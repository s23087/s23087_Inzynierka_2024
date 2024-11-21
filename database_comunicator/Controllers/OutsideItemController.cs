using database_communicator.FilterClass;
using database_communicator.Models.DTOs.Create;
using database_communicator.Services;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator.Controllers
{
    /// <summary>
    /// This controller works on Outside items table and few other related tables. Used for receiving, modifying and creating data 
    /// that holds users outside items information. Use db_name parameter to pass the name of database that you want ot connect.
    /// </summary>
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class OutsideItemController(IOutsideItemServices outsideItemServices, IUserServices userServices, ILogServices logServices, INotificationServices notificationServices) : ControllerBase
    {
        private readonly IOutsideItemServices _outsideItemServices = outsideItemServices;
        private readonly IUserServices _userServices = userServices;
        private readonly ILogServices _logServices = logServices;
        private readonly INotificationServices _notificationServices = notificationServices;

        /// <summary>
        /// Tries to receive basic information about outside items.
        /// </summary>
        /// <param name="search">The phrase searched in items information. It will check if phrase exist in item name or partnumber.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="qtyL">Filter that search for qty that is lower then given value</param>
        /// <param name="qtyG">Filter that search for qty that is greater then given value</param>
        /// <param name="priceL">Filter that search for price that is lower then given value</param>
        /// <param name="priceG">Filter that search for price that is greater then given value</param>
        /// <param name="source">Filter that search for source with given value</param>
        /// <param name="currency">Filter that search for currency with given value</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetOutsideItem"/> or 400 when sort if given sort is incorrect.</returns>
        [HttpGet]
        [Route("get/items")]
        public async Task<IActionResult> GetOutsideItems(string? search, string? sort, int? qtyL, int? qtyG, int? priceL, int? priceG, int? source, string? currency)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest("Sort value is incorrect.");
            }
            var filters = new OutsideItemFiltersTemplate
            {
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
        /// <summary>
        /// Delete chosen item from database. This action will also create new log entry and notification.
        /// </summary>
        /// <param name="itemId">Id of item to delete</param>
        /// <param name="orgId">Id of organization that item belongs to</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code when success, 500 when failure or 404 when user or item is not found.</returns>
        [HttpDelete]
        [Route("delete/org/{orgId}/item/{itemId}/user/{userId}")]
        public async Task<IActionResult> DeleteOutsideItem(int itemId, int orgId, int userId)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound("User not found.");
            var exist = await _outsideItemServices.ItemExist(itemId, orgId);
            if (!exist) return NotFound("Outside item not found.");
            var owners = await _outsideItemServices.GetItemOwners(itemId, orgId);
            var result = await _outsideItemServices.DeleteItem(itemId, orgId);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Delete");
            var logDescription = $"User with id {userId} has deleted the outside item with id {itemId} and org id {orgId}.";
            await _logServices.CreateActionLog(logDescription, userId, logId);
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
        /// <summary>
        /// Add new outside items to database using given data. This action will also create new log entry and notifications. Notification will be created on start of import, error and success.
        /// </summary>
        /// <param name="data">New outside item data wrapped in <see cref="CreateOutsideItems"/> object.</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code when successful or 404 when user is not found</returns>
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
            var logId = await _logServices.getLogTypeId("Create");
            var logDescription = $"User with id {userId} has imported outside products.";
            await _logServices.CreateActionLog(logDescription, userId, logId);
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
            await _notificationServices.CreateNotification(new CreateNotification
            {
                UserId = userId,
                Info = $"Your import of outside items started {date:dd/MM/yyyy H:mm} has succeeded.",
                ObjectType = "User",
                Referance = $"{userId}"
            });
            return Ok();
        }
        /// <summary>
        /// Create new notification bound to user object. It's serves to pass information, when pipelining file data on web server failed.
        /// </summary>
        /// <param name="data">New pricelist data wrapped in <see cref="AddImportErrorNotification"/> object.</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code when success or 404 when user not found.</returns>
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
