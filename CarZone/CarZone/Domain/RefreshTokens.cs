using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kawerk.Domain
{
    public class RefreshTokens
    {
        [Key]
        public Guid RefreshTokenID { get; set; }
        [Column(TypeName = "varchar(500)")]
        public required string RefreshToken { get; set; }
        public DateTime Expires { get; set; }

        //Relationships
        public Guid CustomerID { get; set; }
        public Customer? Customer { get; set; }
    }
}
