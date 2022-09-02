using BookStore.DataAccess.Repository.IRepository;

namespace BookStore.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;

    private bool _disposed = false;

    public ICategoryRepository CategoryRepository { get; private set; }
    
    public ICoverTyeRepository CoverTyeRepository { get; private set;}

    public IProductRepository ProductRepository { get; private set;}

    public ICompanyRepository CompanyRepository { get; private set;}

    public IShoppingCartRepository ShoppingCartRepository { get; private set;}

    public IApplicationUserRepository ApplicationUserRepository { get; private set;}

    public UnitOfWork(DataContext context)
    {
        _context = context;

        CategoryRepository = new CategoryRepository(_context);
        
        CoverTyeRepository = new CoverTypeRepository(_context);

        ProductRepository = new ProductRepository(_context);

        CompanyRepository = new CompanyRepository(_context);

        ShoppingCartRepository = new ShoppingCartRepository(_context);

        ApplicationUserRepository = new ApplicationUserRepository(_context);
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