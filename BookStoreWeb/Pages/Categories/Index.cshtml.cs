using BookStoreWeb.Data;
using BookStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BookStoreWeb.Pages.Categories;

public class IndexModel : PageModel
{
    private readonly DataContext _context;
    
    public IEnumerable<Category> Categories { get; set; } = Enumerable.Empty<Category>();

    public IndexModel(DataContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> OnGetAsync()
    {
        Categories = await _context.Categories.ToListAsync();
        
        return Page();
    }
}