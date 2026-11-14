using Microsoft.EntityFrameworkCore;

namespace AddressBook;

/// <summary>
/// Ensures <see cref="ContactsService"/> works correctly with a database.
/// </summary>
public class ContactsServiceFacts : DatabaseFactsBase<ContactsService>
{
    [Fact]
    public async Task ReadsAllFromDatabase()
    {
        var entry1 = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "John", LastName = "Smith"}, TestContext.Current.CancellationToken);
        var entry2 = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "Jane", LastName = "Doe"}, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

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
        var entry = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "John", LastName = "Smith"}, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = (await Subject.ReadAsync(entry.Entity.Id)).Value;
        result.Should().Be(new Contact {Id = entry.Entity.Id, FirstName = "John", LastName = "Smith"});
    }

    [Fact]
    public async Task CreatesInDatabase()
    {
        var result = await Subject.CreateAsync(new Contact {FirstName = "John", LastName = "Smith"});

        Context.Contacts.Single().Should().BeEquivalentTo(new ContactEntity {Id = result.Id, FirstName = "John", LastName = "Smith"}, opts => opts.Excluding(x => x.Version));
    }

    [Fact]
    public async Task UpdatesInDatabase()
    {
        var entry = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "John", LastName = "Smith"}, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        await Subject.UpdateAsync(new Contact {Id = entry.Entity.Id, FirstName = "Jane", LastName = "Doe"}, null);

        var entity = await Context.Contacts.FindAsync([entry.Entity.Id], TestContext.Current.CancellationToken);
        entity!.FirstName.Should().Be("Jane");
        entity.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task ChangesVersionOnUpdate()
    {
        var entry = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "John", LastName = "Smith"}, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var original = await Subject.ReadAsync(entry.Entity.Id);

        await Subject.UpdateAsync(new Contact {Id = entry.Entity.Id, FirstName = "Jane", LastName = "Doe"}, [original.Version]);

        var updated = await Subject.ReadAsync(entry.Entity.Id);
        updated.Version.Should().NotBe(original.Version);
    }

    [Fact]
    public async Task RejectsUpdateOnVersionMismatch()
    {
        var entry = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "John", LastName = "Smith"}, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        await Subject.Awaiting(x => x.UpdateAsync(new Contact {Id = entry.Entity.Id, FirstName = "Jane", LastName = "Doe"}, [Guid.NewGuid()]))
            .Should().ThrowAsync<DbUpdateConcurrencyException>();
    }

    [Fact]
    public async Task DeletesFromDatabase()
    {
        var entry = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "John", LastName = "Smith"}, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        await Subject.DeleteAsync(entry.Entity.Id, null);

        Context.Contacts.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadsNoteFromDatabase()
    {
        var entry = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "John", LastName = "Smith", Note = "my note"}, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var note = (await Subject.ReadNoteAsync(entry.Entity.Id)).Value;
        note.Should().Be(new Note {Content = "my note"});
    }

    [Fact]
    public async Task WritesNoteInDatabase()
    {
        var entry = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "John", LastName = "Smith"}, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        await Subject.SetNoteAsync(entry.Entity.Id, new Note {Content = "my note"}, null);

        var contact = await Context.Contacts.FindAsync([entry.Entity.Id], TestContext.Current.CancellationToken);
        contact!.Note.Should().Be("my note");
    }

    [Fact]
    public async Task StoresPokeInDatabase()
    {
        var entry = await Context.Contacts.AddAsync(new ContactEntity {FirstName = "John", LastName = "Smith"}, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        await Subject.PokeAsync(entry.Entity.Id);

        Context.Pokes.Single().ContactId.Should().Be(entry.Entity.Id);
    }
}
