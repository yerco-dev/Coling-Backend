
using Coling.Application.Mappers.ActionResponse;
using Coling.Domain.Entities.ActionResponse;
using Coling.Domain.Interfaces.BaseEntities;
using Coling.Domain.Interfaces.Repositories.Generics;
using System.Linq.Expressions;

namespace Coling.Aplication.Validators;

public static class GuidValidators
{
    public static async Task<ActionResponse<Guid>> IdValidator<T>(
        this string? id,
        IGenericRepository<T> repository,
        bool includeDeleteds = false) where T : IBaseEntity
    {
        if (id == null || !Guid.TryParse(id, out Guid guid))
            return ActionResponse<Guid>.Failure("El identificador no es válido", ResultCode.InputError);

        return await guid.IdValidator(repository, includeDeleteds);
    }

    public static async Task<ActionResponse<Guid>> IdValidator<T>(
        this Guid id,
        IGenericRepository<T> repository,
        bool includeDeleteds = false) where T : IBaseEntity
    {
        var existingEntity = await id.GetIdValidatedEntity(repository, includeDeleteds);

        if (!existingEntity.WasSuccessful)
            return existingEntity.ChangeNullActionResponseType<T, Guid>();

        return ActionResponse<Guid>.Success(id);
    }

    public static async Task<ActionResponse<T>> GetIdValidatedEntity<T>(
        this Guid id,
        IGenericRepository<T> repository,
        bool includeDeleteds = false) where T : IBaseEntity
    {
        var existingEntity = await repository.GetAsync(id, includeDeleteds);

        if (!existingEntity.WasSuccessful)
            return ActionResponse<T>.NotFound("El identificador no corresponde con una entidad válida");

        return ActionResponse<T>.Success(existingEntity.Result);
    }

    public static async Task<ActionResponse<Guid>> IdValidator<T>(
        this string? id,
        IGenericRepository<T> repository,
        Expression<Func<T, bool>> predicate,
        bool includeDeleteds = false) where T : IBaseEntity
    {
        if (id == null || !Guid.TryParse(id, out Guid guid))
            return ActionResponse<Guid>.Failure("El identificador no es válido", ResultCode.InputError);

        return await guid.IdValidator(repository, predicate, includeDeleteds);
    }


    public static async Task<ActionResponse<Guid>> IdValidator<T>(
        this Guid id,
        IGenericRepository<T> repository,
        Expression<Func<T, bool>> predicate,
        bool includeDeleteds = false) where T : IBaseEntity
    {
        var existingEntity = await id.GetIdValidatedEntity(repository, predicate, includeDeleteds);

        if (!existingEntity.WasSuccessful)
            return existingEntity.ChangeNullActionResponseType<T, Guid>();

        return ActionResponse<Guid>.Success(id);
    }

    public static async Task<ActionResponse<T>> GetIdValidatedEntity<T>(
        this Guid id,
        IGenericRepository<T> repository,
        Expression<Func<T, bool>> predicate,
        bool includeDeleteds = false) where T : IBaseEntity
    {
        var existingEntity = await repository.GetAsync(predicate);

        if (!existingEntity.WasSuccessful || (existingEntity.Result!.IsActive == false && includeDeleteds))
            return ActionResponse<T>.NotFound("El identificador no corresponde con una entidad válida");

        return ActionResponse<T>.Success(existingEntity.Result);
    }

    public static async Task<ActionResponse<T>> GetIdValidatedEntity<T>(
        this string id,
        IGenericRepository<T> repository,
        Expression<Func<T, bool>> predicate,
        bool includeDeleteds) where T : IBaseEntity
    {
        if (!Guid.TryParse(id, out Guid guid))
            return ActionResponse<T>.Failure("El identificador no es válido", ResultCode.InputError);

        return await guid.GetIdValidatedEntity(repository, predicate, includeDeleteds);
    }
}
