using Microsoft.Extensions.DependencyInjection;
using MorseCode.ITask;
using TypedRest.Endpoints.Generic;

namespace AddressBook;

/// <summary>
/// Ensures the client generated from the OpenAPI document (<see cref="Generated.IAddressBookClient.Contacts"/>) and <see cref="ContactsController"/> work together.
/// </summary>
/// <remarks>
/// The generated client has its own DTOs (<see cref="Generated.Contact"/>, <see cref="Generated.Note"/>), separate from the ones used by the Service.
/// </remarks>
public class GeneratedContactsApiFacts(ITestOutputHelper output) : ApiFactsBase(output)
{
    private readonly Mock<IContactsService> _serviceMock = new();

    protected override void ConfigureService(IServiceCollection services)
        => services.AddMock(_serviceMock);

    [Fact]
    public async Task ReadsAllFromService()
    {
        var contacts = new List<Contact>
        {
            new() {Id = "1", FirstName = "John", LastName = "Smith"},
            new() {Id = "2", FirstName = "Jane", LastName = "Doe"}
        };
        _serviceMock.Setup(x => x.ReadAllAsync()).ReturnsAsync(contacts);

        var result = await GeneratedClient.Contacts.ReadAllAsync(cancellationToken: TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(contacts, opts => opts.ComparingByMembers<Contact>().WithStrictOrdering());
    }

    [Fact]
    public async Task ReadsFromService()
    {
        var contact = new Contact {Id = "1", FirstName = "John", LastName = "Smith"};
        _serviceMock.Setup(x => x.ReadAsync("1")).ReturnsAsync(contact);

        var result = await GeneratedClient.Contacts["1"].ReadAsync(TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(contact, opts => opts.ComparingByMembers<Contact>());
    }

    [Fact]
    public async Task CreatesInService()
    {
        var contactWithoutId = new Contact {FirstName = "John", LastName = "Smith"};
        var contactWithId = new Contact {Id = "1", FirstName = "John", LastName = "Smith"};
        _serviceMock.Setup(x => x.CreateAsync(contactWithoutId)).ReturnsAsync(contactWithId);

        var result = await GeneratedClient.Contacts.CreateAsync(
            new Generated.Contact {FirstName = "John", LastName = "Smith"},
            TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.Uri.Should().Be("http://localhost/contacts/1");
    }

    [Fact]
    public async Task RejectsCreateOnIncompleteBody()
    {
        await GeneratedClient.Contacts.Awaiting(x => x.CreateAsync(new Generated.Contact()).AsTask())
            .Should().ThrowAsync<InvalidDataException>();
    }

    [Fact]
    public async Task UpdatesInService()
    {
        await GeneratedClient.Contacts.SetAsync(
            new Generated.Contact {Id = "1", FirstName = "John", LastName = "Smith"},
            TestContext.Current.CancellationToken);

        _serviceMock.Verify(x => x.UpdateAsync(new Contact {Id = "1", FirstName = "John", LastName = "Smith"}));
    }

    [Fact]
    public async Task RejectsUpdateOnIdMismatch()
    {
        var contact = new Generated.Contact {Id = "1", FirstName = "John", LastName = "Smith"};

        await GeneratedClient.Contacts["2"].Awaiting(x => x.SetAsync(contact))
            .Should().ThrowAsync<InvalidDataException>();
    }

    [Fact]
    public async Task DeletesFromService()
    {
        await GeneratedClient.Contacts["1"].DeleteAsync(TestContext.Current.CancellationToken);

        _serviceMock.Verify(x => x.DeleteAsync("1"));
    }

    [Fact]
    public async Task ReadsNoteFromService()
    {
        var note = new Note {Content = "my note"};
        _serviceMock.Setup(x => x.ReadNoteAsync("1")).ReturnsAsync(note);

        var result = await GeneratedClient.Contacts["1"].Note.ReadAsync(TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(note, opts => opts.ComparingByMembers<Note>());
    }

    [Fact]
    public async Task SetsNoteInService()
    {
        await GeneratedClient.Contacts["1"].Note.SetAsync(
            new Generated.Note {Content = "my note"},
            TestContext.Current.CancellationToken);

        _serviceMock.Verify(x => x.SetNoteAsync("1", new Note {Content = "my note"}));
    }

    [Fact]
    public async Task PokesViaService()
    {
        await GeneratedClient.Contacts["1"].Poke.InvokeAsync(TestContext.Current.CancellationToken);

        _serviceMock.Verify(x => x.PokeAsync("1"));
    }
}
