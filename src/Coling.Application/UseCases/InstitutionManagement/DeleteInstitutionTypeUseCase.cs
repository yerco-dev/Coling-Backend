using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.InstitutionManagement;

public class DeleteInstitutionTypeUseCase
{
    private readonly IInstitutionTypeRepository _institutionTypeRepository;

    public DeleteInstitutionTypeUseCase(IInstitutionTypeRepository institutionTypeRepository)
    {
        _institutionTypeRepository = institutionTypeRepository;
    }

    public async Task<ActionResponse<bool>> ExecuteAsync(Guid institutionTypeId)
    {
        var institutionTypeResult = await _institutionTypeRepository.GetAsync(institutionTypeId);

        if (!institutionTypeResult.WasSuccessful)
            return ActionResponse<bool>.NotFound("Tipo de institución no encontrado.");

        var deleteResult = await _institutionTypeRepository.DeleteAsync(institutionTypeId);

        if (!deleteResult.WasSuccessful)
            return ActionResponse<bool>.Failure(
                "Error al dar de baja el tipo de institución.",
                ResultCode.DatabaseError);

        return ActionResponse<bool>.Success(true, "Tipo de institución dado de baja correctamente.");
    }
}
