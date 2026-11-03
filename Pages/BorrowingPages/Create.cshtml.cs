using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cristea_Alexandra_Lab2.Models;

namespace Cristea_Alexandra_Lab2.Pages.BorrowingPages;

public class CreateModel : PageModel
{
    private readonly Cristea_Alexandra_Lab2Context _context;

    public CreateModel(Cristea_Alexandra_Lab2Context context)
    {
        _context = context;
    }
    public IActionResult OnGet()
    {
        var bookList = _context.Book
            .Include(b => b.Authors)
            .Select(x => new
            {
                x.ID,
                BookFullName = x.Title + " - " +
                    (x.Authors != null
                        ? x.Authors.LastName + " " + x.Authors.FirstName
                        : "")
            })
            .ToList();

        ViewData["BookID"] = new SelectList(
            bookList, "ID", "BookFullName");

        var memberList = _context.Member
            .Select(m => new
            {
                m.ID,
                FullName = (m.FirstName ?? "") + " " +
                           (m.LastName ?? "")
            })
            .ToList();

        ViewData["MemberID"] = new SelectList(
            memberList, "ID", "FullName");

        return Page();
    }


    [BindProperty]
    public Borrowing Borrowing { get; set; } = default!;

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Borrowing.Add(Borrowing);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
