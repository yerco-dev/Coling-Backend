using Coling.Domain.Constants;
using Coling.Domain.Entities;
using Coling.Domain.Entities.ActionResponse;
using Coling.Domain.Interfaces.Repositories.MembersManagement;
using Coling.Infrastructure.Data;
using Coling.Infrastructure.Repositories.Generics;
using Microsoft.EntityFrameworkCore;

namespace Coling.Infrastructure.Repositories.MembersManagement;

public class MemberRepository : GenericRepository<Member>, IMemberRepository
{
    private readonly AppDbContext _context;

    public MemberRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<IEnumerable<Member>>> GetPendingMembersAsync()
    {
        try
        {
            var pendingStatus = BusinessConstants.MemberStatusValues[MemberStatus.Pending];

            var members = await _context.Members
                .Include(m => m.Person)
                .Where(m => m.Status == pendingStatus && m.IsActive)
                .OrderByDescending(m => m.MembershipDate)
                .ToListAsync();

            return ActionResponse<IEnumerable<Member>>.Success(members);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<Member>>.Failure(
                $"Error al obtener miembros pendientes: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<Member>> GetMemberByPersonIdAsync(Guid personId)
    {
        try
        {
            var member = await _context.Members
                .Include(m => m.Person)
                .FirstOrDefaultAsync(m => m.PersonId == personId && m.IsActive);

            if (member == null)
                return ActionResponse<Member>.NotFound("Miembro no encontrado.");

            return ActionResponse<Member>.Success(member);
        }
        catch (Exception ex)
        {
            return ActionResponse<Member>.Failure(
                $"Error al obtener el miembro: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
