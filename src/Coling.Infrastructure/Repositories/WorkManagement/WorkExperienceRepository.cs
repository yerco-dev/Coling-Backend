using Coling.Domain.Entities.WorkManagement;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;
using Coling.Infrastructure.Data;
using Coling.Infrastructure.Repositories.Generics;
using Microsoft.EntityFrameworkCore;

namespace Coling.Infrastructure.Repositories.WorkManagement;

public class WorkExperienceRepository : GenericRepository<WorkExperience>, IWorkExperienceRepository
{
    private readonly AppDbContext _context;

    public WorkExperienceRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<IEnumerable<WorkExperience>>> GetByMemberIdAsync(Guid memberId)
    {
        try
        {
            var experiences = await _context.WorkExperiences
                .Where(we => we.MemberId == memberId && we.IsActive)
                .ToListAsync();

            // Ordenar en memoria usando CompareTo de PartialDate
            var orderedExperiences = experiences
                .OrderByDescending(we => we.StartDate, Comparer<Domain.Entities.PartialDateManagement.PartialDate?>.Create((x, y) =>
                {
                    if (x == null && y == null) return 0;
                    if (x == null) return -1;
                    if (y == null) return 1;
                    return x.CompareTo(y);
                }))
                .ToList();

            return ActionResponse<IEnumerable<WorkExperience>>.Success(orderedExperiences);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<WorkExperience>>.Failure(
                $"Error al obtener experiencias laborales del miembro: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<IEnumerable<WorkExperience>>> GetByInstitutionIdAsync(Guid institutionId)
    {
        try
        {
            var experiences = await _context.WorkExperiences
                .Include(we => we.Member)
                    .ThenInclude(m => m.Person)
                .Where(we => we.InstitutionId == institutionId && we.IsActive)
                .ToListAsync();

            // Ordenar en memoria usando CompareTo de PartialDate
            var orderedExperiences = experiences
                .OrderByDescending(we => we.StartDate, Comparer<Domain.Entities.PartialDateManagement.PartialDate?>.Create((x, y) =>
                {
                    if (x == null && y == null) return 0;
                    if (x == null) return -1;
                    if (y == null) return 1;
                    return x.CompareTo(y);
                }))
                .ToList();

            return ActionResponse<IEnumerable<WorkExperience>>.Success(orderedExperiences);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<WorkExperience>>.Failure(
                $"Error al obtener experiencias laborales de la institución: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<IEnumerable<WorkExperience>>> GetByMemberIdWithDetailsAsync(Guid memberId)
    {
        try
        {
            var experiences = await _context.WorkExperiences
                .Include(we => we.Institution)
                    .ThenInclude(i => i.InstitutionType)
                .Include(we => we.WorkExperienceFields)
                    .ThenInclude(wef => wef.WorkField)
                        .ThenInclude(wf => wf.WorkFieldCategory)
                .Where(we => we.MemberId == memberId && we.IsActive)
                .ToListAsync();

            // Ordenar en memoria usando CompareTo de PartialDate
            var orderedExperiences = experiences
                .OrderByDescending(we => we.StartDate, Comparer<Domain.Entities.PartialDateManagement.PartialDate?>.Create((x, y) =>
                {
                    if (x == null && y == null) return 0;
                    if (x == null) return -1;
                    if (y == null) return 1;
                    return x.CompareTo(y);
                }))
                .ToList();

            return ActionResponse<IEnumerable<WorkExperience>>.Success(orderedExperiences);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<WorkExperience>>.Failure(
                $"Error al obtener experiencias laborales con detalles: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<IEnumerable<WorkExperience>>> GetCurrentJobsByMemberIdAsync(Guid memberId)
    {
        try
        {
            var experiences = await _context.WorkExperiences
                .Include(we => we.Institution)
                .Include(we => we.WorkExperienceFields)
                    .ThenInclude(wef => wef.WorkField)
                .Where(we => we.MemberId == memberId &&
                            we.EndDate == null &&
                            we.IsActive)
                .ToListAsync();

            // Ordenar en memoria usando CompareTo de PartialDate (ascendente para trabajos actuales)
            var orderedExperiences = experiences
                .OrderBy(we => we.StartDate, Comparer<Domain.Entities.PartialDateManagement.PartialDate?>.Create((x, y) =>
                {
                    if (x == null && y == null) return 0;
                    if (x == null) return -1;
                    if (y == null) return 1;
                    return x.CompareTo(y);
                }))
                .ToList();

            return ActionResponse<IEnumerable<WorkExperience>>.Success(orderedExperiences);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<WorkExperience>>.Failure(
                $"Error al obtener trabajos actuales: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
