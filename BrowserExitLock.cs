using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Mirality.Blazor.Routing;

/// <summary>Interface for a browser exit handler</summary>
public interface IBrowserExitHandler
{
    /// <summary>Raised when an attempted browser exit has been blocked</summary>
    event EventHandler? BrowserExitBlocked;
}

/// <summary>A component that will conditionally block (as much as the browser will allow) closing or exiting
/// while the navigation state is locked.</summary>
/// <remarks>Recommended to include in your App or MainLayout if you want this functionality.</remarks>
public class BrowserExitLock : ComponentBase, IAsyncDisposable, IBrowserExitHandler
{
    [Inject] private IJSRuntime Js { get; set; } = default!;
    [Inject] private ILockableNavigationManager LockableNav { get; set; } = default!;

    private readonly Lazy<Task<IJSObjectReference>> _ModuleTask;
    private DotNetObjectReference<BrowserExitLock> _Self = default!;
    private bool _IsLocked;

    /// <summary>Constructor</summary>
    public BrowserExitLock()
    {
        _ModuleTask = new(() => Js.InvokeAsync<IJSObjectReference>("import", "./_content/Mirality.Blazor.Routing/browserexit.js").AsTask());
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        LockableNav.RegisterBrowserExitHandler(this);
        LockableNav.IsLockedChanged += OnLockedChanged;

        _Self = DotNetObjectReference.Create(this);

        if (LockableNav.IsLocked)
        {
            await Lock();
        }

        await base.OnInitializedAsync();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        LockableNav.UnregisterBrowserExitHandler(this);
        LockableNav.IsLockedChanged -= OnLockedChanged;

        if (_IsLocked)
        {
            await Unlock();
        }

        _Self.Dispose();

        if (_ModuleTask.IsValueCreated)
        {
            var module = await _ModuleTask.Value;
            await module.DisposeAsync();
        }
    }

    /// <inheritdoc />
    public event EventHandler? BrowserExitBlocked;

    private void OnLockedChanged(object? sender, EventArgs e)
    {
        if (LockableNav.IsLocked && !_IsLocked)
        {
            InvokeAsync(Lock);
        }
        else if (!LockableNav.IsLocked && _IsLocked)
        {
            InvokeAsync(Unlock);
        }
    }

    private async Task Lock()
    {
        _IsLocked = true;
        var module = await _ModuleTask.Value;
        await module.InvokeVoidAsync("lock", _Self);
    }

    private async Task Unlock()
    {
        _IsLocked = false;
        var module = await _ModuleTask.Value;
        await module.InvokeVoidAsync("unlock");
    }

    /// <summary>Intended for internal use only</summary>
    [JSInvokable]
    public Task NotifyExitAttempt()
    {
        BrowserExitBlocked?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }
}
