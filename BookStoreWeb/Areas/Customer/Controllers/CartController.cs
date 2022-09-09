using System.Security.Claims;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace BookStoreWeb.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<IdentityUser> _userManager;
    public CartViewModel CartViewModel { get; set; } = new();

    public CartController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
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

        var user = await _unitOfWork.ApplicationUserRepository
            .GetFirstOrDefaultAsync(u => u.Id == currentUserId);

        var isCompanyUser = await IsCompanyUser(user!);

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
            PaymentStatus = isCompanyUser ? SD.PaymentStatusDelayedPayment : SD.StatusPending,
            OrderStatus = isCompanyUser ? SD.StatusApproved : SD.StatusPending,
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
        // ----------------
        // ----------------
        // ----------------

        // -------If company user ---------
        if (isCompanyUser)
        {
            return RedirectToAction(nameof(OrderConfirmation), new { id = order.Id });
        }
        // ----------------

        // ----------------
        // ----------------
        // ----------------
        var baseUrl = string.Format("{0}://{1}",
                       HttpContext.Request.Scheme, HttpContext.Request.Host);

        var options = new SessionCreateOptions
        {
            LineItems = new() { },
            Mode = "payment",
            SuccessUrl = baseUrl + $"/Customer/Cart/OrderConfirmation?id={order.Id}",
            CancelUrl = baseUrl + "/Customer/Cart/Index",
        };

        foreach (var cart in model.Carts)
        {
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)cart.Price * 1000,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = cart.Product.Title,
                    },
                },
                Quantity = cart.Count,
            });
        }

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        _unitOfWork.OrderRepository.UpdateStripePayment(order, session.Id, session.PaymentIntentId);
        await _unitOfWork.SaveAsync();
        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }

    public async Task<IActionResult> OrderConfirmation(int id)
    {
        var order = await _unitOfWork.OrderRepository.GetFirstOrDefaultAsync(o => o.Id == id);

        if (order is null) return NotFound();

        if (order.PaymentStatus != SD.PaymentStatusDelayedPayment)
        {
            var service = new SessionService();
            var session = await service.GetAsync(order.SessionId);

            // check stripe status
            if (session.PaymentStatus.ToLower() == "paid")
            {
                await _unitOfWork.OrderRepository.UpdateStatus(
                    SD.StatusApproved, paymentStatus: SD.StatusApproved, order: order);
                await _unitOfWork.SaveAsync();
            }
        }

        var carts = await _unitOfWork.ShoppingCartRepository
            .GetAllAsync(c => c.ApplicationUserId == order.ApplicationUserId);
        _unitOfWork.ShoppingCartRepository.RemoveRange(carts);
        await _unitOfWork.SaveAsync();
        HttpContext.Session.Clear();
        return View(id);
    }

    public Task<bool> IsCompanyUser(ApplicationUser user) => _userManager.IsInRoleAsync(user, SD.CompanyUserRole);

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
            DecrementCartSession();
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
        DecrementCartSession();
        return Ok(new { count = cart.Count });
    }

    #endregion

    private void DecrementCartSession()
    {
        var shoppingCartCount = HttpContext.Session.GetInt32(SD.ShoppingCartCount) ?? 0;
        if (shoppingCartCount > 0)
        {
            HttpContext.Session.SetInt32(SD.ShoppingCartCount, shoppingCartCount - 1);
        }
    }
}