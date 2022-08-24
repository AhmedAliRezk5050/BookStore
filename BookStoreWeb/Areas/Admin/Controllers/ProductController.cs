﻿using BookStore.DataAccess;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;

namespace BookStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<ProductController> _logger;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, ILogger<ProductController> logger,
            IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            ProductViewModel productViewModel = new();
            productViewModel.CategoriesSelectList = (await _unitOfWork.CategoryRepository
                .GetAllAsync()).Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });

            productViewModel.CoverTypesSelectList = (await _unitOfWork.CoverTyeRepository
                .GetAllAsync()).Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });

            if (id is null or 0)
            {
                return View(productViewModel);
            }
            else
            {
                productViewModel.Product = await _unitOfWork.ProductRepository.GetFirstOrDefaultAsync(p => p.Id == id);
                return View(productViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductViewModel productViewModel, IFormFile? formFile)
        {
            if (productViewModel.Product?.Id is 0)
            {
                // create
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (formFile is not null)
                        {
                            string? uniqueFileName = UploadedFile(formFile);

                            productViewModel.Product!.ImageUrl = uniqueFileName;
                        }

                        _unitOfWork.ProductRepository.Add(productViewModel.Product!);
                        await _unitOfWork.SaveAsync();
                        TempData["success"] = "Product created successfully";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occurred while creating the product.");

                    ModelState.AddModelError("", "Unable to save changes. " +
                                                 "Try again, and if the problem persists, " +
                                                 "see your system administrator.");
                }
            }
            else
            {
                // edit
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (formFile is not null)
                        {
                            string? productOldImgUrl = (await _unitOfWork.ProductRepository
                                .GetFirstOrDefaultAsync(p => p.Id == productViewModel.Product!.Id))!.ImageUrl;

                            string? toDeleteOldProductImg = Path.Combine(_webHostEnvironment.WebRootPath, "images",
                                "products", productOldImgUrl!);
                            System.IO.File.Delete(toDeleteOldProductImg);
                            string? uniqueFileName = UploadedFile(formFile);
                            productViewModel.Product!.ImageUrl = uniqueFileName;
                        }

                        await _unitOfWork.ProductRepository.Update(productViewModel.Product!);
                        await _unitOfWork.SaveAsync();
                        TempData["success"] = "Product updated successfully";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occurred while updating the product.");
                    ModelState.AddModelError("", "Unable to update changes. " +
                                                 "Try again, and if the problem persists, " +
                                                 "see your system administrator.");
                }
            }

            productViewModel.CategoriesSelectList = (await _unitOfWork.CategoryRepository
                .GetAllAsync()).Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });

            productViewModel.CoverTypesSelectList = (await _unitOfWork.CoverTyeRepository
                .GetAllAsync()).Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });

            return View(productViewModel);
        }

        #region API CALLS

        public async Task<IActionResult> GetAll()
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync("Category,CoverType");
            return Json(new { data = products });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var product = await _unitOfWork.ProductRepository.GetFirstOrDefaultAsync(c => c.Id == id);

            if (product == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            try
            {
                _unitOfWork.ProductRepository.Remove(product);
                await _unitOfWork.SaveAsync();
                string toDeleteImgPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products",
                    product.ImageUrl!);
                System.IO.File.Delete(toDeleteImgPath);
                return Json(new { success = true, message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Category.");
                return Json(new { success = false, message = "Error while deleting" });
            }
        }

        #endregion 

        private void AddDeletionFailureTempData()
        {
            TempData["error"] =
                "Delete failed. Try again, and if the problem persists " +
                "see your system administrator.";
        }

        private string UploadedFile(IFormFile formFile)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                formFile.CopyTo(fileStream);
            }

            return uniqueFileName;
        }
    }
}