using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kawerk.Domain
{
    public class Manufacturer
    {
        [Key]
        public Guid ManufacturerID { get; set; }
        [Column(TypeName = "varchar(100)")]
        public required string Name { get; set; }
        [Column(TypeName = "varchar(2000)")]
        public string? Description { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? Type { get; set; } // Cars or motorcycles , etc
        [Column(TypeName = "varchar(200)")]
        public string? Warranty { get; set; }
        public DateTime CreatedAt { get; set; }

        //Relationships
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public List<Customer> Subscribers { get; set; } = new List<Customer>();
        public Customer? Manager { get; set; }
        public Guid? ManagerID { get; set; }

    }
}
