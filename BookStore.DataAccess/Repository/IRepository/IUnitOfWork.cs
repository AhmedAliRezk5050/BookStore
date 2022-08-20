namespace BookStore.DataAccess.Repository.IRepository;

public interface IUnitOfWork : IDisposable
{
    ICategoryRepository CategoryRepository { get; }

    void Save();
    
    Task SaveAsync();
}