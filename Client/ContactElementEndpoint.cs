using TypedRest;
using TypedRest.Endpoints;
using TypedRest.Endpoints.Generic;
using TypedRest.Endpoints.Rpc;

namespace AddressBook;

/// <summary>
/// Represents a REST endpoint for a single <see cref="Contact"/>.
/// </summary>
public class ContactElementEndpoint(IEndpoint referrer, Uri relativeUri)
    : ElementEndpoint<Contact>(referrer, relativeUri.EnsureTrailingSlash()), IContactElementEndpoint
{
    /// <summary>
    /// An optional note on the contact.
    /// </summary>
    public IElementEndpoint<Note> Note => new ElementEndpoint<Note>(this, relativeUri: "note");

    /// <summary>
    /// A action for poking the contact.
    /// </summary>
    public IActionEndpoint Poke => new ActionEndpoint(this, relativeUri: "poke");
}
