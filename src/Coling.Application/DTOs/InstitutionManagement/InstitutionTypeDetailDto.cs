namespace Coling.Application.DTOs.InstitutionManagement;

public class InstitutionTypeDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}
