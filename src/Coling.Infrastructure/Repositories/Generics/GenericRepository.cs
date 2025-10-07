using Coling.Domain.Entities.ActionResponse;
using Coling.Domain.Interfaces.BaseEntities;
using Coling.Domain.Interfaces.Repositories.Generics;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using Coling.Infrastructure.Data;

namespace Coling.Infrastructure.Repositories.Generics;


public class GenericRepository<T> : IGenericRepository<T> where T : class, IBaseEntity
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _entity;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _entity = _context.Set<T>();
    }
    public virtual async Task<ActionResponse<T>> AddAsync(T entity)
    {
        _context.Add(entity);

        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<T>
            {
                WasSuccessful = true,
                Result = entity
            };
        }
        catch (DbUpdateException)
        {
            return DbUpdateExceptionActionResponse();
        }
        catch (Exception exception)
        {
            return ExceptionActionResponse(exception);
        }
    }


    public virtual async Task<ActionResponse<T>> DeleteAsync(Guid id)
    {
        var data = await _entity.FindAsync(id);

        if (data == null)
            return new ActionResponse<T>
            {
                WasSuccessful = false,
                Message = "Registro no encontrado.",
                ResultCode = ResultCode.NotFound,
            };

        data.IsActive = false;

        return await UpdateAsync(data);
    }

    public virtual async Task<ActionResponse<T>> GetAsync(Guid id, bool includeDeleteds = true)
    {
        var data =
            includeDeleteds ?
                await _entity.FindAsync(id) :
                await _entity.Where(e => e.IsActive).FirstOrDefaultAsync(e => e.Id == id);

        if (data == null)
            return new ActionResponse<T>
            {
                WasSuccessful = false,
                Message = "Registro no encontrado.",
                ResultCode = ResultCode.NotFound
            };
        else
            return new ActionResponse<T>
            {
                WasSuccessful = true,
                Result = data
            };
    }


    public virtual async Task<ActionResponse<IEnumerable<T>>> GetAsync(bool includeDeleteds = false)
    {
        return new ActionResponse<IEnumerable<T>>()
        {
            WasSuccessful = true,
            Result = await _entity.Where(e => e.IsActive || includeDeleteds).ToListAsync()
        };
    }

    public virtual async Task<ActionResponse<IEnumerable<T>>> GetDeleteds()
    {
        return new ActionResponse<IEnumerable<T>>()
        {
            WasSuccessful = true,
            Result = await _entity.Where(e => e.IsActive).ToListAsync()
        };
    }

    public virtual async Task<ActionResponse<T>> GetAsync(Expression<Func<T, bool>> predicate)
    {
        var entity = await _entity.FirstOrDefaultAsync(predicate);

        if (entity == null)
        {
            return new ActionResponse<T>
            {
                WasSuccessful = false,
                Result = entity,
                Message = "No encontrado",
                ResultCode = ResultCode.NotFound
            };
        }
        return new ActionResponse<T>
        {
            WasSuccessful = true,
            Result = entity
        };
    }

    public virtual async Task<ActionResponse<T>> RestoreRegisterAsync(Guid id)
    {
        var data = await _entity.FindAsync(id);

        if (data == null)
            return new ActionResponse<T>
            {
                WasSuccessful = false,
                Message = "Registro no encontrado.",
                ResultCode = ResultCode.NotFound,
            };

        data.IsActive = true;

        return await UpdateAsync(data);
    }

    public virtual async Task<ActionResponse<T>> UpdateAsync(T entity)
    {

        _context.Update(entity);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<T>
            {
                WasSuccessful = true,
                Result = entity
            };
        }
        catch (DbUpdateException)
        {
            return DbUpdateExceptionActionResponse();
        }
        catch (Exception exception)
        {
            return ExceptionActionResponse(exception);
        }

    }

    protected virtual ActionResponse<T> DbUpdateExceptionActionResponse()
    {
        return new ActionResponse<T>
        {
            WasSuccessful = false,
            Message = "Error en la base de datos.",
            ResultCode = ResultCode.DatabaseError
        };
    }

    protected virtual ActionResponse<T> ExceptionActionResponse(Exception exception)
    {
        return new ActionResponse<T>
        {
            WasSuccessful = false,
            Message = exception.Message,
            ResultCode = ResultCode.DatabaseError
        };
    }
}

