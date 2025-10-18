namespace Coling.Application.DTOs.WorkManagement;

public class WorkFieldCategoryWithFieldsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public IEnumerable<WorkFieldGetDto> WorkFields { get; set; } = new List<WorkFieldGetDto>();
}
