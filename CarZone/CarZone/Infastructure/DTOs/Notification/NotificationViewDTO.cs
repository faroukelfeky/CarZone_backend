using System.ComponentModel.DataAnnotations.Schema;

namespace Kawerk.Infastructure.DTOs.Notification
{
    public class NotificationViewDTO
    {
        public Guid NotificationID { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
