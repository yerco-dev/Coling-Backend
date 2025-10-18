using Coling.Application.DTOs.AcademicManagement;
using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Entities.PartialDateManagement;

namespace Coling.Application.Mappers.AcademicManagement;

public static class ProfessionalCertificationMappers
{
    public static ProfessionalCertification ToProfessionalCertification(this RegisterProfessionalCertificationDto dto)
    {
        return new ProfessionalCertification
        {
            Name = dto.Name,
            Description = dto.Description,
            InstitutionId = dto.InstitutionId,
            CertificationNumber = dto.CertificationNumber,
            ExpirationDate = dto.ExpirationDate,
            RequiresRenewal = dto.RequiresRenewal
        };
    }

    public static MemberEducation ToMemberEducation(this RegisterProfessionalCertificationDto dto, Guid memberId, Guid educationId)
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

    public static ProfessionalCertificationCreatedDto ToProfessionalCertificationCreatedDto(
        this MemberEducation memberEducation,
        ProfessionalCertification education,
        string institutionName)
    {
        return new ProfessionalCertificationCreatedDto
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

    public static ProfessionalCertificationDetailDto ToProfessionalCertificationDetailDto(
        this MemberEducation memberEducation,
        ProfessionalCertification education,
        string institutionName)
    {
        return new ProfessionalCertificationDetailDto
        {
            MemberEducationId = memberEducation.Id,
            EducationId = education.Id,
            Name = education.Name,
            Description = education.Description,
            InstitutionId = education.InstitutionId,
            InstitutionName = institutionName,
            CertificationNumber = education.CertificationNumber,
            ExpirationDate = education.ExpirationDate,
            RequiresRenewal = education.RequiresRenewal,
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
