using Coling.Application.DTOs.WorkManagement;
using Coling.Application.Mappers.WorkManagement;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.WorkManagement;

public class GetMyWorkExperiencesUseCase
{
    private readonly IWorkExperienceRepository _workExperienceRepository;

    public GetMyWorkExperiencesUseCase(IWorkExperienceRepository workExperienceRepository)
    {
        _workExperienceRepository = workExperienceRepository;
    }

    public async Task<ActionResponse<IEnumerable<WorkExperienceDetailDto>>> ExecuteAsync(Guid memberId)
    {
        var result = await _workExperienceRepository.GetByMemberIdWithDetailsAsync(memberId);

        if (!result.WasSuccessful)
            return ActionResponse<IEnumerable<WorkExperienceDetailDto>>.Failure(
                result.Message ?? "Error al obtener las experiencias laborales.",
                ResultCode.DatabaseError);

        var dtos = result.Result!.Select(we => we.ToDetailDto());

        return ActionResponse<IEnumerable<WorkExperienceDetailDto>>.Success(dtos);
    }
}
