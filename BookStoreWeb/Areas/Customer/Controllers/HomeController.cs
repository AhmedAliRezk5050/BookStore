using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;

namespace BookStoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.ProductRepository
                                                  .GetAllAsync("Category,CoverType");

            return View(products);
        }

        public async Task<IActionResult> Details(int? id)
        {

            
            if (id is 0 or null)
            {
                return NotFound();
            }

            var shoppingCart = new ShoppingCart()
            {
                Product = await _unitOfWork.ProductRepository
                    .GetFirstOrDefaultAsync(
                        p => p.Id == id,
                        "Category,CoverType")
            };
            
            return View(shoppingCart);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}