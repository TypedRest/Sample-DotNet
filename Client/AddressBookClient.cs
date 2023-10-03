using TypedRest.Endpoints;
using TypedRest.Endpoints.Generic;
using TypedRest.Serializers;

namespace AddressBook;

/// <summary>
/// Provides a type-safe client for the Address Book REST API.
/// </summary>
public class AddressBookClient : EntryEndpoint, IAddressBookClient
{
    /// <summary>
    /// Creates a new Address Book Client.
    /// </summary>
    /// <param name="uri">The base URI of the Address Book API.</param>
    public AddressBookClient(Uri uri)
        : base(uri, serializer: new SystemTextJsonSerializer())
    {}

    /// <summary>
    /// Creates a new Address Book Client using a custom <see cref="HttpClient"/>. This is usually used for testing.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to communicate with the remote element.</param>
    /// <param name="uri">The base URI of the Address Book API.</param>
    public AddressBookClient(HttpClient httpClient, Uri uri)
        : base(httpClient, uri, serializer: new SystemTextJsonSerializer())
    {}

    /// <summary>
    /// A collection of contacts in an address book.
    /// </summary>
    public ICollectionEndpoint<Contact, ContactElementEndpoint> Contacts
        => new CollectionEndpoint<Contact, ContactElementEndpoint>(this, relativeUri: "contacts");
}
