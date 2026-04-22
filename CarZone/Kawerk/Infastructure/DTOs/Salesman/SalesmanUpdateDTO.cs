using System.ComponentModel.DataAnnotations.Schema;

namespace Kawerk.Infastructure.DTOs.Salesman
{
    public class SalesmanUpdateDTO
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public int Salary { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
