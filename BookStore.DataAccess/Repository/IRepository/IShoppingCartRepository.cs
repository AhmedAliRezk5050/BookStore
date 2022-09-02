using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository :  IRepository<ShoppingCart>, IDisposable
    {
        void IncrementCount(ShoppingCart shoppingCart, int count = 1);
        void DecrementCount(ShoppingCart shoppingCart, int count = 1);
    }
}
