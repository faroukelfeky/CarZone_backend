using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kawerk.Domain
{
    public class Vehicle
    {
        [Key]
        public Guid VehicleID { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ManufacturerName {  get; set; }
        [Column(TypeName = "varchar(200)")]
        public string? Model { get; set; }
        [Column(TypeName ="varchar(2000)")]
        public string? Description { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? Country { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? City { get; set; }
        public int Price { get; set; }
        public float? ConditionScore {  get; set; }
        public int? DaysOnMarket { get; set; }
        public int? HorsePower { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? Type { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? Transmission {  get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? Year { get; set; }
        public int SeatingCapacity { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? EngineCapacity { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? FuelType { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? Status { get; set; }
        public int Doors { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? Color { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string>? Images { get; set; }

        //Relationships
        public Manufacturer? Manufacturer { get; set; }
        public Guid? ManufacturerID { get; set; }
        public Customer? Seller { get; set; }
        public Guid? SellerID { get; set; }
        public Customer? Buyer { get; set; }
        public Guid? BuyerID {  get; set; }
        public Transaction? Transaction { get; set; }
    }
}
