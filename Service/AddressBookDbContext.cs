namespace AddressBook;

/// <summary>
/// Describes the service's database model.
/// </summary>
public class AddressBookDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<ContactEntity> Contacts { get; set; } = default!;

    public DbSet<PokeEntity> Pokes { get; set; } = default!;
}
