using Kawerk.Application.Interfaces;
using Kawerk.Infastructure.DTOs.Vehicle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Kawerk.API.Controllers
{
    [Route("api/v1/vehicle")]
    public class VehicleController : Controller
    {
        private readonly IVehicleService _vehicleService;
        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateVehicle([FromBody] VehicleViewDTO vehicle)
        {
            var result = await _vehicleService.CreateVehicle(vehicle);
            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(new { message = result.msg });
        }

        // New endpoint: import CSV
        [HttpPost("import-csv")]
        public async Task<IActionResult> ImportCsv()
        {

            var result = await _vehicleService.ImportVehiclesFromCsv();
            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(new { message = result.msg });
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("delete-data")]
        public async Task<IActionResult> DeleteData()
        {
            var result = await _vehicleService.DeleteAllData();
            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(new { message = result.msg });
        }

        [HttpPut("update/{vehicleID}")]
        public async Task<IActionResult> UpdateVehicle([FromRoute] Guid vehicleID, [FromBody] VehicleViewDTO vehicle)
        {
            var result = await _vehicleService.UpdateVehicle(vehicleID, vehicle);

            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(new { message = result.msg });
        }
        [HttpDelete("delete/{vehicleID}")]
        public async Task<IActionResult> DeleteVehicle([FromRoute] Guid vehicleID)
        {
            var result = await _vehicleService.DeleteVehicle(vehicleID);

            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(new { message = result.msg });
        }
        [HttpGet("get/{vehicleID}")]
        public async Task<IActionResult> GetVehicle([FromRoute] Guid vehicleID)
        {
            var result = await _vehicleService.GetVehicle(vehicleID);

            return Ok(result);
        }
        [HttpGet("get-vehicles/")]
        public async Task<IActionResult> GetAllVehicles([FromQuery] string startDate, string endDate,  string sortColumn, string OrderBy, string SearchTerm, int minimumPrice = 0, int maximumPrice = 0, int page = 1, int pageSize = 10)
        {
            var result = await _vehicleService.GetFilteredVehicles(startDate, endDate, minimumPrice, maximumPrice, page, sortColumn, OrderBy, SearchTerm, pageSize);
            return Ok(result);
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetVehicles(int page = 1,int pageSize = 10)
        {
            var result = await _vehicleService.GetVehicles(page, pageSize);

            return Ok(result);
        }

    }
}
