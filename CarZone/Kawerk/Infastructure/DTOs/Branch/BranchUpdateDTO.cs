using System.ComponentModel.DataAnnotations.Schema;

namespace Kawerk.Infastructure.DTOs.Branch
{
    public class BranchUpdateDTO
    {
        public required string Name { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? Warranty { get; set; }
    }
}
