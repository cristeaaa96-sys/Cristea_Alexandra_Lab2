using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cristea_Alexandra_Lab2.Models;

namespace Cristea_Alexandra_Lab2.Pages.BorrowingPages;

public class EditModel : PageModel
{
    private readonly Cristea_Alexandra_Lab2Context _context;

    public EditModel(Cristea_Alexandra_Lab2Context context)
    {
        _context = context;
    }

    [BindProperty]
    public Borrowing Borrowing { get; set; } = default!;

    private async Task PopulateDropDownListsAsync()
    {
        var members = await _context.Member
            .Select(m => new
            {
                m.ID,
                FullName = (m.FirstName ?? "") + " " +
                           (m.LastName ?? "")
            })
            .ToListAsync();

        ViewData["MemberID"] = new SelectList(
            members,
            "ID",
            "FullName",
            Borrowing.MemberID);

        var books = await _context.Book
            .Include(b => b.Authors)
            .Select(b => new
            {
                b.ID,
                BookFullName = b.Title + " - " +
                    (b.Authors != null
                        ? b.Authors.FirstName + " " +
                          b.Authors.LastName
                        : "")
            })
            .ToListAsync();

        ViewData["BookID"] = new SelectList(
            books,
            "ID",
            "BookFullName",
            Borrowing.BookID);
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var borrowing = await _context.Borrowing
            .FirstOrDefaultAsync(m => m.ID == id);

        if (borrowing == null)
        {
            return NotFound();
        }

        Borrowing = borrowing;

        await PopulateDropDownListsAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropDownListsAsync();
            return Page();
        }

        _context.Attach(Borrowing).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BorrowingExists(Borrowing.ID))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool BorrowingExists(int id)
    {
        return _context.Borrowing.Any(e => e.ID == id);
    }
}