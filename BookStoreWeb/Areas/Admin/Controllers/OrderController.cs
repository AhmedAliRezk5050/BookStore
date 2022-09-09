using System.Collections;
using System.Security.Claims;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

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

    public async Task<IActionResult> Details(int id)
    {
        var order = await _unitOfWork.OrderRepository.GetFirstOrDefaultAsync(o => o.Id == id, "ApplicationUser");
        if (order == null) return NotFound();
        var orderDetails = await GetOrderDetails(order.Id);
        var orderDetailsViewModel = new OrderDetailsViewModel()
        {
            IsAdminOrEmployee = User.IsInRole(SD.AdminRole) || User.IsInRole(SD.EmployeeRole),
            Id = order.Id,
            Name = order.Name,
            PhoneNumber = order.PhoneNumber,
            StreetAddress = order.StreetAddress,
            City = order.City,
            State = order.State,
            PostalCode = order.PostalCode,
            Email = order.ApplicationUser.Email,
            OrderTotal = order.OrderTotal,
            OrderDate = order.OrderDate,
            Carrier = order.Carrier,
            TrackingNumber = order.TrackingNumber,
            ShippingDate = order.ShippingDate,
            PaymentDate = order.PaymentDate,
            PaymentDueDate = order.PaymentDueDate,
            SessionId = order.SessionId,
            PaymentIntentId = order.PaymentIntentId,
            PaymentStatus = order.PaymentStatus,
            OrderStatus = order.OrderStatus,
            OrderDetails = orderDetails,
        };

        return View(orderDetailsViewModel);
    }


    [HttpPost]
    [Authorize(Roles = SD.AdminRole + "," + SD.EmployeeRole)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderDetail(OrderDetailsViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.OrderDetails = await GetOrderDetails(model.Id);
            return View(nameof(Details), model);
        }

        var orderFromDb = await _unitOfWork.OrderRepository.GetFirstOrDefaultAsync(o => o.Id == model.Id);
        if (orderFromDb is null) return NotFound();

        orderFromDb.Name = model.Name;
        orderFromDb.PhoneNumber = model.PhoneNumber;
        orderFromDb.StreetAddress = model.StreetAddress;
        orderFromDb.City = model.City;
        orderFromDb.State = model.State;
        orderFromDb.PostalCode = model.PostalCode;
        orderFromDb.Carrier = model.Carrier;
        orderFromDb.TrackingNumber = model.TrackingNumber;

        _unitOfWork.OrderRepository.Update(orderFromDb);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Order details updated successfully";
        return RedirectToAction(nameof(Details), new { id = orderFromDb.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.AdminRole + "," + SD.EmployeeRole)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StartProcessing(OrderDetailsViewModel model)
    {
        await _unitOfWork.OrderRepository.UpdateStatus(SD.StatusInProcess, id: model.Id);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Order status updated successfully";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.AdminRole + "," + SD.EmployeeRole)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ShipOrder(OrderDetailsViewModel model)
    {
        if (model.TrackingNumber == null || model.Carrier == null)
        {
            if (model.TrackingNumber == null)
            {
                ModelState.AddModelError("TrackingNumber", "TrackingNumber is required");
            }

            if (model.Carrier == null)
            {
                ModelState.AddModelError("Carrier", "Carrier is required");
            }

            model.OrderDetails = await GetOrderDetails(model.Id);
            return View(nameof(Details), model);
        }

        var orderFromDb = await _unitOfWork.OrderRepository.GetFirstOrDefaultAsync(o => o.Id == model.Id);
        if (orderFromDb is null) return NotFound();

        orderFromDb.TrackingNumber = model.TrackingNumber;
        orderFromDb.Carrier = model.Carrier;
        orderFromDb.OrderStatus = SD.StatusShipped;
        orderFromDb.ShippingDate = DateTime.Now;
        if (orderFromDb.PaymentStatus == SD.PaymentStatusDelayedPayment)
        {
            orderFromDb.PaymentDueDate = DateTime.Now.AddDays(30);
        }

        _unitOfWork.OrderRepository.Update(orderFromDb);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Order status updated successfully";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.AdminRole + "," + SD.EmployeeRole)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelOrder(OrderDetailsViewModel model)
    {
        var orderFromDb = await _unitOfWork.OrderRepository.GetFirstOrDefaultAsync(o => o.Id == model.Id);
        if (orderFromDb is null) return NotFound();


        if (orderFromDb.PaymentStatus == SD.StatusApproved)
        {
            // -------- refund --------
            var options = new RefundCreateOptions()
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = model.PaymentIntentId
            };

            var service = new RefundService();
            var refund = await service.CreateAsync(options);
            await _unitOfWork.OrderRepository.UpdateStatus(
                orderStatus: SD.StatusCancelled,
                paymentStatus: SD.StatusRefunded,
                order: orderFromDb
            );
        }
        else
        {
            await _unitOfWork.OrderRepository.UpdateStatus(
                orderStatus: SD.StatusCancelled,
                paymentStatus: SD.StatusCancelled,
                order: orderFromDb
            );
        }

        await _unitOfWork.SaveAsync();
        TempData["success"] = "Order cancelled successfully";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DelayedPay(int orderId)
    {
        var order = await _unitOfWork.OrderRepository.GetFirstOrDefaultAsync(o => o.Id == orderId, "ApplicationUser");
        if (order == null) return NotFound();
        var orderDetails = await GetOrderDetails(order.Id);
        var baseUrl = string.Format("{0}://{1}",
                      HttpContext.Request.Scheme, HttpContext.Request.Host);
        var options = new SessionCreateOptions
        {
            LineItems = new() { },
            Mode = "payment",
            SuccessUrl = baseUrl + $"/admin/order/PaymentConfirmation?id={orderId}",
            CancelUrl = baseUrl + $"/admin/order/details?id={orderId}",
        };

        foreach (var orderDetail in orderDetails)
        {
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)orderDetail.Price * 1000,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = orderDetail.Product.Title,
                    },
                },
                Quantity = orderDetail.Quantity,
            });
        }

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        _unitOfWork.OrderRepository.UpdateStripePayment(order, session.Id, session.PaymentIntentId);
        await _unitOfWork.SaveAsync();
        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }

    public async Task<IActionResult> PaymentConfirmation(int id)
    {
        var order = await _unitOfWork.OrderRepository.GetFirstOrDefaultAsync(o => o.Id == id);

        if (order is null) return NotFound();

        if (order.PaymentStatus == SD.PaymentStatusDelayedPayment)
        {
            var service = new SessionService();
            var session = await service.GetAsync(order.SessionId);

            // check stripe status
            if (session.PaymentStatus.ToLower() == "paid")
            {
                await _unitOfWork.OrderRepository.UpdateStatus(
                    orderStatus: order.OrderStatus ?? SD.StatusApproved, paymentStatus: SD.StatusApproved,
                    order: order);
                await _unitOfWork.SaveAsync();
            }
        }
        return View(id);
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
            orders = await _unitOfWork.OrderRepository.GetAllAsync(o => o.ApplicationUserId == currentUserId,
                includedProperties: "ApplicationUser");
        }


        return Json(orders);
    }

    #endregion

    private Task<List<OrderDetail>> GetOrderDetails(int orderId) => _unitOfWork.OrderDetailRepository
        .GetAllAsync(o => o.OrderId == orderId, "Product");
}