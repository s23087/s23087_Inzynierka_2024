﻿using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace database_comunicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryServices _deliveryService;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;
        private readonly INotificationServices _notificationServices;
        public DeliveryController(IDeliveryServices deliveryServices, IUserServices userServices, ILogServices logServices, INotificationServices notificationServices)
        {
            _deliveryService = deliveryServices;
            _userServices = userServices;
            _logServices = logServices;
            _notificationServices = notificationServices;
        }
        [HttpGet]
        [Route("get/user/{userId}")]
        public async Task<IActionResult> GetToUserDeliveries(int userId, string? search)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            if (search != null)
            {
                var sResult = await _deliveryService.GetDeliveries(true, userId, search);
                return Ok(sResult);
            }
            var result = await _deliveryService.GetDeliveries(true, userId);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/user")]
        public async Task<IActionResult> GetToUserDeliveries(string? search)
        {
            if (search != null)
            {
                var sResult = await _deliveryService.GetDeliveries(true, search);
                return Ok(sResult);
            }
            var result = await _deliveryService.GetDeliveries(true);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/client/{userId}")]
        public async Task<IActionResult> GetClientProformas(int userId, string? search)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            if (search != null)
            {
                var sResult = await _deliveryService.GetDeliveries(false, userId, search);
                return Ok(sResult);
            }
            var result = await _deliveryService.GetDeliveries(false, userId);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/client")]
        public async Task<IActionResult> GetClientProformas(string? search)
        {
            if (search != null)
            {
                var sResult = await _deliveryService.GetDeliveries(false, search);
                return Ok(sResult);
            }
            var result = await _deliveryService.GetDeliveries(false);
            return Ok(result);
        }
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddDelivery(AddDelivery data)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound("User not found.");
            var proExist = await _deliveryService.DeliveryProformaExist(data.ProformaId);
            if (proExist) return BadRequest("Delivery for this proforma already exist.");
            var deliveryId = await _deliveryService.AddDelivery(data);
            if (deliveryId == 0) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"User with id {data.UserId} has created the delivery with id {deliveryId}.";
            await _logServices.CreateActionLog(desc, data.UserId, logId);
            return Ok();
        }
        [HttpPost]
        [Route("company/add")]
        public async Task<IActionResult> AddDeliveryCompany(AddDeliveryComapny data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            var comapnyExist = await _deliveryService.DoesDeliveryCompanyExist(data.CompanyName);
            if (comapnyExist) return BadRequest("This company already exists.");
            await _deliveryService.AddDeliveryCompany(data.CompanyName);
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"User with id {userId} has created the delivery company with name {data.CompanyName}.";
            await _logServices.CreateActionLog(desc, userId, logId);
            return Ok();
        }
        [HttpGet]
        [Route("get/delivery_companies")]
        public async Task<IActionResult> GetDeliveryCompanies()
        {
            var result = await _deliveryService.GetDeliveryCompanies();
            return Ok(result);
        }
        [HttpGet]
        [Route("get/user/proformas/{userId}")]
        public async Task<IActionResult> GetUserProformas(int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            var result = await _deliveryService.GetProformaListWithoutDelivery(true, userId);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/client/proformas/{userId}")]
        public async Task<IActionResult> GetClientProformas(int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            var result = await _deliveryService.GetProformaListWithoutDelivery(false, userId);
            return Ok(result);
        }
        [HttpDelete]
        [Route("delete/{deliveryId}")]
        public async Task<IActionResult> DeleteDelivery(int deliveryId, int userId, bool isDeliveryToUser)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound("User not found.");
            var exist = await _deliveryService.DeliveryExist(deliveryId);
            if (!exist) return NotFound("Delivery not found.");
            var owner = await _deliveryService.GetDeliveryOwnerId(deliveryId);
            var result = await _deliveryService.DeleteDelivery(deliveryId);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Delete");
            var desc = $"User with id {userId} has deleted the delivery with number {deliveryId}.";
            await _logServices.CreateActionLog(desc, userId, logId);
            if (userId != owner)
            {
                var userFull = await _userServices.GetUserFullName(userId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = owner,
                    Info = $"{userFull} has deleted a delivery with id {deliveryId}.",
                    ObjectType = isDeliveryToUser ? "To user delivery" : "To client delivery",
                    Referance = $"{deliveryId}"
                });
            }
            return Ok();
        }
        [HttpGet]
        [Route("get/rest/{deliveryId}")]
        public async Task<IActionResult> GetRestDeliveryId(int deliveryId)
        {
            var exist = await _deliveryService.DeliveryExist(deliveryId);
            if (!exist) return NotFound("Delivery not found.");
            var result = await _deliveryService.GetRestDelivery(deliveryId);
            return Ok(result);
        }
        [HttpPost]
        [Route("note/add")]
        public async Task<IActionResult> AddNote(AddNote data)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound("User not found.");
            var deliveryExist = await _deliveryService.DeliveryExist(data.DeliveryId);
            if (!deliveryExist) return BadRequest("This delivery do not exists.");
            var result = await _deliveryService.AddNote(data);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return Ok();
        }
        [HttpPost]
        [Route("status/change")]
        public async Task<IActionResult> SetDeliveryStatus(SetDeliveryStatus data)
        {
            var deliveryExist = await _deliveryService.DeliveryExist(data.DeliveryId);
            if (!deliveryExist) return BadRequest("This delivery do not exists.");
            await _deliveryService.SetDeliveryStatus(data);
            return Ok();
        }
        [HttpPost]
        [Route("modify/{isDeliveryToUser}/{userId}")]
        public async Task<IActionResult> ModifyDelivery(ModifyDelivery data, int userId, bool isDeliveryToUser)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            var deliveryExist = await _deliveryService.DeliveryExist(data.DeliveryId);
            if (!deliveryExist) return NotFound("This delivery do not exists.");
            var owner = await _deliveryService.GetDeliveryOwnerId(data.DeliveryId);
            var result = await _deliveryService.ModifyDelivery(data);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Modify");
            var desc = $"User with id {userId} has modified the delivery with id {data.DeliveryId}.";
            await _logServices.CreateActionLog(desc, userId, logId);
            if (userId != owner)
            {
                var userFull = await _userServices.GetUserFullName(userId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = owner,
                    Info = $"{userFull} has modified a delivery with id {data.DeliveryId}.",
                    ObjectType = isDeliveryToUser ? "To user delivery" : "To client delivery",
                    Referance = $"{data.DeliveryId}"
                });
            }
            return Ok();
        }
    }
}
