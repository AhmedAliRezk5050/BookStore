using BookStore.DataAccess;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BookStoreWeb.Pages.Categories;

public class DetailsModel : PageModel
{
    private readonly DataContext _context;

    public DetailsModel(DataContext context)
    {
        _context = context;
    }

    public Category? Category { get; set; }

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
}