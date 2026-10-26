using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cristea_Alexandra_Lab2.Models;

namespace Cristea_Alexandra_Lab2.Pages.CategoryPages;

public class IndexModel : PageModel
{
    private readonly Cristea_Alexandra_Lab2Context _context;

    public IndexModel(Cristea_Alexandra_Lab2Context context)
    {
        _context = context;
    }

    public IList<Category> Category { get; set; }
        = new List<Category>();

    public int? SelectedCategoryID { get; set; }

    public IList<Book> SelectedBooks { get; set; }
        = new List<Book>();

    public async Task OnGetAsync(int? id)
    {
        // Incarcam toate categoriile
        Category = await _context.Category
            .OrderBy(c => c.CategoryName)
            .ToListAsync();

        // Verificam daca a fost selectata o categorie
        if (id != null)
        {
            SelectedCategoryID = id.Value;

            // Incarcam cartile categoriei selectate
            SelectedBooks = await _context.Book
                .Include(b => b.Authors)
                .Include(b => b.BookCategories)
                .Where(b => b.BookCategories != null &&
                    b.BookCategories.Any(bc =>
                        bc.CategoryID == id.Value))
                .OrderBy(b => b.Title)
                .ToListAsync();
        }
    }
}
