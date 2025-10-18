using Coling.Application.DTOs.AcademicManagement;
using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Entities.PartialDateManagement;

namespace Coling.Application.Mappers.AcademicManagement;

public static class DegreeEducationMappers
{
    public static DegreeEducation ToDegreeEducation(this RegisterDegreeEducationDto dto)
    {
        return new DegreeEducation
        {
            Name = dto.Name,
            Description = dto.Description,
            InstitutionId = dto.InstitutionId,
            AcademicDegree = dto.AcademicDegree,
            Major = dto.Major,
            Specialization = dto.Specialization,
            ThesisTitle = dto.ThesisTitle,
            GPA = dto.GPA,
            HasHonors = dto.HasHonors
        };
    }

    public static MemberEducation ToMemberEducation(this RegisterDegreeEducationDto dto, Guid memberId, Guid educationId)
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

        return new MemberEducation
        {
            MemberId = memberId,
            EducationId = educationId,
            TitleReceived = dto.TitleReceived,
            StartDate = startDate,
            EndDate = endDate,
            Status = dto.Status
        };
    }

    public static MemberEducationCreatedDto ToMemberEducationCreatedDto(
        this MemberEducation memberEducation,
        DegreeEducation education,
        string institutionName)
    {
        return new MemberEducationCreatedDto
        {
            MemberEducationId = memberEducation.Id,
            EducationId = education.Id,
            EducationName = education.Name,
            InstitutionName = institutionName,
            TitleReceived = memberEducation.TitleReceived,
            Status = memberEducation.Status,
            DocumentUrl = memberEducation.DocumentUrl
        };
    }

    public static DegreeEducationDetailDto ToDegreeEducationDetailDto(
        this MemberEducation memberEducation,
        DegreeEducation education,
        string institutionName)
    {
        return new DegreeEducationDetailDto
        {
            MemberEducationId = memberEducation.Id,
            EducationId = education.Id,
            Name = education.Name,
            Description = education.Description,
            InstitutionId = education.InstitutionId,
            InstitutionName = institutionName,
            AcademicDegree = education.AcademicDegree,
            Major = education.Major,
            Specialization = education.Specialization,
            ThesisTitle = education.ThesisTitle,
            GPA = education.GPA,
            HasHonors = education.HasHonors,
            TitleReceived = memberEducation.TitleReceived,
            StartYear = memberEducation.StartDate?.Year,
            StartMonth = memberEducation.StartDate?.Month,
            StartDay = memberEducation.StartDate?.Day,
            EndYear = memberEducation.EndDate?.Year,
            EndMonth = memberEducation.EndDate?.Month,
            EndDay = memberEducation.EndDate?.Day,
            DocumentUrl = memberEducation.DocumentUrl,
            Status = memberEducation.Status,
            IsActive = memberEducation.IsActive
        };
    }
}
