namespace AddressBook;

/// <summary>
/// A representation of a contact for database storage.
/// </summary>
public class ContactEntity
{
    /// <summary>
    /// The ID of the contact.
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The first name of the contact.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// The last name of the contact.
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// A note about a contact.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Changes whenever the contact (including its <see cref="Note"/>) is modified. Used for optimistic concurrency and caching.
    /// </summary>
    /// <remarks>Assigned automatically by <see cref="AddressBookDbContext"/> on every save that modifies the contact.</remarks>
    [ConcurrencyCheck]
    public Guid Version { get; set; } = Guid.NewGuid();

    /// <summary>
    /// A list of pokes performed on this contact.
    /// </summary>
    public ICollection<PokeEntity> Pokes { get; set; } = new List<PokeEntity>();
}
