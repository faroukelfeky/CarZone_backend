using Kawerk.Application.Interfaces;
using Kawerk.Infastructure.DTOs.Customer;
using Microsoft.AspNetCore.Mvc;

namespace Kawerk.API.Controllers
{
    [Route("api/v1/customer")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerCreationDTO customer)
        {
            var result = await _customerService.CreateCustomer(customer);

            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(new { message = result.msg });
        }

        [HttpPut("update/{customerID}")]
        public async Task<IActionResult> UpdateCustomer([FromRoute] Guid customerID, [FromBody] CustomerUpdateDTO customer)
        {
            var result = await _customerService.UpdateCustomer(customerID, customer);

            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else if (result.status == 1)
                return Forbid();
            else
                return Ok(new { message = result.msg });

        }

        [HttpDelete("delete/{customerID}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] Guid customerID)
        {
            var result = await _customerService.DeleteCustomer(customerID);

            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else if (result.status == 1)
                return Forbid();
            else
                return Ok(new { message = result.msg });

        }

        [HttpPost("buy-vehicle/{customerID}/{vehicleID}")]
        public async Task<IActionResult> BuyVehicle([FromRoute] Guid customerID, [FromRoute] Guid vehicleID)
        {
            var result = await _customerService.BuyVehicle(customerID, vehicleID);
            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(new { message = result.msg });
        }
        [HttpPost("sell-vehicle/{customerID}/{vehicleID}")]
        public async Task<IActionResult> SellVehicle([FromRoute] Guid customerID, [FromRoute] Guid vehicleID)
        {
            var result = await _customerService.SellVehicle(customerID, vehicleID);
            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(new { message = result.msg });
        }
        [HttpPost("subscribe/{customerID}/{manufacturerID}")]
        public async Task<IActionResult> SubscribeToManufacturer([FromRoute] Guid customerID, [FromRoute] Guid manufacturerID)
        {
            var result = await _customerService.Subscribe(customerID, manufacturerID);
            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(new { message = result.msg });
        }
        [HttpGet("get/{customerID}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid customerID)
        {
            var result = await _customerService.GetCustomer(customerID);

            return Ok(result);
        }
        [HttpGet("get-search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string startDate, string endDate, string sortColumn, string OrderBy, string SearchTerm, int page = 1, int pageSize = 10)
        {
            var result = await _customerService.GetFilteredCustomers(startDate, endDate, page, sortColumn, OrderBy, SearchTerm, pageSize);
            if(result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(result.Data);
        }
        [HttpGet("get-bought-vehicles/{customerID}")]
        public async Task<IActionResult> GetUserVehicles([FromRoute] Guid customerID,[FromQuery] string startDate, string endDate, string sortColumn, string OrderBy, string SearchTerm, int page = 1, int pageSize = 10)
        {
            var result = await _customerService.GetBoughtVehicles(customerID, startDate, endDate, page, sortColumn, OrderBy, SearchTerm, pageSize);
            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(result.Data);
        }
        [HttpGet("get-sold-vehicles/{customerID}")]
        public async Task<IActionResult> GetSoldVehicles([FromRoute] Guid customerID,[FromQuery] string startDate, string endDate, string sortColumn, string OrderBy, string SearchTerm, int page = 1, int pageSize = 10)
        {
            var result = await _customerService.GetSoldVehicles(customerID, startDate, endDate, page, sortColumn, OrderBy, SearchTerm, pageSize);
            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(result.Data);
        }
        [HttpGet("get-subscribed-manufacturers/{customerID}")]
        public async Task<IActionResult> GetSubscribedManufacturers([FromRoute] Guid customerID, [FromQuery] int page = 1, int pageSize = 10)
        {
            var result = await _customerService.GetSubscribedManufacturers(customerID, page, pageSize);
            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(result.Data);
        }
        [HttpGet("get-notifications/{customerID}")]
        public async Task<IActionResult> GetNotifications([FromRoute] Guid customerID, [FromQuery] int page = 1, int pageSize = 10)
        {
            var result = await _customerService.GetNotifications(customerID, page, pageSize);
            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(result.Data);
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _customerService.GetCustomers(page, pageSize);
            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(result.Data);
        }
    }
}
