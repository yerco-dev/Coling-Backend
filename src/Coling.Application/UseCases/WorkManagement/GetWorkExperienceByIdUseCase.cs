using Coling.Application.DTOs.WorkManagement;
using Coling.Application.Mappers.WorkManagement;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.WorkManagement;

public class GetWorkExperienceByIdUseCase
{
    private readonly IWorkExperienceRepository _workExperienceRepository;

    public GetWorkExperienceByIdUseCase(IWorkExperienceRepository workExperienceRepository)
    {
        _workExperienceRepository = workExperienceRepository;
    }

    public async Task<ActionResponse<WorkExperienceDetailDto>> ExecuteAsync(Guid id, Guid memberId)
    {
        // Obtener todas las experiencias del miembro con detalles
        var memberExperiences = await _workExperienceRepository.GetByMemberIdWithDetailsAsync(memberId);

        if (!memberExperiences.WasSuccessful)
            return ActionResponse<WorkExperienceDetailDto>.Failure(
                "Error al obtener la experiencia laboral.",
                ResultCode.DatabaseError);

        // Buscar la experiencia específica
        var experience = memberExperiences.Result!.FirstOrDefault(we => we.Id == id);

        if (experience == null)
            return ActionResponse<WorkExperienceDetailDto>.NotFound(
                "Experiencia laboral no encontrada o no tienes permiso para acceder a ella.");

        return ActionResponse<WorkExperienceDetailDto>.Success(experience.ToDetailDto());
    }
}
