using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModels;
using System.Security.Claims;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;

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
                .GetAllAsync(includedProperties: "Category,CoverType");

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id is 0)
            {
                return NotFound();
            }

            var product = await _unitOfWork.ProductRepository
                .GetFirstOrDefaultAsync(p => p.Id == id, "Category,CoverType");

            if (product is null)
            {
                return NotFound();
            }

            return View(new AddShoppingCartViewModel() { Product = product });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(AddShoppingCartViewModel model)
        {
            var id = model.Product.Id;
            var product = await FetchProduct(id);
            if (product is null) return NotFound();

            if (!ModelState.IsValid)
            {
                model.Product = product;
                return View(model);
            }

            try
            {
                var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                var currentUserId = userClaim!.Value;
                //
                var sc = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefaultAsync(sc =>
                        sc.ApplicationUserId == currentUserId && sc.ProductId == product.Id);
                if (sc is not null)
                {
                    _unitOfWork.ShoppingCartRepository.IncrementCount(sc, model.Count ?? 0);
                }
                else
                {
                    var shoppingCart = new ShoppingCart()
                    {
                        Count = model.Count ?? 1,
                        ProductId = id,
                        ApplicationUserId = currentUserId
                    };
                    _unitOfWork.ShoppingCartRepository.Add(shoppingCart);
                }
                
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Product added to shopping card successfully";
                HttpContext.Session.SetInt32(SD.ShoppingCartCount, 
                    (await _unitOfWork.ShoppingCartRepository
                        .GetAllAsync()).ToList().Count);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while adding the product to the shopping card");
                ModelState.AddModelError("", "Unable to add the product. " +
                                             "Try again, and if the problem persists, " +
                                             "see your system administrator.");
                model.Product = product;
                return View(model);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<Product?> FetchProduct(int id) => await _unitOfWork.ProductRepository
            .GetFirstOrDefaultAsync(p => p.Id == id, "Category,CoverType");
    }
}