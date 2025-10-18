using Coling.Application.DTOs.AcademicManagement;
using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Entities.PartialDateManagement;

namespace Coling.Application.Mappers.AcademicManagement;

public static class ContinuingEducationMappers
{
    public static ContinuingEducation ToContinuingEducation(this RegisterContinuingEducationDto dto)
    {
        return new ContinuingEducation
        {
            Name = dto.Name,
            Description = dto.Description,
            InstitutionId = dto.InstitutionId,
            DurationHours = dto.DurationHours,
            EducationType = dto.EducationType,
            IssuesCertificate = dto.IssuesCertificate,
            CertificateNumber = dto.CertificateNumber
        };
    }

    public static MemberEducation ToMemberEducation(this RegisterContinuingEducationDto dto, Guid memberId, Guid educationId)
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

    public static ContinuingEducationCreatedDto ToContinuingEducationCreatedDto(
        this MemberEducation memberEducation,
        ContinuingEducation education,
        string institutionName)
    {
        return new ContinuingEducationCreatedDto
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

    public static ContinuingEducationDetailDto ToContinuingEducationDetailDto(
        this MemberEducation memberEducation,
        ContinuingEducation education,
        string institutionName)
    {
        return new ContinuingEducationDetailDto
        {
            MemberEducationId = memberEducation.Id,
            EducationId = education.Id,
            Name = education.Name,
            Description = education.Description,
            InstitutionId = education.InstitutionId,
            InstitutionName = institutionName,
            DurationHours = education.DurationHours,
            EducationType = education.EducationType,
            IssuesCertificate = education.IssuesCertificate,
            CertificateNumber = education.CertificateNumber,
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
