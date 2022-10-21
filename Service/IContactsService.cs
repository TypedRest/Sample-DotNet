using System.Collections.Generic;
using System.Threading.Tasks;

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
    Task<Contact> ReadAsync(string id);

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
    /// <exception cref="KeyNotFoundException">Specified contact not found.</exception>
    Task UpdateAsync(Contact contact);

    /// <summary>
    /// Deletes an existing contact.
    /// </summary>
    /// <param name="id">The ID of the contact to delete.</param>
    /// <exception cref="KeyNotFoundException">Specified contact not found.</exception>
    Task DeleteAsync(string id);

    /// <summary>
    /// Returns the note for a specific contact.
    /// </summary>
    /// <param name="id">The ID of the contact to get the note for.</param>
    /// <exception cref="KeyNotFoundException">Specified contact not found.</exception>
    Task<Note> ReadNoteAsync(string id);

    /// <summary>
    /// Sets a note for a specific contact.
    /// </summary>
    /// <param name="id">The ID of the contact to set the note for.</param>
    /// <param name="note">The note to set</param>
    /// <exception cref="KeyNotFoundException">Specified contact not found.</exception>
    Task SetNoteAsync(string id, Note note);

    /// <summary>
    /// Pokes a contact.
    /// </summary>
    /// <param name="id">The ID of the contact to poke.</param>
    /// <exception cref="KeyNotFoundException">Specified contact not found.</exception>
    Task PokeAsync(string id);
}
