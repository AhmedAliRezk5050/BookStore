using System.Security.Claims;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Utility;
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

    public async Task<IActionResult> Summary()
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
        var user = await _unitOfWork.ApplicationUserRepository
            .GetFirstOrDefaultAsync(u => u.Id == currentUserId);
        if (user is null) return NotFound();

        var summaryViewModel = new SummaryViewModel()
        {
            Carts = carts,
            TotalPrice = carts.Sum(c => c.Price * c.Count),
            Name = user.Name ?? "",
            PhoneNumber = user.PhoneNumber,
            StreetAddress = user.StreetAddress ?? "",
            City = user.City ?? "",
            State = user.State ?? "",
            PostalCode = user.PostalCode ?? ""
        };

        return View(summaryViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Summary(SummaryViewModel model)
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
        model.Carts = carts;
        model.TotalPrice = carts.Sum(c => c.Price * c.Count);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var order = new Order()
        {
            ApplicationUserId = currentUserId,
            Name = model.Name,
            PhoneNumber = model.PhoneNumber,
            StreetAddress = model.StreetAddress,
            City = model.City,
            State = model.State,
            PostalCode = model.PostalCode,
            OrderTotal = model.TotalPrice,
            PaymentStatus = SD.StatusPending,
            OrderStatus = SD.StatusPending,
            OrderDate = DateTime.Now
        };

        _unitOfWork.OrderRepository.Add(order);
        await _unitOfWork.SaveAsync();

        foreach (var cart in model.Carts)
        {
            var orderDetail = new OrderDetail()
            {
                OrderId = order.Id,
                ProductId = cart.ProductId,
                Price = cart.Price,
                Quantity = cart.Count
            };
            _unitOfWork.OrderDetailRepository.Add(orderDetail);
        }

        await _unitOfWork.SaveAsync();

        _unitOfWork.ShoppingCartRepository.RemoveRange(model.Carts);
        await _unitOfWork.SaveAsync();

        return RedirectToAction("Index", "Home");
    }

    #region API

    public async Task<IActionResult> Increment(int id)
    {
        var cart = await _unitOfWork.ShoppingCartRepository
            .GetFirstOrDefaultAsync(c => c.Id == id);
        if (cart is null) return BadRequest();
        _unitOfWork.ShoppingCartRepository.IncrementCount(cart);
        await _unitOfWork.SaveAsync();
        return Ok(new { count = cart.Count, price = cart.Price });
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
        return Ok(new { count = cart.Count });
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

    #endregion
}