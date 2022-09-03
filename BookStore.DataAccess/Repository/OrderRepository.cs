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

        public async Task  UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orderFromDb is not null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (paymentStatus is not null)
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
                dbSet.Update(orderFromDb);
            }
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