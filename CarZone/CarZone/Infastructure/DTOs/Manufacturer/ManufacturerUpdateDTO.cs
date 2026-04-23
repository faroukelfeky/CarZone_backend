namespace Kawerk.Infastructure.DTOs.Manufacturer
{
    public class ManufacturerUpdateDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; } // Cars or motorcycles , etc
        public string? Warranty { get; set; }
    }
}
