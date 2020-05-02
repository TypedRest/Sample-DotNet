using TypedRest.Endpoints;
using TypedRest.Endpoints.Generic;

namespace AddressBook
{
    /// <summary>
    /// Provides a type-safe client for the Address Book REST API.
    /// </summary>
    public interface IAddressBookClient : IEndpoint
    {
        /// <summary>
        /// Provides access to contacts in an address book.
        /// </summary>
        ICollectionEndpoint<Contact, ContactElementEndpoint> Contacts { get; }
    }
}
