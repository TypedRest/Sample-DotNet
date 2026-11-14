using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace AddressBook;

/// <summary>
/// Provides access to contacts in an address book.
/// </summary>
[ApiController, Route("contacts"), Produces("application/json"), Consumes("application/json")]
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
    /// <response code="304">Not modified since the version specified by If-None-Match</response>
    /// <response code="404">Specified contact not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<Contact>> Read([FromRoute] string id)
        => WithETag(await service.ReadAsync(id));

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
    /// <response code="412">Contact was modified since the version specified by If-Match</response>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Set([FromRoute] string id, [FromBody] Contact contact)
    {
        if (contact.Id != id) throw new InvalidDataException($"ID in URI ({id}) must match the ID in the body ({contact.Id}).");

        await service.UpdateAsync(contact, GetExpectedVersions());

        return StatusCode((int)HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Deletes an existing contact.
    /// </summary>
    /// <param name="id">The ID of the contact to delete.</param>
    /// <response code="204">Success</response>
    /// <response code="404">Specified contact not found</response>
    /// <response code="412">Contact was modified since the version specified by If-Match</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    public async Task Delete([FromRoute] string id)
        => await service.DeleteAsync(id, GetExpectedVersions());

    /// <summary>
    /// Returns the note for a specific contact.
    /// </summary>
    /// <param name="id">The ID of the contact to get the note for.</param>
    /// <response code="200">OK</response>
    /// <response code="304">Not modified since the version specified by If-None-Match</response>
    /// <response code="404">Specified contact not found</response>
    [HttpGet("{id}/note")]
    public async Task<ActionResult<Note>> ReadNote([FromRoute] string id)
        => WithETag(await service.ReadNoteAsync(id));

    /// <summary>
    /// Sets a note for a specific contact.
    /// </summary>
    /// <param name="id">The ID of the contact to set the note for.</param>
    /// <param name="note">The note to set</param>
    /// <response code="200">OK</response>
    /// <response code="400">Missing or invalid request body</response>
    /// <response code="404">Specified contact not found</response>
    /// <response code="412">Contact was modified since the version specified by If-Match</response>
    [HttpPut("{id}/note")]
    public async Task<IActionResult> SetNote([FromRoute] string id, [FromBody] Note note)
    {
        await service.SetNoteAsync(id, note, GetExpectedVersions());

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

    /// <summary>
    /// Sets the ETag response header to the <paramref name="result"/>'s version.
    /// Responds with 304 Not Modified instead if the request's <c>If-None-Match</c> header already contains it.
    /// </summary>
    private ActionResult<T> WithETag<T>(Versioned<T> result)
    {
        var eTag = new EntityTagHeaderValue($"\"{result.Version}\"");
        Response.GetTypedHeaders().ETag = eTag;

        // Weak comparison, as required for If-None-Match by RFC 9110
        var ifNoneMatch = Request.GetTypedHeaders().IfNoneMatch;
        if (ifNoneMatch.Any(x => x.Equals(EntityTagHeaderValue.Any) || x.Compare(eTag, useStrongComparison: false)))
            return StatusCode((int)HttpStatusCode.NotModified);

        return result.Value;
    }

    /// <summary>
    /// Returns the versions listed in the request's <c>If-Match</c> header.
    /// </summary>
    private List<Guid>? GetExpectedVersions()
    {
        var ifMatch = Request.GetTypedHeaders().IfMatch;
        if (ifMatch.Count == 0 || ifMatch.Any(x => x.Equals(EntityTagHeaderValue.Any))) return null;

        // Strong comparison, as required for If-Match by RFC 9110, so weak ETags never match
        return
        [
            ..ifMatch.Where(x => !x.IsWeak)
                     .Select(x => Guid.TryParse(x.Tag.Value?.Trim('"'), out var version) ? version : (Guid?)null)
                     .OfType<Guid>()
        ];
    }
}
