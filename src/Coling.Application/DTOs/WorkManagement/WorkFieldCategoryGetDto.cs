namespace Coling.Application.DTOs.WorkManagement;

public class WorkFieldCategoryGetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
