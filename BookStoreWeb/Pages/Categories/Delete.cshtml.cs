using BookStoreWeb.Data;
using BookStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookStoreWeb.Pages.Categories;

public class DeleteModel : PageModel
{
    private readonly DataContext _context;
    public Category? Category { get; set; }

    public DeleteModel(DataContext context)
    {
        _context = context;
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
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            return RedirectToPage("Index", new { id = category.Id, saveChangesError = true });
        }
    }
}