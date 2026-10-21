using Microsoft.EntityFrameworkCore;

public class Cristea_Alexandra_Lab2Context(
    DbContextOptions<Cristea_Alexandra_Lab2Context> options
) : DbContext(options)
{
    public DbSet<Cristea_Alexandra_Lab2.Models.Book> Book { get; set; } = default!;

    public DbSet<Cristea_Alexandra_Lab2.Models.Publisher> Publisher { get; set; } = default!;

    public DbSet<Cristea_Alexandra_Lab2.Models.Authors> Authors { get; set; } = default!;

    public DbSet<Cristea_Alexandra_Lab2.Models.Category> Category { get; set; } = default!;

    public DbSet<Cristea_Alexandra_Lab2.Models.BookCategory> BookCategory { get; set; } = default!;
}