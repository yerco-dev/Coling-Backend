using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.InstitutionManagement;

public class DeleteInstitutionUseCase
{
    private readonly IInstitutionRepository _institutionRepository;

    public DeleteInstitutionUseCase(IInstitutionRepository institutionRepository)
    {
        _institutionRepository = institutionRepository;
    }

    public async Task<ActionResponse<bool>> ExecuteAsync(Guid institutionId)
    {
        var institutionResult = await _institutionRepository.GetAsync(institutionId);

        if (!institutionResult.WasSuccessful)
            return ActionResponse<bool>.NotFound("Institución no encontrada.");

        var deleteResult = await _institutionRepository.DeleteAsync(institutionId);

        if (!deleteResult.WasSuccessful)
            return ActionResponse<bool>.Failure(
                "Error al dar de baja la institución.",
                ResultCode.DatabaseError);

        return ActionResponse<bool>.Success(true, "Institución dada de baja correctamente.");
    }
}
