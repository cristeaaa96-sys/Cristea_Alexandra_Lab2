using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cristea_Alexandra_Lab2.Models;

namespace Cristea_Alexandra_Lab2.Pages.BookPages;

public class IndexModel : PageModel
{
    private readonly Cristea_Alexandra_Lab2Context _context;

    public IndexModel(Cristea_Alexandra_Lab2Context context)
    {
        _context = context;
    }

    public BookData BookD { get; set; } = new BookData();

    public int BookID { get; set; }

    public int CategoryID { get; set; }

    public string TitleSort { get; set; } = string.Empty;

    public string AuthorSort { get; set; } = string.Empty;

    public string CurrentFilter { get; set; } = string.Empty;

    public async Task OnGetAsync(
        int? id,
        int? categoryID,
        string? sortOrder,
        string? searchString)
    {
        BookD = new BookData();

        TitleSort = string.IsNullOrEmpty(sortOrder)
            ? "title_desc"
            : "";

        AuthorSort = sortOrder == "author"
            ? "author_desc"
            : "author";

        CurrentFilter = searchString ?? string.Empty;

        BookD.Books = await _context.Book
            .Include(b => b.Publisher)
            .Include(b => b.Authors)
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .AsNoTracking()
            .OrderBy(b => b.Title)
            .ToListAsync();

        // Cautam cartile dupa titlu sau numele autorului
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            BookD.Books = BookD.Books
                .Where(b =>
                    (b.Title != null &&
                     b.Title.Contains(
                         searchString,
                         StringComparison.OrdinalIgnoreCase))
                    ||
                    (b.Authors != null &&
                     ((b.Authors.FirstName ?? "").Contains(
                         searchString,
                         StringComparison.OrdinalIgnoreCase)
                      ||
                      (b.Authors.LastName ?? "").Contains(
                         searchString,
                         StringComparison.OrdinalIgnoreCase))))
                .ToList();
        }

        // Selectarea unei carti
        if (id != null)
        {
            BookID = id.Value;

            Book? book = BookD.Books
                .FirstOrDefault(b => b.ID == id.Value);

            if (book != null)
            {
                BookD.Categories = book.BookCategories?
                    .Select(bc => bc.Category)
                    .Where(c => c != null)
                    .Select(c => c!)
                    .ToList() ?? new List<Category>();
            }
        }

        if (categoryID != null)
        {
            CategoryID = categoryID.Value;
        }

        // Sortarea cartilor
        switch (sortOrder)
        {
            case "title_desc":
                BookD.Books = BookD.Books
                    .OrderByDescending(b => b.Title);
                break;

            case "author_desc":
                BookD.Books = BookD.Books
                    .OrderByDescending(b => b.Authors?.FullName);
                break;

            case "author":
                BookD.Books = BookD.Books
                    .OrderBy(b => b.Authors?.FullName);
                break;

            default:
                BookD.Books = BookD.Books
                    .OrderBy(b => b.Title);
                break;
        }
    }
}