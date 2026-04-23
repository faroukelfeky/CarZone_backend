using System.ComponentModel.DataAnnotations.Schema;

namespace Kawerk.Infastructure.DTOs.Salesman
{
    public class SalesmanCreationDTO
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
