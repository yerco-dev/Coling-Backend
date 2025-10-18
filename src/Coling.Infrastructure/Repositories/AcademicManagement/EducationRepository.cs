using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.AcademicManagement;
using Coling.Domain.Wrappers;
using Coling.Infrastructure.Data;
using Coling.Infrastructure.Repositories.Generics;
using Microsoft.EntityFrameworkCore;

namespace Coling.Infrastructure.Repositories.AcademicManagement;

public class EducationRepository : GenericRepository<Education>, IEducationRepository
{
    private readonly AppDbContext _context;

    public EducationRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<IEnumerable<Education>>> GetByInstitutionIdAsync(Guid institutionId)
    {
        try
        {
            var educations = await _context.Educations
                .Include(e => e.Institution)
                .Where(e => e.InstitutionId == institutionId && e.IsActive)
                .ToListAsync();

            return ActionResponse<IEnumerable<Education>>.Success(educations);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<Education>>.Failure(
                $"Error al obtener educaciones por institución: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<Education>> GetByIdWithInstitutionAsync(Guid id)
    {
        try
        {
            var education = await _context.Educations
                .Include(e => e.Institution)
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

            if (education == null)
                return ActionResponse<Education>.NotFound("Educación no encontrada.");

            return ActionResponse<Education>.Success(education);
        }
        catch (Exception ex)
        {
            return ActionResponse<Education>.Failure(
                $"Error al obtener educación: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
