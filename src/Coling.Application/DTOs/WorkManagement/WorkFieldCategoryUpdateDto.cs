namespace Coling.Application.DTOs.WorkManagement;

public class WorkFieldCategoryUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
