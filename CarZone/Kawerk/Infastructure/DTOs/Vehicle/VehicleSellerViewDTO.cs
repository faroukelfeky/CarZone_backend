namespace Kawerk.Infastructure.DTOs.Vehicle
{
    public class VehicleSellerViewDTO
    {
        public Guid? SellerID { get; set; }
        public Guid VehicleID { get; set; }
        public required string Model { get; set; }
        public string? ManufacturerName { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public string? Type { get; set; }
        public string? Transmission { get; set; }
        public string? Year { get; set; }
        public int SeatingCapacity { get; set; }
        public string? EngineCapacity { get; set; }
        public string? FuelType { get; set; }
        public float? ConditionScore { get; set; }
        public int? DaysOnMarket { get; set; }
        public int? HorsePower { get; set; }
        public string? Status { get; set; }
        public List<string>? Images { get; set; }
    }
}
