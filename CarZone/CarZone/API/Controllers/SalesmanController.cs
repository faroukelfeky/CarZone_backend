using Kawerk.Application.Interfaces;
using Kawerk.Infastructure.DTOs.Salesman;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawerk.API.Controllers
{
    [Authorize]
    [Route("api/v1/salesman")]
    public class SalesmanController : Controller
    {
        private readonly ISalesmanService _salesmanService;
        public SalesmanController(ISalesmanService salesmanService)
        {
            _salesmanService = salesmanService;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateSalesman([FromBody] SalesmanCreationDTO salesman)
        {
            var result = await _salesmanService.CreateSalesman(salesman);

            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(new { message = result.msg });
        }
        [HttpPut("update/{salesmanID}")]
        public async Task<IActionResult> UpdateSalesman([FromRoute] Guid salesmanID, [FromBody] SalesmanUpdateDTO salesman)
        {
            var result = await _salesmanService.UpdateSalesman(salesmanID, salesman);
            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(new { message = result.msg });
        }
        [HttpDelete("delete/{salesmanID}")]
        public async Task<IActionResult> DeleteSalesman([FromRoute] Guid salesmanID)
        {
            var result = await _salesmanService.DeleteSalesman(salesmanID);

            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(new { message = result.msg });
        }
        [HttpGet("get/{salesmanID}")]
        public async Task<IActionResult> GetSalesman([FromRoute] Guid salesmanID)
        {
            var result = await _salesmanService.GetSalesman(salesmanID);
            if (result == null)
                return BadRequest(new { message = "Salesman not found" });
            else
                return Ok(result);
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetSalesmen([FromQuery] int page = 1,int pageSize = 10)
        {
            var result = await _salesmanService.GetSalesmen(page, pageSize);
            if (result == null)
                return BadRequest(new { message = "Salesman not found" });
            else
                return Ok(result);
        }
    }
}
