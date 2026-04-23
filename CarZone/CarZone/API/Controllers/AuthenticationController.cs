using Kawerk.Application.Interfaces;
using Kawerk.Application.Services;
using Kawerk.Infastructure.DTOs.Customer;
using Microsoft.AspNetCore.Mvc;

namespace Kawerk.API.Controllers
{
    [Route("api/v1/auth")]
    public class AuthenticationController : Controller
    {
        private readonly ICustomerService _customerService;
        public AuthenticationController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [HttpPost("sign-up")]
        public async Task<IActionResult> Signup([FromBody] CustomerCreationDTO customer)
        {
            var result = await _customerService.CreateCustomer(customer);

            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(result);
        }
        [HttpGet("sign-in")]
        public async Task<IActionResult> Signin([FromQuery] string email, [FromQuery] string password)
        {
            var result = await _customerService.Login(email, password);
            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(result);
        }
        [HttpPost("sign-up-admin")]
        public async Task<IActionResult> SignUpAdming([FromBody] CustomerCreationDTO customer)
        {
            var result = await _customerService.CreateAdmin(customer);

            if (result.status == 0)
                return BadRequest(new { message = result.msg });
            else
                return Ok(result);
        }
    }
}
