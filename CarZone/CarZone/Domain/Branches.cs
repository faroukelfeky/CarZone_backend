using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kawerk.Domain
{
    public class Branches
    {
        [Key]
        public Guid BranchID { get; set; }
        [Column(TypeName ="nvarchar(100)")]
        public required string Name { get; set; }
        [Column(TypeName = "varchar(500)")]
        public string? Location { get; set; }
        [Column(TypeName = "varchar(2000)")]
        public string? Description { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string? Warranty { get; set; }
        public DateTime CreatedAt { get; set; }

        //Relationships
        public Customer? BranchManager { get; set; }
        public List<Salesman> Salesmen { get; set; } = new List<Salesman>();
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
