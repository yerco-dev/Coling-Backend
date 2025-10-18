using Coling.Application.DTOs.WorkManagement;
using Coling.Domain.Entities.WorkManagement;

namespace Coling.Application.Mappers.WorkManagement;

public static class WorkFieldCategoryMappers
{
    public static WorkFieldCategory ToEntity(this WorkFieldCategoryCreateDto dto)
    {
        return new WorkFieldCategory
        {
            Name = dto.Name,
            Description = dto.Description
        };
    }

    public static WorkFieldCategoryGetDto ToGetDto(this WorkFieldCategory entity)
    {
        return new WorkFieldCategoryGetDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive
        };
    }

    public static WorkFieldCategoryWithFieldsDto ToWithFieldsDto(this WorkFieldCategory entity)
    {
        return new WorkFieldCategoryWithFieldsDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive,
            WorkFields = entity.WorkFields
                .Where(wf => wf.IsActive)
                .Select(wf => wf.ToGetDto())
                .ToList()
        };
    }

    public static void UpdateFromDto(this WorkFieldCategory entity, WorkFieldCategoryUpdateDto dto)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
    }
}
