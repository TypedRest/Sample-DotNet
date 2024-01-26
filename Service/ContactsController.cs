using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AddressBook;

/// <summary>
/// Provides access to contacts in an address book.
/// </summary>
[ApiController, Route("contacts")]
public class ContactsController(IContactsService service) : Controller
{
    /// <summary>
    /// Returns all contacts.
    /// </summary>
    /// <response code="200">OK</response>
    [HttpGet("")]
    public async Task<IEnumerable<Contact>> ReadAll()
        => await service.ReadAllAsync();

    /// <summary>
    /// Returns a specific contact.
    /// </summary>
    /// <param name="id">The ID of the contact to look for.</param>
    /// <response code="200">OK</response>
    /// <response code="404">Specified contact not found</response>
    [HttpGet("{id}")]
    public async Task<Contact> Read([FromRoute] string id)
        => await service.ReadAsync(id);

    /// <summary>
    /// Creates a new contact.
    /// </summary>
    /// <param name="contact">The contact to create (without an ID).</param>
    /// <returns>The contact that was created (with the ID).</returns>
    /// <response code="201">Created</response>
    /// <response code="400">Missing or invalid request body</response>
    [HttpPost("")]
    [ProducesResponseType(201)]
    public async Task<ActionResult<Contact>> Create([FromBody] Contact contact)
    {
        var result = await service.CreateAsync(contact);

        return CreatedAtAction(
            actionName: nameof(Read),
            routeValues: new {id = result.Id},
            result);
    }

    /// <summary>
    /// Updates an existing contact.
    /// </summary>
    /// <param name="id">The ID of the contact to update (must match the ID in <paramref name="contact"/>).</param>
    /// <param name="contact">The modified contact.</param>
    /// <response code="204">Success</response>
    /// <response code="400">Missing or invalid request body</response>
    /// <response code="404">Specified contact not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Set([FromRoute] string id, [FromBody] Contact contact)
    {
        if (contact.Id != id) throw new InvalidDataException($"ID in URI ({id}) must match the ID in the body ({contact.Id}).");

        await service.UpdateAsync(contact);

        return StatusCode((int)HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Deletes an existing contact.
    /// </summary>
    /// <param name="id">The ID of the contact to delete.</param>
    /// <response code="204">Success</response>
    /// <response code="404">Specified contact not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    public async Task Delete([FromRoute] string id)
        => await service.DeleteAsync(id);

    /// <summary>
    /// Returns the note for a specific contact.
    /// </summary>
    /// <param name="id">The ID of the contact to get the note for.</param>
    /// <response code="200">OK</response>
    /// <response code="404">Specified contact not found</response>
    [HttpGet("{id}/note")]
    public async Task<Note> ReadNote([FromRoute] string id)
        => await service.ReadNoteAsync(id);

    /// <summary>
    /// Sets a note for a specific contact.
    /// </summary>
    /// <param name="id">The ID of the contact to set the note for.</param>
    /// <param name="note">The note to set</param>
    /// <response code="200">OK</response>
    /// <response code="400">Missing or invalid request body</response>
    /// <response code="404">Specified contact not found</response>
    [HttpPut("{id}/note")]
    public async Task<IActionResult> SetNote([FromRoute] string id, [FromBody] Note note)
    {
        await service.SetNoteAsync(id, note);

        return StatusCode((int)HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Pokes a contact.
    /// </summary>
    /// <param name="id">The ID of the contact to poke.</param>
    /// <response code="204">Success</response>
    /// <response code="404">Specified contact not found</response>
    [HttpPost("{id}/poke")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Poke([FromRoute] string id)
    {
        await service.PokeAsync(id);

        return NoContent();
    }
}
