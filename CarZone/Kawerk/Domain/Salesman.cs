using System.ComponentModel.DataAnnotations.Schema;

namespace Kawerk.Domain
{
    public class Salesman
    {
        public Guid SalesmanID { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public required string Name { get; set; }
        [Column(TypeName = "varchar(100)")]
        public required string Email { get; set; }
        [Column(TypeName = "varchar(200)")]
        public required string Password { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? Phone { get; set; }
        public int Salary { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string? Address { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? City { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; }

        //Relationships
        public Branches? Branch { get; set; }
        public Guid? BranchID { get; set; }
    }
}
