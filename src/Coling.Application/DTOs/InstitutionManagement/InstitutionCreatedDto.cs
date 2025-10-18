namespace Coling.Application.DTOs.InstitutionManagement;

public class InstitutionCreatedDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string InstitutionTypeName { get; set; } = null!;
}
