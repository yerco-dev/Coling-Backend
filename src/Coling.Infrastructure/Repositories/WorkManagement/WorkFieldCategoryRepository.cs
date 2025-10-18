using Coling.Domain.Entities.WorkManagement;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;
using Coling.Infrastructure.Data;
using Coling.Infrastructure.Repositories.Generics;
using Microsoft.EntityFrameworkCore;

namespace Coling.Infrastructure.Repositories.WorkManagement;

public class WorkFieldCategoryRepository : GenericRepository<WorkFieldCategory>, IWorkFieldCategoryRepository
{
    private readonly AppDbContext _context;

    public WorkFieldCategoryRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<IEnumerable<WorkFieldCategory>>> GetAllWithWorkFieldsAsync()
    {
        try
        {
            var categories = await _context.WorkFieldCategories
                .Include(c => c.WorkFields.Where(wf => wf.IsActive))
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return ActionResponse<IEnumerable<WorkFieldCategory>>.Success(categories);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<WorkFieldCategory>>.Failure(
                $"Error al obtener categorías con campos de trabajo: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<WorkFieldCategory>> GetByIdWithWorkFieldsAsync(Guid id)
    {
        try
        {
            var category = await _context.WorkFieldCategories
                .Include(c => c.WorkFields.Where(wf => wf.IsActive))
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (category == null)
                return ActionResponse<WorkFieldCategory>.NotFound("Categoría de campo de trabajo no encontrada.");

            return ActionResponse<WorkFieldCategory>.Success(category);
        }
        catch (Exception ex)
        {
            return ActionResponse<WorkFieldCategory>.Failure(
                $"Error al obtener categoría con campos de trabajo: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
