namespace BookStore.DataAccess.Repository.IRepository;

public interface IUnitOfWork : IDisposable
{
    ICategoryRepository CategoryRepository { get; }
    
    ICoverTyeRepository CoverTyeRepository { get; }

    IProductRepository ProductRepository { get; }

    ICompanyRepository CompanyRepository { get; }

    void Save();
    
    Task SaveAsync();
}