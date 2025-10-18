using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.AcademicManagement;
using Coling.Domain.Wrappers;
using Coling.Infrastructure.Data;
using Coling.Infrastructure.Repositories.Generics;
using Microsoft.EntityFrameworkCore;

namespace Coling.Infrastructure.Repositories.AcademicManagement;

public class ContinuingEducationRepository : GenericRepository<ContinuingEducation>, IContinuingEducationRepository
{
    private readonly AppDbContext _context;

    public ContinuingEducationRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<IEnumerable<ContinuingEducation>>> GetByEducationTypeAsync(string educationType)
    {
        try
        {
            var educations = await _context.ContinuingEducations
                .Include(c => c.Institution)
                .Where(c => c.EducationType == educationType && c.IsActive)
                .ToListAsync();

            return ActionResponse<IEnumerable<ContinuingEducation>>.Success(educations);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<ContinuingEducation>>.Failure(
                $"Error al obtener educaciones continuas: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<IEnumerable<ContinuingEducation>>> GetWithCertificateAsync()
    {
        try
        {
            var educations = await _context.ContinuingEducations
                .Include(c => c.Institution)
                .Where(c => c.IssuesCertificate && c.IsActive)
                .ToListAsync();

            return ActionResponse<IEnumerable<ContinuingEducation>>.Success(educations);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<ContinuingEducation>>.Failure(
                $"Error al obtener educaciones con certificado: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
