namespace BookStore.DataAccess.Repository.IRepository;

public interface IUnitOfWork : IDisposable
{
    ICategoryRepository CategoryRepository { get; }
    
    ICoverTyeRepository CoverTyeRepository { get; }

    IProductRepository ProductRepository { get; }

    ICompanyRepository CompanyRepository { get; }

    IShoppingCartRepository ShoppingCartRepository { get; }

    IApplicationUserRepository ApplicationUserRepository { get; }
    
    IOrderRepository OrderRepository { get; }
    
    IOrderDetailRepository OrderDetailRepository { get; }

    void Save();
    
    Task SaveAsync();
}