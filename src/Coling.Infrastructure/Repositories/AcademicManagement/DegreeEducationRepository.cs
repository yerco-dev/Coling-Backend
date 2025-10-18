using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.AcademicManagement;
using Coling.Domain.Wrappers;
using Coling.Infrastructure.Data;
using Coling.Infrastructure.Repositories.Generics;
using Microsoft.EntityFrameworkCore;

namespace Coling.Infrastructure.Repositories.AcademicManagement;

public class DegreeEducationRepository : GenericRepository<DegreeEducation>, IDegreeEducationRepository
{
    private readonly AppDbContext _context;

    public DegreeEducationRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<IEnumerable<DegreeEducation>>> GetByAcademicDegreeAsync(string academicDegree)
    {
        try
        {
            var degrees = await _context.DegreeEducations
                .Include(d => d.Institution)
                .Where(d => d.AcademicDegree == academicDegree && d.IsActive)
                .ToListAsync();

            return ActionResponse<IEnumerable<DegreeEducation>>.Success(degrees);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<DegreeEducation>>.Failure(
                $"Error al obtener grados académicos: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<IEnumerable<DegreeEducation>>> GetByMajorAsync(string major)
    {
        try
        {
            var degrees = await _context.DegreeEducations
                .Include(d => d.Institution)
                .Where(d => d.Major.Contains(major) && d.IsActive)
                .ToListAsync();

            return ActionResponse<IEnumerable<DegreeEducation>>.Success(degrees);
        }
        catch (Exception ex)
        {
            return ActionResponse<IEnumerable<DegreeEducation>>.Failure(
                $"Error al obtener carreras: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
