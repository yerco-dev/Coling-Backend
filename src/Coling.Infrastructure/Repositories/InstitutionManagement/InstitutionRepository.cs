using Coling.Domain.Entities.InstitutionManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Wrappers;
using Coling.Infrastructure.Data;
using Coling.Infrastructure.Repositories.Generics;
using Microsoft.EntityFrameworkCore;

namespace Coling.Infrastructure.Repositories.InstitutionManagement;

public class InstitutionRepository : GenericRepository<Institution>, IInstitutionRepository
{
    private readonly AppDbContext _context;

    public InstitutionRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<IEnumerable<Institution>>> GetByInstitutionTypeIdAsync(Guid institutionTypeId)
    {
        try
        {
            var institutions = await _context.Institutions
                .Include(i => i.InstitutionType)
                .Where(i => i.InstitutionTypeId == institutionTypeId && i.IsActive)
                .OrderBy(i => i.Name)
                .ToListAsync();

            return ActionResponse<IEnumerable<Institution>>.Success(institutions);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<Institution>>.Failure(
                $"Error al obtener instituciones por tipo: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<Institution>> GetByIdWithTypeAsync(Guid id)
    {
        try
        {
            var institution = await _context.Institutions
                .Include(i => i.InstitutionType)
                .FirstOrDefaultAsync(i => i.Id == id && i.IsActive);

            if (institution == null)
                return ActionResponse<Institution>.NotFound("Institución no encontrada.");

            return ActionResponse<Institution>.Success(institution);
        }
        catch (Exception ex)
        {
            return ActionResponse<Institution>.Failure(
                $"Error al obtener institución: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<IEnumerable<Institution>>> GetByNameAsync(string name)
    {
        try
        {
            var institutions = await _context.Institutions
                .Include(i => i.InstitutionType)
                .Where(i => i.Name.Contains(name) && i.IsActive)
                .OrderBy(i => i.Name)
                .ToListAsync();

            return ActionResponse<IEnumerable<Institution>>.Success(institutions);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<Institution>>.Failure(
                $"Error al buscar instituciones: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
