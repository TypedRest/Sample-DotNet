namespace AddressBook;

/// <summary>
/// Manages contacts in an address book.
/// </summary>
public class ContactsService(AddressBookDbContext context, ILogger<ContactsService> logger) : IContactsService
{
    public async Task<IEnumerable<Contact>> ReadAllAsync()
    {
        var result = await ToDtos(context.Contacts).ToListAsync();

        logger.LogTrace("Read all contacts");
        return result;
    }

    public async Task<Versioned<Contact>> ReadAsync(string id)
    {
        var element = await context.Contacts.Where(x => x.Id == id)
                                     .Select(x => new Versioned<Contact>(new Contact {Id = x.Id, FirstName = x.FirstName, LastName = x.LastName}, x.Version))
                                     .SingleOrDefaultAsync()
                      ?? throw new KeyNotFoundException($"Contact '{id}' not found.");

        logger.LogTrace("Read contact {Id}", id);
        return element;
    }

    private static IQueryable<Contact> ToDtos(IQueryable<ContactEntity> entities)
        => entities.Select(x => new Contact {Id = x.Id, FirstName = x.FirstName, LastName = x.LastName});

    public async Task<Contact> CreateAsync(Contact element)
    {
        var entity = new ContactEntity
        {
            FirstName = element.FirstName,
            LastName = element.LastName
        };

        await context.Contacts.AddAsync(entity);
        await context.SaveChangesAsync();

        logger.LogDebug("Created new contact {Id}", entity.Id);
        return new Contact {Id = entity.Id, FirstName = element.FirstName, LastName = element.LastName};
    }

    public async Task UpdateAsync(Contact element, IReadOnlyCollection<Guid>? expectedVersions)
    {
        var entity = await FindAsync(element.Id!, expectedVersions);

        entity.FirstName = element.FirstName;
        entity.LastName = element.LastName;

        await context.SaveChangesAsync();

        logger.LogDebug("Updated contact {Id}", element.Id);
    }

    public async Task DeleteAsync(string id, IReadOnlyCollection<Guid>? expectedVersions)
    {
        var entity = await FindAsync(id, expectedVersions);

        context.Contacts.Remove(entity);
        await context.SaveChangesAsync();

        logger.LogDebug("Deleted contact {Id}", id);
    }

    public async Task<Versioned<Note>> ReadNoteAsync(string id)
    {
        var note = await context.Contacts.Where(x => x.Id == id)
                                  .Select(x => new Versioned<Note>(new Note {Content = x.Note}, x.Version))
                                  .SingleOrDefaultAsync()
                   ?? throw new KeyNotFoundException($"Contact '{id}' not found.");

        logger.LogTrace("Read note for contact {Id}", id);
        return note;
    }

    public async Task SetNoteAsync(string id, Note note, IReadOnlyCollection<Guid>? expectedVersions)
    {
        var entity = await FindAsync(id, expectedVersions);

        entity.Note = note.Content;

        await context.SaveChangesAsync();

        logger.LogDebug("Set note for contact {Id}", id);
    }

    public async Task PokeAsync(string id)
    {
        var entity = await FindAsync(id);

        entity.Pokes.Add(new PokeEntity {Timestamp = DateTime.UtcNow});
        await context.SaveChangesAsync();

        logger.LogDebug("Poked contact {Id}", id);
    }

    /// <summary>
    /// Loads a contact for modification.
    /// </summary>
    /// <param name="id">The ID of the contact.</param>
    /// <param name="expectedVersions">The versions the contact may have; <c>null</c> for any.</param>
    /// <exception cref="KeyNotFoundException">Specified contact not found.</exception>
    /// <exception cref="DbUpdateConcurrencyException">The contact does not have one of the <paramref name="expectedVersions"/>.</exception>
    private async Task<ContactEntity> FindAsync(string id, IReadOnlyCollection<Guid>? expectedVersions = null)
    {
        var entity = await context.Contacts.FindAsync(id)
                     ?? throw new KeyNotFoundException($"Contact '{id}' not found.");

        if (expectedVersions != null && !expectedVersions.Contains(entity.Version))
            throw new DbUpdateConcurrencyException($"Contact '{id}' was modified since it was last read. Reload it and try again.");

        return entity;
    }
}
