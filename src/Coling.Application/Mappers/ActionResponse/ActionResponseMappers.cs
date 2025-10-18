using Coling.Domain.Wrappers;

namespace Coling.Application.Mappers.ActionResponse;

public static class ActionResponseMappers
{
    public static ActionResponse<Tout> ChangeNullActionResponseType<Tin, Tout>(this ActionResponse<Tin> actionResponse, string? message = null)
    {
        return new ActionResponse<Tout>
        {
            Message = message ?? actionResponse.Message,
            ResultCode = actionResponse.ResultCode,
            Errors = actionResponse.Errors,
            WasSuccessful = actionResponse.WasSuccessful
        };
    }
}
