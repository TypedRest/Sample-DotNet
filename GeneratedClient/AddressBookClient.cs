using TypedRest.Serializers;

namespace AddressBook.Generated;

// The rest of this class is generated from the OpenAPI document by TypedRest.SourceGenerator.
public partial class AddressBookClient
{
    /// <summary>
    /// Creates a new Address Book Client using a custom <see cref="HttpClient"/>. This is usually used for testing.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to communicate with the remote element.</param>
    /// <param name="uri">The base URI of the Address Book API.</param>
    public AddressBookClient(HttpClient httpClient, Uri uri)
        : base(httpClient, uri, serializer: new SystemTextJsonSerializer())
    {}
}
