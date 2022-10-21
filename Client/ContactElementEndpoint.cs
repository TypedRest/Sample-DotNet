using System;
using TypedRest;
using TypedRest.Endpoints;
using TypedRest.Endpoints.Generic;
using TypedRest.Endpoints.Rpc;

namespace AddressBook;

/// <summary>
/// Represents a REST endpoint for a single <see cref="Contact"/>.
/// </summary>
public class ContactElementEndpoint : ElementEndpoint<Contact>, IContactElementEndpoint
{
    public ContactElementEndpoint(IEndpoint referrer, Uri relativeUri)
        : base(referrer, relativeUri.EnsureTrailingSlash())
    {}

    /// <summary>
    /// An optional note on the contact.
    /// </summary>
    public IElementEndpoint<Note> Note => new ElementEndpoint<Note>(this, relativeUri: "note");

    /// <summary>
    /// A action for poking the contact.
    /// </summary>
    public IActionEndpoint Poke => new ActionEndpoint(this, relativeUri: "poke");
}
