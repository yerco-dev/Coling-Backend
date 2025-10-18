namespace Coling.Application.DTOs.WorkManagement;

public class WorkExperienceDetailDto
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public Guid InstitutionId { get; set; }
    public string? InstitutionName { get; set; }
    public string? InstitutionTypeName { get; set; }
    public string JobTitle { get; set; } = null!;

    public int? StartYear { get; set; }
    public int? StartMonth { get; set; }
    public int? StartDay { get; set; }

    public int? EndYear { get; set; }
    public int? EndMonth { get; set; }
    public int? EndDay { get; set; }

    public string? Description { get; set; }
    public string? Responsibilities { get; set; }
    public string? Achievements { get; set; }
    public string? DocumentUrl { get; set; }
    public int DurationInMonths { get; set; }
    public bool IsActive { get; set; }

    public List<WorkFieldInExperienceDto> WorkFields { get; set; } = new List<WorkFieldInExperienceDto>();
}
