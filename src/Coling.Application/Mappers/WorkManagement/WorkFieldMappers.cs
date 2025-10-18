using Coling.Application.DTOs.WorkManagement;
using Coling.Domain.Entities.WorkManagement;

namespace Coling.Application.Mappers.WorkManagement;

public static class WorkFieldMappers
{
    public static WorkField ToEntity(this WorkFieldCreateDto dto)
    {
        return new WorkField
        {
            Name = dto.Name,
            Description = dto.Description,
            WorkFieldCategoryId = dto.WorkFieldCategoryId
        };
    }

    public static WorkFieldGetDto ToGetDto(this WorkField entity)
    {
        return new WorkFieldGetDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            WorkFieldCategoryId = entity.WorkFieldCategoryId,
            WorkFieldCategoryName = entity.WorkFieldCategory?.Name,
            IsActive = entity.IsActive
        };
    }

    public static void UpdateFromDto(this WorkField entity, WorkFieldUpdateDto dto)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.WorkFieldCategoryId = dto.WorkFieldCategoryId;
    }
}
