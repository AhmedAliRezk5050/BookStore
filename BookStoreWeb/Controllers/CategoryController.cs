using BookStore.DataAccess;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(IUnitOfWork unitOfWork, ILogger<CategoryController> logger)
        {
            _unitOfWork = unitOfWork;
            
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();

            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name", "DisplayOrder, CreatedDateTime")] Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _unitOfWork.CategoryRepository.Add(category);
                    await _unitOfWork.SaveAsync();
                    TempData["success"] = "Category created successfully";
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the Category.");
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists " +
                                             "see your system administrator.");
            }

            return View(category);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null or 0)
            {
                return NotFound();
            }

            var category = await _unitOfWork.CategoryRepository.GetFirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, [Bind("Name", "DisplayOrder, CreatedDateTime")] Category category)
        {
            if (!IsValidId(id) || !await IsCategoryExist(id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.CategoryRepository.Update(category);
                    await _unitOfWork.SaveAsync();
                    TempData["success"] = "Category edited successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "An error occurred while updating the Category.");

                    ModelState.AddModelError("", "Unable to save changes. " +
                                                 "Try again, and if the problem persists, " +
                                                 "see your system administrator.");
                }
            }

            return View(category);
        }

        public async Task<IActionResult> Delete(int? id, bool? saveChangesError)
        {
            if (!IsValidId(id))
            {
                return NotFound();
            }

            var category = await _unitOfWork.CategoryRepository.GetFirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                AddDeletionFailureTempData();
            }

            return View(category!);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var category = await _unitOfWork.CategoryRepository.GetFirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                AddDeletionFailureTempData();
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _unitOfWork.CategoryRepository.Remove(category!);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Category deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Category.");

                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private void AddDeletionFailureTempData()
        {
            TempData["error"] =
                "Delete failed. Try again, and if the problem persists " +
                "see your system administrator.";
        }
        
        private static bool IsValidId(int? id) => id is not null or 0;

        private async Task<bool> IsCategoryExist(int? id)
        {
            var category = await _unitOfWork.CategoryRepository.GetFirstOrDefaultAsync(c => c.Id == id);
            return category != null;
        }
    }
}