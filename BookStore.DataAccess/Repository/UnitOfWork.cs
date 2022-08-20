using BookStore.DataAccess.Repository.IRepository;

namespace BookStore.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;

    private bool _disposed = false;

    public ICategoryRepository CategoryRepository { get; private set; }

    public UnitOfWork(DataContext context)
    {
        _context = context;

        CategoryRepository = new CategoryRepository(_context);
    }
    
    
    public void Save()
    {
        _context.SaveChanges();
    }

    public Task SaveAsync()
    {
        return _context.SaveChangesAsync();
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}