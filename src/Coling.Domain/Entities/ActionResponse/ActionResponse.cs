namespace Coling.Domain.Entities.ActionResponse;

public enum ResultCode
{
    Ok = 200,
    NotFound = 404,
    DatabaseError = 500,
    InputError = 400,
    Conflict = 409,
    Unauthorized = 401,
    Forbidden = 403
}

public class ActionResponse<T>
{
    public bool WasSuccessful { get; set; } = true;
    public string? Message { get; set; }
    public ResultCode ResultCode { get; set; } = ResultCode.Ok;
    public T? Result { get; set; }
    public List<string>? Errors { get; set; }

    public static ActionResponse<T> Success(T result, string? message = null)
    {
        return new ActionResponse<T>
        {
            WasSuccessful = true,
            Result = result,
            ResultCode = ResultCode.Ok,
            Message = message
        };
    }

    public static ActionResponse<T> Success(string? message = null)
    {
        return new ActionResponse<T>
        {
            WasSuccessful = true,
            ResultCode = ResultCode.Ok,
            Message = message
        };
    }

    public static ActionResponse<T> Failure(string message, ResultCode code = ResultCode.DatabaseError)
    {
        return new ActionResponse<T>
        {
            WasSuccessful = false,
            Message = message,
            ResultCode = code
        };
    }

    public static ActionResponse<T> Failure(string message, List<string> errors, ResultCode code = ResultCode.InputError)
    {
        return new ActionResponse<T>
        {
            WasSuccessful = false,
            Message = message,
            ResultCode = code,
            Errors = errors
        };
    }

    public static ActionResponse<T> NotFound(string message = "Registro no encontrado.")
    {
        return new ActionResponse<T>
        {
            WasSuccessful = false,
            Message = message,
            ResultCode = ResultCode.NotFound
        };
    }

    public static ActionResponse<T> Conflict(string message)
    {
        return new ActionResponse<T>
        {
            WasSuccessful = false,
            Message = message,
            ResultCode = ResultCode.Conflict
        };
    }
}

