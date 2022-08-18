﻿using BookStoreWeb.Data;
using BookStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _context;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(DataContext context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync();

            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name", "DisplayOrder")] Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(category);
                    await _context.SaveChangesAsync();
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
    }
}