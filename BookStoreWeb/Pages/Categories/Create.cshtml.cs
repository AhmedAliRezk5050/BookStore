using BookStoreWeb.Data;
using BookStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookStoreWeb.Pages.Categories;

public class CreateModel : PageModel
{
    private readonly DataContext _context;

    public CreateModel(DataContext context)
    {
        _context = context;
    }

    [BindProperty] public Category Category { get; set; } = null!;
    
    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var emptyCategory = new Category();

        if (await TryUpdateModelAsync<Category>(
                emptyCategory,
                "category",
                c => c.Name,
                c => c.DisplayOrder!,
                c => c.CreatedDateTime
            )
           )
        {
            _context.Categories.Add(emptyCategory);

            await _context.SaveChangesAsync();
            
            TempData["success"] = "Category created successfully";
            
            return RedirectToPage("./Index");
        }

        return new EmptyResult();
    }
}