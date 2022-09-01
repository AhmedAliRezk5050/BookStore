using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {

        private readonly DataContext _context;

        private bool _disposed = false;

        public ProductRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task Update(Product productToUpdate)
        {
            var dbProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == productToUpdate.Id);

            if(dbProduct == null)
            {
                return;
            }

            dbProduct.Title = productToUpdate.Title;
            dbProduct.Description = productToUpdate.Description;
            dbProduct.ISBN = productToUpdate.ISBN;
            dbProduct.Author = productToUpdate.Author;
            dbProduct.ListPrice = productToUpdate.ListPrice;
            dbProduct.Price = productToUpdate.Price;
            dbProduct.Price50 = productToUpdate.Price50;
            dbProduct.Price100 = productToUpdate.Price100;
            dbProduct.CategoryId = productToUpdate.CategoryId;
            dbProduct.CoverTypeId = productToUpdate.CoverTypeId;

            if (productToUpdate.ImageUrl != null)
            {
                dbProduct.ImageUrl = productToUpdate.ImageUrl;
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
