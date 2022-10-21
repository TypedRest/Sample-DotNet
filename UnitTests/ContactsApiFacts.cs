using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MorseCode.ITask;
using TypedRest.Endpoints.Generic;
using Xunit;
using Xunit.Abstractions;

namespace AddressBook;

/// <summary>
/// Ensures <see cref="IAddressBookClient.Contacts"/> and <see cref="ContactsController"/> work together.
/// </summary>
public class ContactsApiFacts : ApiFactsBase
{
    public ContactsApiFacts(ITestOutputHelper output)
        : base(output)
    {}

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

        var result = await Client.Contacts.ReadAllAsync();

        result.Should().Equal(contacts);
    }

    [Fact]
    public async Task ReadsFromService()
    {
        var contact = new Contact {Id = "1", FirstName = "John", LastName = "Smith"};
        _serviceMock.Setup(x => x.ReadAsync("1")).ReturnsAsync(contact);

        var result = await Client.Contacts["1"].ReadAsync();

        result.Should().Be(contact);
    }

    [Fact]
    public async Task CreatesInService()
    {
        var contactWithoutId = new Contact {FirstName = "John", LastName = "Smith"};
        var contactWithId = new Contact {Id = "1", FirstName = "John", LastName = "Smith"};
        _serviceMock.Setup(x => x.CreateAsync(contactWithoutId)).ReturnsAsync(contactWithId);

        var result = await Client.Contacts.CreateAsync(contactWithoutId);

        result.Should().NotBeNull();
        result!.Uri.Should().Be("http://localhost/contacts/1/");
    }

    [Fact]
    public async Task RejectsCreateOnIncompleteBody()
    {
        await Client.Contacts.Awaiting(x => x.CreateAsync(new Contact()).AsTask())
            .Should().ThrowAsync<InvalidDataException>();
    }

    [Fact]
    public async Task UpdatesInService()
    {
        var contact = new Contact {Id = "1", FirstName = "John", LastName = "Smith"};

        await Client.Contacts.SetAsync(contact);

        _serviceMock.Verify(x => x.UpdateAsync(contact));
    }

    [Fact]
    public async Task RejectsUpdateOnIdMismatch()
    {
        var contactDto = new Contact {Id = "1", FirstName = "John", LastName = "Smith"};

        await Client.Contacts["2"].Awaiting(x => x.SetAsync(contactDto))
            .Should().ThrowAsync<InvalidDataException>();
    }

    [Fact]
    public async Task DeletesFromService()
    {
        await Client.Contacts["1"].DeleteAsync();

        _serviceMock.Verify(x => x.DeleteAsync("1"));
    }

    [Fact]
    public async Task ReadsNoteFromService()
    {
        var note = new Note {Content = "my note"};
        _serviceMock.Setup(x => x.ReadNoteAsync("1")).ReturnsAsync(note);

        var result = await Client.Contacts["1"].Note.ReadAsync();

        result.Should().Be(note);
    }

    [Fact]
    public async Task SetsNoteInService()
    {
        var note = new Note {Content = "my note"};

        await Client.Contacts["1"].Note.SetAsync(note);

        _serviceMock.Verify(x => x.SetNoteAsync("1", note));
    }

    [Fact]
    public async Task PokesViaService()
    {
        await Client.Contacts["1"].Poke.InvokeAsync();

        _serviceMock.Verify(x => x.PokeAsync("1"));
    }
}
