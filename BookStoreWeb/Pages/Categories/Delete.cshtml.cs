using BookStoreWeb.Data;
using BookStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookStoreWeb.Pages.Categories;

public class DeleteModel : PageModel
{
    private readonly DataContext _context;
    public Category? Category { get; set; }

    private readonly ILogger<DeleteModel> _logger;
    
    public DeleteModel(DataContext context, ILogger<DeleteModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> OnGet(int? id, bool? saveChangesError = false)
    {
        if (id is null or 0)
        {
            return NotFound();
        }

        Category = await _context.Categories.FindAsync(id);

        if (Category is null)
        {
            return NotFound();
        }
        
        if (saveChangesError.GetValueOrDefault())
        {
            TempData["error"] = "Delete failed. Try again";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null or 0)
        {
            return NotFound();
        }

        var category = await _context.Categories.FindAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        try
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            
            TempData["success"] = "Category deleted successfully";
            
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting the Category.");
            
            return RedirectToPage("Index", new { id = category.Id, saveChangesError = true });
        }
    }
}