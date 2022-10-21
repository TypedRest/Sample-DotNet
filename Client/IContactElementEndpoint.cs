using TypedRest.Endpoints.Generic;
using TypedRest.Endpoints.Rpc;

namespace AddressBook;

public interface IContactElementEndpoint : IElementEndpoint<Contact>
{
    /// <summary>
    /// An optional note on the contact.
    /// </summary>
    IElementEndpoint<Note> Note { get; }

    /// <summary>
    /// A action for poking the contact.
    /// </summary>
    IActionEndpoint Poke { get; }
}
