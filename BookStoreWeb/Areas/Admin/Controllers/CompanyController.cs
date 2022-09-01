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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BookStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<CompanyController> _logger;

        private readonly IMapper _mapper;

        public CompanyController(IUnitOfWork unitOfWork, ILogger<CompanyController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            if (id is null or 0)
            {
                return View(new UpsertCompanyViewModel());
            }
            else
            {
                var company = await _unitOfWork.CompanyRepository.GetFirstOrDefaultAsync(p => p.Id == id);

                if (company == null)
                {
                    return NotFound();
                }
                var x = _mapper.Map<UpsertCompanyViewModel>(company);
                return View(x);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(UpsertCompanyViewModel upsertCompanyViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(upsertCompanyViewModel);
            }

            if (upsertCompanyViewModel.Id is 0)
            {
                try
                {

                    _unitOfWork.CompanyRepository.Add(_mapper.Map<Company>(upsertCompanyViewModel));
                    await _unitOfWork.SaveAsync();
                    TempData["success"] = "Company created successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occurred while creating the Company.");
                    ModelState.AddModelError("", "Unable to save changes. " +
                                                 "Try again, and if the problem persists, " +
                                                 "see your system administrator.");
                    return View(upsertCompanyViewModel);
                }
            }

            // edit
            try
            {
                 _unitOfWork.CompanyRepository.Update(_mapper.Map<Company>(upsertCompanyViewModel));
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Company updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while updating the Company.");
                ModelState.AddModelError("", "Unable to update changes. " +
                                             "Try again, and if the problem persists, " +
                                             "see your system administrator.");
                return View(upsertCompanyViewModel);
            }
        }

        //#region API CALLS

        public async Task<IActionResult> GetAll()
        {
            var companies = await _unitOfWork.CompanyRepository.GetAllAsync();
            return Json(new { data = companies });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var company = await _unitOfWork.CompanyRepository.GetFirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            try
            {
                _unitOfWork.CompanyRepository.Remove(company);
                await _unitOfWork.SaveAsync();
                return Json(new { success = true, message = "Company deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Category.");
                return Json(new { success = false, message = "Error while deleting" });
            }
        }
    }
}
