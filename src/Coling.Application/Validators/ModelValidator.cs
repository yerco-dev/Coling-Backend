using Coling.Domain.Wrappers;
using System.ComponentModel.DataAnnotations;

namespace Coling.Aplication.Validators;

public static class ModelValidator
{
    public static ActionResponse<T> TryValidateModel<T>(this T model) where T : class
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);

        bool isValid = Validator.TryValidateObject(model, context, results, true);

        if (isValid)
            return ActionResponse<T>.Success(model);

        var errors = results.Select(r => r.ErrorMessage ?? "Error de validación").ToList();
        return ActionResponse<T>.Failure("Errores de validación del modelo.", errors, ResultCode.InputError);
    }
}
