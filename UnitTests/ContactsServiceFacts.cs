using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace AddressBook;

/// <summary>
/// Ensures <see cref="ContactsService"/> works correctly with a database.
/// </summary>
public class ContactsServiceFacts : DatabaseFactsBase<ContactsService>
{
    [Fact]
    public async Task ReadsAllFromDatabase()
    {
        var entry1 = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "John", LastName = "Smith"});
        var entry2 = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "Jane", LastName = "Doe"});
        await Context.SaveChangesAsync();

        var result = await Subject.ReadAllAsync();
        result.Should().BeEquivalentTo(new[]
        {
            new Contact {Id = entry1.Entity.Id, FirstName = "John", LastName = "Smith"},
            new Contact {Id = entry2.Entity.Id, FirstName = "Jane", LastName = "Doe"}
        });
    }

    [Fact]
    public async Task ReadsFromDatabase()
    {
        var entry = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "John", LastName = "Smith"});
        await Context.SaveChangesAsync();

        var result = await Subject.ReadAsync(entry.Entity.Id);
        result.Should().Be(new Contact {Id = entry.Entity.Id, FirstName = "John", LastName = "Smith"});
    }

    [Fact]
    public async Task CreatesInDatabase()
    {
        var result = await Subject.CreateAsync(new Contact {FirstName = "John", LastName = "Smith"});

        Context.Contacts.Single().Should().BeEquivalentTo(new ContactEntity {Id = result.Id, FirstName = "John", LastName = "Smith"});
    }

    [Fact]
    public async Task UpdatesInDatabase()
    {
        var entry = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "John", LastName = "Smith"});
        await Context.SaveChangesAsync();

        await Subject.UpdateAsync(new Contact {Id = entry.Entity.Id, FirstName = "Jane", LastName = "Doe"});

        var entity = await Context.Contacts.FindAsync(entry.Entity.Id);
        entity.FirstName.Should().Be("Jane");
        entity.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task DeletesFromDatabase()
    {
        var entry = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "John", LastName = "Smith"});
        await Context.SaveChangesAsync();

        await Subject.DeleteAsync(entry.Entity.Id);

        Context.Contacts.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadsNoteFromDatabase()
    {
        var entry = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "John", LastName = "Smith", Note = "my note"});
        await Context.SaveChangesAsync();

        var note = await Subject.ReadNoteAsync(entry.Entity.Id);
        note.Should().Be(new Note {Content = "my note"});
    }

    [Fact]
    public async Task WritesNoteInDatabase()
    {
        var entry = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "John", LastName = "Smith"});
        await Context.SaveChangesAsync();

        await Subject.SetNoteAsync(entry.Entity.Id, new Note {Content = "my note"});

        var contact = await Context.Contacts.FindAsync(entry.Entity.Id);
        contact.Note.Should().Be("my note");
    }

    [Fact]
    public async Task StoresPokeInDatabase()
    {
        var entry = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "John", LastName = "Smith"});
        await Context.SaveChangesAsync();

        await Subject.PokeAsync(entry.Entity.Id);

        Context.Pokes.Single().ContactId.Should().Be(entry.Entity.Id);
    }
}
