namespace BookStore.DataAccess.Repository.IRepository;

public interface IUnitOfWork : IDisposable
{
    ICategoryRepository CategoryRepository { get; }
    
    ICoverTyeRepository CoverTyeRepository { get; }

    void Save();
    
    Task SaveAsync();
}