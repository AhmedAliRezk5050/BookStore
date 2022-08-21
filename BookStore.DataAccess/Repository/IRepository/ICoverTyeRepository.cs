using BookStore.Models;

namespace BookStore.DataAccess.Repository.IRepository;

public interface ICoverTyeRepository : IRepository<CoverType>, IDisposable
{
    void Update(CoverType coverType);
}