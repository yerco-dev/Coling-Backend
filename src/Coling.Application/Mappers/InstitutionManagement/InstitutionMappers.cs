using Coling.Application.DTOs.InstitutionManagement;
using Coling.Domain.Entities.InstitutionManagement;

namespace Coling.Application.Mappers.InstitutionManagement;

public static class InstitutionMappers
{
    public static Institution ToInstitution(this RegisterInstitutionDto dto)
    {
        return new Institution
        {
            Name = dto.Name,
            InstitutionTypeId = dto.InstitutionTypeId
        };
    }

    public static InstitutionCreatedDto ToInstitutionCreatedDto(
        this Institution institution,
        string institutionTypeName)
    {
        return new InstitutionCreatedDto
        {
            Id = institution.Id,
            Name = institution.Name,
            InstitutionTypeName = institutionTypeName
        };
    }

    public static InstitutionType ToInstitutionType(this RegisterInstitutionTypeDto dto)
    {
        return new InstitutionType
        {
            Name = dto.Name
        };
    }

    public static InstitutionTypeCreatedDto ToInstitutionTypeCreatedDto(this InstitutionType institutionType)
    {
        return new InstitutionTypeCreatedDto
        {
            Id = institutionType.Id,
            Name = institutionType.Name
        };
    }
}
