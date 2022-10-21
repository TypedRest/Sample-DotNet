namespace AddressBook;

/// <summary>
/// Describes the service's database model.
/// </summary>
public class AddressBookDbContext : DbContext
{
    public AddressBookDbContext(DbContextOptions options)
        : base(options)
    {}

    public DbSet<ContactEntity> Contacts { get; set; } = default!;

    public DbSet<PokeEntity> Pokes { get; set; } = default!;
}
