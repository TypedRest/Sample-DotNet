namespace AddressBook;

/// <summary>
/// Describes the service's database model.
/// </summary>
public class AddressBookDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<ContactEntity> Contacts { get; set; } = default!;

    public DbSet<PokeEntity> Pokes { get; set; } = default!;

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        BumpVersions();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        BumpVersions();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>
    /// Assigns a new <see cref="ContactEntity.Version"/> to every modified contact.
    /// </summary>
    private void BumpVersions()
    {
        foreach (var entry in ChangeTracker.Entries<ContactEntity>().Where(x => x.State == EntityState.Modified))
            entry.Entity.Version = Guid.NewGuid();
    }
}
