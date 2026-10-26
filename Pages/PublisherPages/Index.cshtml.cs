using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cristea_Alexandra_Lab2.Models;
using Cristea_Alexandra_Lab2.ViewModels;

namespace Cristea_Alexandra_Lab2.Pages.PublisherPages;

public class IndexModel : PageModel
{
    private readonly Cristea_Alexandra_Lab2Context _context;

    public IndexModel(Cristea_Alexandra_Lab2Context context)
    {
        _context = context;
    }

    public IList<Publisher> Publisher { get; set; }
        = new List<Publisher>();

    public PublisherIndexData PublisherData { get; set; }
        = new PublisherIndexData();

    public int PublisherID { get; set; }

    public int BookID { get; set; }

    public async Task OnGetAsync(int? id, int? bookID)
    {
        PublisherData = new PublisherIndexData();

        PublisherData.Publishers = await _context.Publisher
            .Include(p => p.Books)
                .ThenInclude(b => b.Authors)
            .OrderBy(p => p.PublisherName)
            .ToListAsync();

        Publisher = PublisherData.Publishers.ToList();

        if (id != null)
        {
            PublisherID = id.Value;

            var selectedPublisher = PublisherData.Publishers
                .FirstOrDefault(p => p.ID == id.Value);

            if (selectedPublisher != null)
            {
                PublisherData.Books =
                    selectedPublisher.Books ?? new List<Book>();
            }
        }

        if (bookID != null)
        {
            BookID = bookID.Value;
        }
    }
}