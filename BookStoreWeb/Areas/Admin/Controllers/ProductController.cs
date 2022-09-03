using BookStore.DataAccess;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using NuGet.Packaging;
using AutoMapper;
using Newtonsoft.Json.Bson;

namespace BookStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<ProductController> _logger;

        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly IMapper _mapper;


        public ProductController(IUnitOfWork unitOfWork, ILogger<ProductController> logger,
            IWebHostEnvironment webHostEnvironment, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            UpsertProductViewModel upsertProductViewModel = new();

            upsertProductViewModel.CategoriesSelectList = (await _unitOfWork.CategoryRepository
                .GetAllAsync()).Select(c =>
                                new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });

            upsertProductViewModel.CoverTypesSelectList = (await _unitOfWork.CoverTyeRepository
                .GetAllAsync()).Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });


            if (id is null or 0)
            {
                return View(upsertProductViewModel);
            }
            else
            {
                var product = await _unitOfWork.ProductRepository.GetFirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return NotFound();
                }

                return View(_mapper.Map(product, upsertProductViewModel));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(UpsertProductViewModel upsertProductViewModel, IFormFile? formFile)
        {
            if (!ModelState.IsValid)
            {
                upsertProductViewModel.CategoriesSelectList = (await _unitOfWork.CategoryRepository
              .GetAllAsync()).Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });

                upsertProductViewModel.CoverTypesSelectList = (await _unitOfWork.CoverTyeRepository
                    .GetAllAsync()).Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });

                return View(upsertProductViewModel);
            }

            Product product = new();

            if (upsertProductViewModel.Id is 0)
            {
                try
                {
                    if (formFile != null)
                    {
                        string? uniqueFileName = UploadedFile(formFile);
                        upsertProductViewModel.ImageUrl = Path.Join("/images", "products", uniqueFileName);
                    }

                    _unitOfWork.ProductRepository.Add(_mapper.Map(upsertProductViewModel, product));
                    await _unitOfWork.SaveAsync();
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occurred while creating the product.");
                    DeleteProduct(product.ImageUrl);
                    ModelState.AddModelError("", "Unable to save changes. " +
                                                 "Try again, and if the problem persists, " +
                                                 "see your system administrator.");
                    return View(upsertProductViewModel);
                }
            }

            // edit
            try
            {
                if (formFile is not null)
                {
                    string? oldImgUrl = (await _unitOfWork.ProductRepository
                        .GetFirstOrDefaultAsync(p => p.Id == upsertProductViewModel.Id))!.ImageUrl;

                    DeleteProduct(oldImgUrl);

                    string? uniqueFileName = UploadedFile(formFile);
                    upsertProductViewModel.ImageUrl = Path.Join("/images", "products", uniqueFileName);
                }

                await _unitOfWork.ProductRepository.Update(_mapper.Map(upsertProductViewModel, product));
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while updating the product.");
                ModelState.AddModelError("", "Unable to update changes. " +
                                             "Try again, and if the problem persists, " +
                                             "see your system administrator.");
                return View(upsertProductViewModel);
            }
        }

        #region API CALLS

        public async Task<IActionResult> GetAll()
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(includedProperties: "Category,CoverType");
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

                DeleteProduct(product.ImageUrl);

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

        /// <summary>
        /// delete product by imgage url which combined with the absolute path of the directory
        /// </summary>
        /// <param name="imgUrl"></param>
        private void DeleteProduct(string imgUrl)
        {
            if (imgUrl == null) return;
            string toDeleteImgPath = Path.Join(_webHostEnvironment.WebRootPath, imgUrl);
            System.IO.File.Delete(toDeleteImgPath);
        }
    }
}
