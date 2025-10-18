using Coling.Domain.Entities.WorkManagement;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;
using Coling.Infrastructure.Data;
using Coling.Infrastructure.Repositories.Generics;
using Microsoft.EntityFrameworkCore;

namespace Coling.Infrastructure.Repositories.WorkManagement;

public class WorkExperienceFieldRepository : GenericRepository<WorkExperienceField>, IWorkExperienceFieldRepository
{
    private readonly AppDbContext _context;

    public WorkExperienceFieldRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<IEnumerable<WorkExperienceField>>> GetByWorkExperienceIdAsync(Guid workExperienceId)
    {
        try
        {
            var fields = await _context.WorkExperienceFields
                .Include(wef => wef.WorkField)
                    .ThenInclude(wf => wf.WorkFieldCategory)
                .Where(wef => wef.WorkExperienceId == workExperienceId && wef.IsActive)
                .ToListAsync();

            return ActionResponse<IEnumerable<WorkExperienceField>>.Success(fields);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<WorkExperienceField>>.Failure(
                $"Error al obtener campos de la experiencia laboral: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<IEnumerable<WorkExperienceField>>> GetByWorkFieldIdAsync(Guid workFieldId)
    {
        try
        {
            var fields = await _context.WorkExperienceFields
                .Include(wef => wef.WorkExperience)
                    .ThenInclude(we => we.Member)
                        .ThenInclude(m => m.Person)
                .Where(wef => wef.WorkFieldId == workFieldId && wef.IsActive)
                .ToListAsync();

            return ActionResponse<IEnumerable<WorkExperienceField>>.Success(fields);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<WorkExperienceField>>.Failure(
                $"Error al obtener experiencias del campo de trabajo: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
