namespace Coling.Application.DTOs.WorkManagement;

public class WorkFieldCreateDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Guid WorkFieldCategoryId { get; set; }
}
