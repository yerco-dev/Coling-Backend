using Coling.Domain.Entities.WorkManagement;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.Validators;

public static class WorkFieldValidator
{
    public static async Task<ActionResponse<Guid>> ValidateWorkFieldCategoryExists(
        this Guid workFieldCategoryId, IWorkFieldCategoryRepository repository)
    {
        var category = await repository.GetAsync(workFieldCategoryId);

        if (!category.WasSuccessful)
            return ActionResponse<Guid>.NotFound("La categoría de campo de trabajo no existe.");

        return ActionResponse<Guid>.Success(workFieldCategoryId);
    }

    public static async Task<ActionResponse<WorkFieldCategory>> ValidateDuplicateWorkFieldCategory(
        this string categoryName, IWorkFieldCategoryRepository repository)
    {
        var existing = await repository.GetAsync(c =>
            c.Name.ToLower() == categoryName.ToLower() && c.IsActive);

        if (existing.WasSuccessful)
            return ActionResponse<WorkFieldCategory>.Conflict("Ya existe una categoría de campo de trabajo con ese nombre.");

        return ActionResponse<WorkFieldCategory>.Success(null!);
    }

    public static async Task<ActionResponse<WorkField>> ValidateDuplicateWorkField(
        this string workFieldName, IWorkFieldRepository repository)
    {
        var existing = await repository.GetAsync(wf =>
            wf.Name.ToLower() == workFieldName.ToLower() && wf.IsActive);

        if (existing.WasSuccessful)
            return ActionResponse<WorkField>.Conflict("Ya existe un campo de trabajo con ese nombre.");

        return ActionResponse<WorkField>.Success(null!);
    }

    public static async Task<ActionResponse<List<Guid>>> ValidateWorkFieldsExist(
        this List<Guid> workFieldIds, IWorkFieldRepository repository)
    {
        if (workFieldIds == null || !workFieldIds.Any())
            return ActionResponse<List<Guid>>.Failure("Debe seleccionar al menos un campo de trabajo.", ResultCode.InputError);

        var invalidIds = new List<Guid>();
        foreach (var id in workFieldIds)
        {
            var workField = await repository.GetAsync(id);
            if (!workField.WasSuccessful)
                invalidIds.Add(id);
        }

        if (invalidIds.Any())
            return ActionResponse<List<Guid>>.NotFound($"Los siguientes IDs de campos de trabajo no existen: {string.Join(", ", invalidIds)}");

        return ActionResponse<List<Guid>>.Success(workFieldIds);
    }
}
