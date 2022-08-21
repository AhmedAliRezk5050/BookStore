using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;

namespace BookStore.DataAccess.Repository;

public class CoverTypeRepository : Repository<CoverType>, ICoverTyeRepository
{
    private readonly DataContext _context;

    private bool _disposed = false;

    public CoverTypeRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public void Update(CoverType coverType)
    {
        dbSet.Update(coverType);
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