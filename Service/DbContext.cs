using Microsoft.EntityFrameworkCore;

namespace AddressBook;

/// <summary>
/// Describes the service's database model.
/// </summary>
public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbContext(DbContextOptions options)
        : base(options)
    {}

    public DbSet<ContactEntity> Contacts { get; set; } = default!;

    public DbSet<PokeEntity> Pokes { get; set; } = default!;
}
