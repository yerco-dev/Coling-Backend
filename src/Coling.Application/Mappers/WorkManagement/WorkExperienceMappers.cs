using Coling.Application.DTOs.WorkManagement;
using Coling.Domain.Entities.PartialDateManagement;
using Coling.Domain.Entities.WorkManagement;

namespace Coling.Application.Mappers.WorkManagement;

public static class WorkExperienceMappers
{
    public static WorkExperience ToEntity(this RegisterWorkExperienceDto dto, Guid memberId)
    {
        PartialDate? startDate = null;
        if (dto.StartYear.HasValue)
        {
            startDate = new PartialDate(dto.StartYear.Value, dto.StartMonth, dto.StartDay);
        }

        PartialDate? endDate = null;
        if (dto.EndYear.HasValue)
        {
            endDate = new PartialDate(dto.EndYear.Value, dto.EndMonth, dto.EndDay);
        }

        return new WorkExperience
        {
            MemberId = memberId,
            InstitutionId = dto.InstitutionId,
            JobTitle = dto.JobTitle,
            StartDate = startDate!,
            EndDate = endDate,
            Description = dto.Description,
            Responsibilities = dto.Responsibilities,
            Achievements = dto.Achievements
        };
    }

    public static WorkExperienceDetailDto ToDetailDto(this WorkExperience entity)
    {
        return new WorkExperienceDetailDto
        {
            Id = entity.Id,
            MemberId = entity.MemberId,
            InstitutionId = entity.InstitutionId,
            InstitutionName = entity.Institution?.Name,
            InstitutionTypeName = entity.Institution?.InstitutionType?.Name,
            JobTitle = entity.JobTitle,
            StartYear = entity.StartDate?.Year,
            StartMonth = entity.StartDate?.Month,
            StartDay = entity.StartDate?.Day,
            EndYear = entity.EndDate?.Year,
            EndMonth = entity.EndDate?.Month,
            EndDay = entity.EndDate?.Day,
            Description = entity.Description,
            Responsibilities = entity.Responsibilities,
            Achievements = entity.Achievements,
            DocumentUrl = entity.DocumentUrl,
            DurationInMonths = entity.DurationInMonths,
            IsActive = entity.IsActive,
            WorkFields = entity.WorkExperienceFields
                .Where(wef => wef.IsActive && wef.WorkField != null)
                .Select(wef => new WorkFieldInExperienceDto
                {
                    Id = wef.WorkField!.Id,
                    Name = wef.WorkField.Name,
                    Description = wef.WorkField.Description,
                    WorkFieldCategoryId = wef.WorkField.WorkFieldCategoryId,
                    WorkFieldCategoryName = wef.WorkField.WorkFieldCategory?.Name
                })
                .ToList()
        };
    }

    public static void UpdateFromDto(this WorkExperience entity, UpdateWorkExperienceDto dto)
    {
        PartialDate? startDate = null;
        if (dto.StartYear.HasValue)
        {
            startDate = new PartialDate(dto.StartYear.Value, dto.StartMonth, dto.StartDay);
        }

        PartialDate? endDate = null;
        if (dto.EndYear.HasValue)
        {
            endDate = new PartialDate(dto.EndYear.Value, dto.EndMonth, dto.EndDay);
        }

        entity.InstitutionId = dto.InstitutionId;
        entity.JobTitle = dto.JobTitle;
        entity.StartDate = startDate!;
        entity.EndDate = endDate;
        entity.Description = dto.Description;
        entity.Responsibilities = dto.Responsibilities;
        entity.Achievements = dto.Achievements;
    }
}
