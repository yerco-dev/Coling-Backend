namespace Coling.Application.DTOs.InstitutionManagement;

public class InstitutionDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid InstitutionTypeId { get; set; }
    public string InstitutionTypeName { get; set; } = null!;
    public bool IsActive { get; set; }
}
