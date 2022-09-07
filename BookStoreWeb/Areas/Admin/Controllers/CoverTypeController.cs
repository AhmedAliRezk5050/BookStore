using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CoverTypeController> _logger;

        public CoverTypeController(IUnitOfWork uow, ILogger<CoverTypeController> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var coverTypes = await _uow.CoverTyeRepository.GetAllAsync();
            return View(coverTypes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] CoverType coverType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _uow.CoverTyeRepository.Add(coverType);
                    await _uow.SaveAsync();
                    TempData["success"] = "Cover Type created successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while creating the Cover Type.");
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists " +
                                             "see your system administrator.");
            }

            return View(coverType);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null or 0)
            {
                return NotFound();
            }

            var coverType = await _uow.CoverTyeRepository.GetFirstOrDefaultAsync(c => c.Id == id);

            if (coverType is null)
            {
                return NotFound();
            }

            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id, Name")] CoverType coverType)
        {
            if (id != coverType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {


                    _uow.CoverTyeRepository.Update(coverType);
                    await _uow.SaveAsync();
                    TempData["success"] = "Cover Type edited successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    if (!await IsCoverTypeExist(id))
                    {
                        return NotFound();
                    }

                    _logger.LogError(e, "An error occurred while updating the Cover Type.");
                    ModelState.AddModelError("", "Unable to save changes. " +
                                                 "Try again, and if the problem persists, " +
                                                 "see your system administrator.");
                }
            }



            return View(coverType);
        }


        public async Task<IActionResult> Delete(int? id, bool saveChangesError = false)
        
        {
            if (id is null or 0)
            {
                return NotFound();
            }

            var coverType = await _uow.CoverTyeRepository.GetFirstOrDefaultAsync(c => c.Id == id);

            if (coverType is null)
            {
                return NotFound();
            }

            if(saveChangesError)
            {
                AddDeletionFailureTempData();
            }

            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null or 0)
            {
                return NotFound();
            }

            var coverType = await _uow.CoverTyeRepository.GetFirstOrDefaultAsync(c => c.Id == id);

            if (coverType is null)
            {
                AddDeletionFailureTempData();
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _uow.CoverTyeRepository.Remove(coverType);
                await _uow.SaveAsync();
                TempData["success"] = "Cover Type deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while deleting the Cover Type.");
                return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
            }
        }


        private async Task<bool> IsCoverTypeExist(int? id)
        {
            var coverType = await _uow.CoverTyeRepository.GetFirstOrDefaultAsync(c => c.Id == id);
            return coverType != null;
        }

        private void AddDeletionFailureTempData()
        {
            TempData["error"] =
                "Delete failed. Try again, and if the problem persists " +
                "see your system administrator.";
        }
    }
}
