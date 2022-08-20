using BookStore.DataAccess;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookStoreWeb.Pages.Categories;

public class EditModel : PageModel
{
    private readonly DataContext _context;

    [BindProperty] public Category? Category { get; set; }

    public EditModel(DataContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
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

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var categoryToUpdate = await _context.Categories.FindAsync(id);

        if (await TryUpdateModelAsync<Category>(
                categoryToUpdate!,
                "category",
                c => c.Name,
                c => c.DisplayOrder!,
                c => c.CreatedDateTime!
            )
           )
        {
            await _context.SaveChangesAsync();

            TempData["success"] = "Category edited successfully";
                
            return RedirectToPage("./Index");
        }

        return new EmptyResult();
    }
}