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
        
        Task UpdateStatus(string orderStatus, int? id = null, Order? order = null, string? paymentStatus = null);
        
        void UpdateStripePayment(Order order, string sessionId, string paymentIntentId);
    }
}
