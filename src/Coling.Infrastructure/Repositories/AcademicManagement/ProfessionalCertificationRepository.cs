using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.AcademicManagement;
using Coling.Domain.Wrappers;
using Coling.Infrastructure.Data;
using Coling.Infrastructure.Repositories.Generics;
using Microsoft.EntityFrameworkCore;

namespace Coling.Infrastructure.Repositories.AcademicManagement;

public class ProfessionalCertificationRepository : GenericRepository<ProfessionalCertification>, IProfessionalCertificationRepository
{
    private readonly AppDbContext _context;

    public ProfessionalCertificationRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<IEnumerable<ProfessionalCertification>>> GetExpiringCertificationsAsync(DateTime beforeDate)
    {
        try
        {
            var certifications = await _context.ProfessionalCertifications
                .Include(p => p.Institution)
                .Where(p => p.ExpirationDate != null &&
                           p.ExpirationDate <= beforeDate &&
                           p.IsActive)
                .OrderBy(p => p.ExpirationDate)
                .ToListAsync();

            return ActionResponse<IEnumerable<ProfessionalCertification>>.Success(certifications);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<ProfessionalCertification>>.Failure(
                $"Error al obtener certificaciones por vencer: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<IEnumerable<ProfessionalCertification>>> GetByCertificationNumberAsync(string certificationNumber)
    {
        try
        {
            var certifications = await _context.ProfessionalCertifications
                .Include(p => p.Institution)
                .Where(p => p.CertificationNumber.Contains(certificationNumber) && p.IsActive)
                .ToListAsync();

            return ActionResponse<IEnumerable<ProfessionalCertification>>.Success(certifications);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<ProfessionalCertification>>.Failure(
                $"Error al obtener certificaciones: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
