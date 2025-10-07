
using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coling.Infrastructure.UnitsOfWork;

public class DbContextUnitOfWork : IDbContextUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public DbContextUnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
        if (_transaction != null)
            await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
            await _transaction.RollbackAsync();
    }

    public async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();


    public void Dispose() => _context.Dispose();
}
