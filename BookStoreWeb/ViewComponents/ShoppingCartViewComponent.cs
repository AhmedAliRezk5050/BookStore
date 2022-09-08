using System.Security.Claims;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Utility;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreWeb.ViewComponents;

public class ShoppingCartViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;

    public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userClaim = UserClaimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
        var currentUserId = userClaim?.Value;

        // if
        if (currentUserId is null)
        {
            HttpContext.Session.Clear();
            return View(0);
        }

        var cartsCount = (await _unitOfWork.ShoppingCartRepository
            .GetAllAsync(c => c.ApplicationUserId == currentUserId)).Count;

        HttpContext.Session.SetInt32(SD.ShoppingCartCount, cartsCount);
        
        return View(HttpContext.Session.GetInt32(SD.ShoppingCartCount) ?? 0);
    }
}