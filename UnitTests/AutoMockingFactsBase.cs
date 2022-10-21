using Microsoft.Extensions.DependencyInjection;
using Moq.AutoMock;

namespace AddressBook;

/// <summary>
/// Instantiates a test <typeparamref name="TSubject"/>, automatically injecting mocks for its dependencies.
/// </summary>
public abstract class AutoMockingFactsBase<TSubject> : AutoMocker, IDisposable
    where TSubject : class
{
    private readonly Lazy<TSubject> _subject;

    /// <summary>
    /// The system under test.
    /// </summary>
    protected TSubject Subject => _subject.Value;

    protected AutoMockingFactsBase()
    {
        GetMock<IServiceProvider>().Setup(x => x.GetService(It.IsAny<Type>())).Returns<Type>(Get);
        GetMock<IServiceScope>().SetupGet(x => x.ServiceProvider).Returns(Get<IServiceProvider>);
        GetMock<IServiceScopeFactory>().Setup(x => x.CreateScope()).Returns(Get<IServiceScope>);

        _subject = new Lazy<TSubject>(CreateInstance<TSubject>);
    }

    /// <summary>
    /// Verifies all created mocks.
    /// </summary>
    public virtual void Dispose() => Verify();
}
