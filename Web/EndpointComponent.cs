using Microsoft.AspNetCore.Components;
using TypedRest.Endpoints;

namespace AddressBook.Web;

/// <summary>
/// Base class for components that work with a TypedRest endpoint.
/// </summary>
/// <typeparam name="TEndpoint">The type of endpoint the component works with.</typeparam>
public abstract class EndpointComponent<TEndpoint> : ComponentBase, IDisposable
    where TEndpoint : class, IEndpoint
{
    /// <summary>
    /// The endpoint this component works with.
    /// </summary>
    /// <remarks>
    /// The component keeps the endpoint it was first rendered with for its whole lifetime,
    /// even when its parent re-renders and passes a new endpoint object for the same resource.
    /// This matters because TypedRest stores ETags on the endpoint object:
    /// reading again sends <c>If-None-Match</c>, and the first save after a read sends <c>If-Match</c> so the server can reject lost updates.
    /// </remarks>
    [Parameter, EditorRequired]
    public TEndpoint Endpoint { get; set; } = null!;

    public override Task SetParametersAsync(ParameterView parameters)
    {
        var endpoint = Endpoint;
        parameters.SetParameterProperties(this);
        if (endpoint != null) Endpoint = endpoint;

        return base.SetParametersAsync(ParameterView.Empty);
    }

    [CascadingParameter]
    private StatusProvider Status { get; set; } = null!;

    /// <summary>
    /// Runs an API call and reports the outcome in the status line of the enclosing <see cref="StatusProvider"/>.
    /// </summary>
    /// <param name="action">The API call to run.</param>
    /// <param name="successMessage">Shown when the call succeeds. Leave unset for background reads that should not replace the current status.</param>
    protected Task RunAsync(Func<Task> action, string? successMessage = null)
        => Status.RunAsync(action, successMessage, CancellationToken);

    private readonly CancellationTokenSource _cancellation = new();

    /// <summary>
    /// Cancelled when the component is removed.
    /// Pass it to TypedRest calls to cancel requests that are still running when the component goes away.
    /// </summary>
    protected CancellationToken CancellationToken => _cancellation.Token;

    // Not disposing the CancellationTokenSource, since it has no timer and CancellationToken may still be read by pending continuations
    public void Dispose() => _cancellation.Cancel();
}
