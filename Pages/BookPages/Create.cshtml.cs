using Cristea_Alexandra_Lab2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cristea_Alexandra_Lab2.Pages.BookPages;

public class CreateModel : BookCategoriesPageModel
{
    private readonly Cristea_Alexandra_Lab2Context _context;

    public CreateModel(Cristea_Alexandra_Lab2Context context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        ViewData["PublisherID"] = new SelectList(
        _context.Set<Publisher>(), "ID", "PublisherName");
        ViewData["AuthorsID"] = new SelectList(
        _context.Set<Authors>()
        .Select(a => new
        {
            a.ID,
            FullName = a.FirstName + " " + a.LastName
        }),
        "ID", "FullName");

        var book = new Book
        {
            BookCategories = new List<BookCategory>()
        };

        PopulateAssignedCategoryData(_context, book);

        return Page();

    }

    [BindProperty]
    public Book Book { get; set; } = default!;

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync(string[] selectedCategories)
    {

        if (!ModelState.IsValid)
        {
            ViewData["PublisherID"] = new SelectList(
                _context.Publisher,
                "ID",
                "PublisherName",
                Book.PublisherID);

            var authorsList = _context.Authors
                .Select(a => new
                {
                    a.ID,
                    FullName = a.FirstName + " " + a.LastName
                }).ToList();

            ViewData["AuthorsID"] = new SelectList(
                authorsList,
                "ID",
                "FullName",
                Book.AuthorsID);

            PopulateAssignedCategoryData(_context, Book);

            var selectedIds = new HashSet<string>(selectedCategories);

            foreach (var cat in AssignedCategoryDataList)
            {
                cat.Assigned = selectedIds.Contains(
                    cat.CategoryID.ToString());
            }

            return Page();
        }

        Book.BookCategories = new List<BookCategory>();

        foreach (var cat in selectedCategories)
        {
            if (int.TryParse(cat, out int categoryId))
            {
                Book.BookCategories.Add(new BookCategory
                {
                    CategoryID = categoryId
                });
            }
        }


        _context.Book.Add(Book);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
