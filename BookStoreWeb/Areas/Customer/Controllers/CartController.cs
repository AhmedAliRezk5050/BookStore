using System.Security.Claims;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreWeb.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CartViewModel CartViewModel { get; set; } = new();

    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var currentUserId = userClaim!.Value;
        var carts = await _unitOfWork.ShoppingCartRepository.GetAllAsync(
            cart => cart.ApplicationUserId == currentUserId,
            includedProperties: "ApplicationUser,Product");

        carts.ForEach(c =>
        {
            c.Price = c.Count switch
            {
                <= 50 => c.Product.Price,
                < 100 => c.Product.Price50,
                _ => c.Product.Price100
            };
        });

        CartViewModel = new CartViewModel()
        {
            Carts = carts,
            TotalPrice = carts.Sum(c => c.Price * c.Count)
        };

        return View(CartViewModel);
    }

    public async Task<IActionResult> Increment(int id)
    {
        var cart = await _unitOfWork.ShoppingCartRepository
            .GetFirstOrDefaultAsync(c => c.Id == id);
        if (cart is null) return BadRequest();
        _unitOfWork.ShoppingCartRepository.IncrementCount(cart);
        await _unitOfWork.SaveAsync();
        return Ok(new { count = cart.Count, price =cart.Price });
    }


    public async Task<IActionResult> Decrement(int id)
    {
        var cart = await _unitOfWork.ShoppingCartRepository
            .GetFirstOrDefaultAsync(c => c.Id == id);

        if (cart is null) return BadRequest();

        _unitOfWork.ShoppingCartRepository.DecrementCount(cart);

        if (cart.Count == 0)
        {
            _unitOfWork.ShoppingCartRepository.Remove(cart);
        }

        await _unitOfWork.SaveAsync();
        return Ok(new { count = cart.Count});
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        if (id == 0)
        {
            return NotFound();
        }

        var cart = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefaultAsync(c => c.Id == id);

        if (cart == null)
        {
            return NotFound();
        }

        _unitOfWork.ShoppingCartRepository.Remove(cart);
        await _unitOfWork.SaveAsync();

        return Ok(new { count = cart.Count });
    }
}