using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository.IRepository
{
    public interface IOrderRepository :  IRepository<Order>, IDisposable
    {
        void Update(Order order);
        
        Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        
        Task UpdateStripePayment(int id, string sessionId, string paymentIntentId);
    }
}
