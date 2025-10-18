using Coling.Domain.Constants;
using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.AcademicManagement;
using Coling.Domain.Wrappers;
using Coling.Infrastructure.Data;
using Coling.Infrastructure.Repositories.Generics;
using Microsoft.EntityFrameworkCore;

namespace Coling.Infrastructure.Repositories.AcademicManagement;

public class MemberEducationRepository : GenericRepository<MemberEducation>, IMemberEducationRepository
{
    private readonly AppDbContext _context;

    public MemberEducationRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<IEnumerable<MemberEducation>>> GetByMemberIdAsync(Guid memberId)
    {
        try
        {
            var memberEducations = await _context.MemberEducations
                .Where(me => me.MemberId == memberId && me.IsActive)
                .ToListAsync();

            // Ordenar en memoria usando CompareTo de PartialDate
            var orderedEducations = memberEducations
                .OrderByDescending(me => me.StartDate, Comparer<Domain.Entities.PartialDateManagement.PartialDate?>.Create((x, y) =>
                {
                    if (x == null && y == null) return 0;
                    if (x == null) return -1;
                    if (y == null) return 1;
                    return x.CompareTo(y);
                }))
                .ToList();

            return ActionResponse<IEnumerable<MemberEducation>>.Success(orderedEducations);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<MemberEducation>>.Failure(
                $"Error al obtener educaciones del miembro: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<IEnumerable<MemberEducation>>> GetByEducationIdAsync(Guid educationId)
    {
        try
        {
            var memberEducations = await _context.MemberEducations
                .Include(me => me.Member)
                    .ThenInclude(m => m.Person)
                .Where(me => me.EducationId == educationId && me.IsActive)
                .ToListAsync();

            return ActionResponse<IEnumerable<MemberEducation>>.Success(memberEducations);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<MemberEducation>>.Failure(
                $"Error al obtener miembros de la educación: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<IEnumerable<MemberEducation>>> GetByMemberIdWithDetailsAsync(Guid memberId)
    {
        try
        {
            var memberEducations = await _context.MemberEducations
                .Include(me => me.Education)
                    .ThenInclude(e => e.Institution)
                .Include(me => me.Member)
                .Where(me => me.MemberId == memberId && me.IsActive)
                .ToListAsync();

            // Ordenar en memoria usando CompareTo de PartialDate
            var orderedEducations = memberEducations
                .OrderByDescending(me => me.StartDate, Comparer<Domain.Entities.PartialDateManagement.PartialDate?>.Create((x, y) =>
                {
                    if (x == null && y == null) return 0;
                    if (x == null) return -1;
                    if (y == null) return 1;
                    return x.CompareTo(y);
                }))
                .ToList();

            return ActionResponse<IEnumerable<MemberEducation>>.Success(orderedEducations);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<MemberEducation>>.Failure(
                $"Error al obtener educaciones del miembro con detalles: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<IEnumerable<MemberEducation>>> GetInProgressByMemberIdAsync(Guid memberId)
    {
        try
        {
            var inProgressStatus = BusinessConstants.EducationStatusValues[EducationStatus.InProgress];

            var memberEducations = await _context.MemberEducations
                .Include(me => me.Education)
                    .ThenInclude(e => e.Institution)
                .Where(me => me.MemberId == memberId &&
                            me.Status == inProgressStatus &&
                            me.IsActive)
                .ToListAsync();

            // Ordenar en memoria usando CompareTo de PartialDate
            var orderedEducations = memberEducations
                .OrderByDescending(me => me.StartDate, Comparer<Domain.Entities.PartialDateManagement.PartialDate?>.Create((x, y) =>
                {
                    if (x == null && y == null) return 0;
                    if (x == null) return -1;
                    if (y == null) return 1;
                    return x.CompareTo(y);
                }))
                .ToList();

            return ActionResponse<IEnumerable<MemberEducation>>.Success(orderedEducations);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<MemberEducation>>.Failure(
                $"Error al obtener educaciones en progreso: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<IEnumerable<MemberEducation>>> GetCompletedByMemberIdAsync(Guid memberId)
    {
        try
        {
            var completedStatus = BusinessConstants.EducationStatusValues[EducationStatus.Completed];

            var memberEducations = await _context.MemberEducations
                .Include(me => me.Education)
                    .ThenInclude(e => e.Institution)
                .Where(me => me.MemberId == memberId &&
                            me.Status == completedStatus &&
                            me.IsActive)
                .ToListAsync();

            // Ordenar en memoria usando CompareTo de PartialDate
            var orderedEducations = memberEducations
                .OrderByDescending(me => me.EndDate, Comparer<Domain.Entities.PartialDateManagement.PartialDate?>.Create((x, y) =>
                {
                    if (x == null && y == null) return 0;
                    if (x == null) return -1;
                    if (y == null) return 1;
                    return x.CompareTo(y);
                }))
                .ToList();

            return ActionResponse<IEnumerable<MemberEducation>>.Success(orderedEducations);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<MemberEducation>>.Failure(
                $"Error al obtener educaciones completadas: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
