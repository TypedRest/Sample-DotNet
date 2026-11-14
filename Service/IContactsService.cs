namespace AddressBook;

/// <summary>
/// Manages contacts in an address book.
/// </summary>
public interface IContactsService
{
    /// <summary>
    /// Returns all contacts.
    /// </summary>
    Task<IEnumerable<Contact>> ReadAllAsync();

    /// <summary>
    /// Returns a specific contact.
    /// </summary>
    /// <param name="id">The ID of the contact to look for.</param>
    /// <exception cref="KeyNotFoundException">Specified contact not found.</exception>
    Task<Versioned<Contact>> ReadAsync(string id);

    /// <summary>
    /// Creates a new contact.
    /// </summary>
    /// <param name="contact">The contact to create (without an ID).</param>
    /// <returns>The contact that was created (with the ID).</returns>
    Task<Contact> CreateAsync(Contact contact);

    /// <summary>
    /// Updates an existing contact.
    /// </summary>
    /// <param name="contact">The modified contact.</param>
    /// <param name="expectedVersions">The versions the contact may have for the update to proceed; <c>null</c> for any.</param>
    /// <exception cref="KeyNotFoundException">Specified contact not found.</exception>
    /// <exception cref="DbUpdateConcurrencyException">The contact does not have one of the <paramref name="expectedVersions"/>.</exception>
    Task UpdateAsync(Contact contact, IReadOnlyCollection<Guid>? expectedVersions);

    /// <summary>
    /// Deletes an existing contact.
    /// </summary>
    /// <param name="id">The ID of the contact to delete.</param>
    /// <param name="expectedVersions">The versions the contact may have for the deletion to proceed; <c>null</c> for any.</param>
    /// <exception cref="KeyNotFoundException">Specified contact not found.</exception>
    /// <exception cref="DbUpdateConcurrencyException">The contact does not have one of the <paramref name="expectedVersions"/>.</exception>
    Task DeleteAsync(string id, IReadOnlyCollection<Guid>? expectedVersions);

    /// <summary>
    /// Returns the note for a specific contact.
    /// </summary>
    /// <param name="id">The ID of the contact to get the note for.</param>
    /// <returns>The note along with the version of the contact.</returns>
    /// <exception cref="KeyNotFoundException">Specified contact not found.</exception>
    Task<Versioned<Note>> ReadNoteAsync(string id);

    /// <summary>
    /// Sets a note for a specific contact.
    /// </summary>
    /// <param name="id">The ID of the contact to set the note for.</param>
    /// <param name="note">The note to set</param>
    /// <param name="expectedVersions">The versions the contact may have for the change to proceed; <c>null</c> for any.</param>
    /// <exception cref="KeyNotFoundException">Specified contact not found.</exception>
    /// <exception cref="DbUpdateConcurrencyException">The contact does not have one of the <paramref name="expectedVersions"/>.</exception>
    Task SetNoteAsync(string id, Note note, IReadOnlyCollection<Guid>? expectedVersions);

    /// <summary>
    /// Pokes a contact.
    /// </summary>
    /// <param name="id">The ID of the contact to poke.</param>
    /// <exception cref="KeyNotFoundException">Specified contact not found.</exception>
    Task PokeAsync(string id);
}
