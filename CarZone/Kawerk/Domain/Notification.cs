using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kawerk.Domain
{
    public class Notification
    {
        [Key]
        public Guid NotificationId { get; set; }
        [Column(TypeName = "nvarchar(150)")]
        public required string Title { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public required string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        //Relationships
        public required Guid CustomerID { get; set; }
        public required Customer Customer { get; set; }

    }
}
