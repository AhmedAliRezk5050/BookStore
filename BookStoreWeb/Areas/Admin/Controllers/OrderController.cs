using System.Collections;
using System.Security.Claims;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class OrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index(string? status)
    {
        return View();
    }

    #region API

    public async Task<IActionResult> GetAll()
    {
        List<Order> orders;
        if (User.IsInRole(SD.AdminRole) || User.IsInRole(SD.EmployeeRole))
        {
            orders = await _unitOfWork.OrderRepository.GetAllAsync(includedProperties: "ApplicationUser");
        }
        else
        {
            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var currentUserId = userClaim!.Value;
            orders = await _unitOfWork.OrderRepository.
                GetAllAsync(o => o.ApplicationUserId == currentUserId, includedProperties: "ApplicationUser");
        }


        return Json(orders);
    }

    #endregion
}