using Kawerk.Application.Interfaces;
using Kawerk.Infastructure.DTOs.Manufacturer;
using Microsoft.AspNetCore.Mvc;

namespace Kawerk.API.Controllers
{
    [Route("api/v1/manufacturer")]
    public class ManufacturerController : Controller
    {
        private readonly IManufacturerService _manufacturerService;
        public ManufacturerController(IManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateManufacturer([FromBody] ManufacturerCreationDTO manufacturer)
        {
            var result = await _manufacturerService.CreateManufacturer(manufacturer);
            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(new { message = result.msg });
        }
        [HttpPut("update/{manufacturerID}")]
        public async Task<IActionResult> UpdateManufacturer([FromRoute]Guid manufacturerID, [FromBody] ManufacturerUpdateDTO manufacturer)
        {
            var result = await _manufacturerService.UpdateManufacturer(manufacturerID, manufacturer);
            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(new { message = result.msg });
        }
        [HttpDelete("delete/{manufacturerID}")]
        public async Task<IActionResult> DeleteManufacturer([FromRoute]Guid manufacturerID)
        {
            var result = await _manufacturerService.DeleteManufacturer(manufacturerID);

            if(result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(new { message = result.msg });
        }
        [HttpPost("sell-vehicle/{manufacturerID}/{vehicleID}")]
        public async Task<IActionResult> SellVehicle([FromRoute] Guid manufacturerID, [FromRoute] Guid vehicleID)
        {
            var result = await _manufacturerService.SellVehicle(manufacturerID, vehicleID);
            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(new { message = result.msg });
        }
        [HttpGet("get/{manufacturerID}")]
        public async Task<IActionResult> GetManufacturer([FromRoute] Guid manufacturerID)
        {
            var result = await _manufacturerService.GetManufacturer(manufacturerID);

            if (result == null)
                return NotFound(new { message = "not found" });
            else
                return Ok(result);
        }
        [HttpGet("get-sold-vehicles/{manufacturerID}")]
        public async Task<IActionResult> GetSoldVehicles([FromRoute] Guid manufacturerID,[FromQuery] string startDate, string endDate, string sortColumn, string OrderBy, string SearchTerm, int page = 1, int pageSize = 10)
        {
            var result = await _manufacturerService.GetSoldVehicles(manufacturerID, startDate, endDate, page, sortColumn, OrderBy, SearchTerm, pageSize);
            if (result == null)
                return NotFound(new { message = "not found" });
            else
                return Ok(result);
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetManufacturers(int page = 1, int pageSize = 10)
        {
            var result = await _manufacturerService.GetManufacturers(page, pageSize);

            if (result == null)
                return NotFound(new { message = "Empty Database" });
            else
                return Ok(result);
        }
    }
}
