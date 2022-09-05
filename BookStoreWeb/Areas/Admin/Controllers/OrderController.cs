using BookStore.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class OrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IActionResult> Index()
    {
        return View();
    }

    #region API
    public async Task<IActionResult> GetAll()
    {
        var orders = await _unitOfWork.OrderRepository.GetAllAsync(includedProperties: "OrderDetails,ApplicationUser");
        return Json(orders);
    }
    #endregion
}