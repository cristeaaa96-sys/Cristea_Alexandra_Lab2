using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cristea_Alexandra_Lab2.Models;

namespace Cristea_Alexandra_Lab2.Pages.BookPages
{
    public class EditModel : BookCategoriesPageModel
    {
        private readonly Cristea_Alexandra_Lab2Context _context;

        public EditModel(Cristea_Alexandra_Lab2Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Publisher)
                .Include(b => b.Authors)
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.ID == id);

            if (book == null)
            {
                return NotFound();
            }

            Book = book;

            PopulateAssignedCategoryData(_context, Book);

            PopulateDropDownLists();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(
            int? id,
            string[] selectedCategories)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookToUpdate = await _context.Book
                .Include(b => b.Publisher)
                .Include(b => b.Authors)
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .FirstOrDefaultAsync(b => b.ID == id);

            if (bookToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Book>(
                bookToUpdate,
                "Book",
                b => b.Title,
                b => b.AuthorsID,
                b => b.Price,
                b => b.PublishingDate,
                b => b.PublisherID))
            {
                UpdateBookCategories(
                    _context,
                    selectedCategories,
                    bookToUpdate);

                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }

            UpdateBookCategories(
                _context,
                selectedCategories,
                bookToUpdate);

            PopulateAssignedCategoryData(
                _context,
                bookToUpdate);

            Book = bookToUpdate;

            PopulateDropDownLists();

            return Page();
        }

        private void PopulateDropDownLists()
        {
            ViewData["PublisherID"] = new SelectList(
                _context.Publisher,
                "ID",
                "PublisherName",
                Book?.PublisherID);

            var authorsList = _context.Authors
                .Select(a => new
                {
                    a.ID,
                    FullName = a.FirstName + " " + a.LastName
                })
                .ToList();

            ViewData["AuthorsID"] = new SelectList(
                authorsList,
                "ID",
                "FullName",
                Book?.AuthorsID);
        }
    }
}
