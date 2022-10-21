using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AddressBook;

/// <summary>
/// Instantiates a test <typeparamref name="TSubject"/>, injecting an in-memory database and mocks for its other dependencies.
/// </summary>
public class DatabaseFactsBase<TSubject> : AutoMockingFactsBase<TSubject>
    where TSubject : class
{
    private readonly SqliteConnection _connection;

    /// <summary>
    /// An in-memory database that is reset after every test.
    /// </summary>
    protected readonly DbContext Context;

    protected DatabaseFactsBase()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        Context = new DbContext(
            new DbContextOptionsBuilder().UseSqlite(_connection).EnableSensitiveDataLogging().Options);
        Context.Database.EnsureCreated();

        Use(Context);
    }

    public override void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();

        base.Dispose();
    }
}
