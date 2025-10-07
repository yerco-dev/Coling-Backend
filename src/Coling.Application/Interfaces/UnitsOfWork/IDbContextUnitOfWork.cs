namespace Coling.Aplication.Interfaces.UnitsOfWork;

public interface IDbContextUnitOfWork : IDisposable
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task<int> SaveChangesAsync();
}