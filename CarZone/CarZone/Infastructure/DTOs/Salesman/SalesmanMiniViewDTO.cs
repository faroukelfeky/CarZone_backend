namespace Kawerk.Infastructure.DTOs.Salesman
{
    public class SalesmanMiniViewDTO
    {
        public Guid SalesmanID { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public int Salary { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
