namespace Kawerk.Infastructure.DTOs.Branch
{
    public class BranchCreationDTO
    {
        public required string Name { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
    }
}
