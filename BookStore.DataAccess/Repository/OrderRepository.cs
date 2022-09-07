using BookStore.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;


namespace BookStore.DataAccess.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly DataContext _context;
        
        private bool _disposed = false;
        
        public OrderRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Order order)
        {
            dbSet.Update(order);
        }

        public async Task UpdateStatus(string orderStatus, int? id = null, Order? order = null, string? paymentStatus = null)
        {
            Order? toUpdateOrder = null;
            
            if (order is null)
            {
                 toUpdateOrder = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            }
            else
            {
                toUpdateOrder = order;
            }

            if (toUpdateOrder is null) return;
            
            toUpdateOrder.OrderStatus = orderStatus;
            if (paymentStatus != null)
            {
                toUpdateOrder.PaymentStatus = paymentStatus;
            }
            dbSet.Update(toUpdateOrder);
        }

        public void  UpdateStripePayment(Order order, string sessionId, string paymentIntentId)
        {
            order.PaymentDate = DateTime.Now;
            order.SessionId = sessionId;
            order.PaymentIntentId = paymentIntentId;
            dbSet.Update(order);
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
}