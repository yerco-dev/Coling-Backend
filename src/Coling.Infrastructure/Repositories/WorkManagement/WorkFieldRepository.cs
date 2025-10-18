using Coling.Domain.Entities.WorkManagement;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;
using Coling.Infrastructure.Data;
using Coling.Infrastructure.Repositories.Generics;
using Microsoft.EntityFrameworkCore;

namespace Coling.Infrastructure.Repositories.WorkManagement;

public class WorkFieldRepository : GenericRepository<WorkField>, IWorkFieldRepository
{
    private readonly AppDbContext _context;

    public WorkFieldRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<IEnumerable<WorkField>>> GetByCategoryIdAsync(Guid categoryId)
    {
        try
        {
            var workFields = await _context.WorkFields
                .Include(wf => wf.WorkFieldCategory)
                .Where(wf => wf.WorkFieldCategoryId == categoryId && wf.IsActive)
                .OrderBy(wf => wf.Name)
                .ToListAsync();

            return ActionResponse<IEnumerable<WorkField>>.Success(workFields);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<WorkField>>.Failure(
                $"Error al obtener campos de trabajo por categoría: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<WorkField>> GetByIdWithCategoryAsync(Guid id)
    {
        try
        {
            var workField = await _context.WorkFields
                .Include(wf => wf.WorkFieldCategory)
                .FirstOrDefaultAsync(wf => wf.Id == id && wf.IsActive);

            if (workField == null)
                return ActionResponse<WorkField>.NotFound("Campo de trabajo no encontrado.");

            return ActionResponse<WorkField>.Success(workField);
        }
        catch (Exception ex)
        {
            return ActionResponse<WorkField>.Failure(
                $"Error al obtener campo de trabajo: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
