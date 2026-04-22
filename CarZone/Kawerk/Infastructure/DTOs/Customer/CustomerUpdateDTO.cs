using System.ComponentModel.DataAnnotations.Schema;

namespace Kawerk.Infastructure.DTOs.Customer
{
    public class CustomerUpdateDTO
    {
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? ProfileUrl { get; set; }
    }
}
