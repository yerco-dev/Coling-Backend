namespace Coling.Application.DTOs.WorkManagement;

public class WorkFieldInExperienceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Guid WorkFieldCategoryId { get; set; }
    public string? WorkFieldCategoryName { get; set; }
}
