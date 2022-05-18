using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Mirality.Blazor.Routing.Internals;

internal class LockableNavigationManager : NavigationManager, ILockableNavigationManager, IDisposable
{
    public LockableNavigationManager(NavigationManager parent)
    {
        _Parent = parent;
        _Parent.LocationChanged += Parent_LocationChanged;
    }

    public void Dispose()
    {
        _Parent.LocationChanged -= Parent_LocationChanged;
    }

    private readonly NavigationManager _Parent;
    private IBrowserExitHandler? _BrowserExitHandler;
    private bool _IsBlindNavigation;
    private int _NavigationLockCount;

    private class Unlocker : IDisposable
    {
        public Unlocker(Action action)
        {
            _Action = action;
        }

        public void Dispose()
        {
            _Action?.Invoke();
            _Action = null;
        }

        private Action? _Action;
    }

    public bool IsLocked => _NavigationLockCount > 0;

    public void RegisterBrowserExitHandler(IBrowserExitHandler handler)
    {
        if (_BrowserExitHandler != null) throw new InvalidOperationException($"Cannot register new IBrowserExitHandler when already have {_BrowserExitHandler.GetType()}");
        _BrowserExitHandler = handler;
        _BrowserExitHandler.BrowserExitBlocked += BrowserExit_Blocked;
    }

    public void UnregisterBrowserExitHandler(IBrowserExitHandler handler)
    {
        if (_BrowserExitHandler == null) return;
        if (_BrowserExitHandler != handler) throw new InvalidOperationException($"Cannot unregister IBrowserExitHandler {handler.GetType()} when {_BrowserExitHandler.GetType()} is registered");
        _BrowserExitHandler.BrowserExitBlocked -= BrowserExit_Blocked;
        _BrowserExitHandler = null;
    }

    public IDisposable LockNavigation()
    {
        if (Interlocked.Increment(ref _NavigationLockCount) == 1)
        {
            IsLockedChanged?.Invoke(this, EventArgs.Empty);
        }
        return new Unlocker(UnlockNavigation);
    }

    private void UnlockNavigation()
    {
        if (Interlocked.Decrement(ref _NavigationLockCount) == 0)
        {
            IsLockedChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? IsLockedChanged;
    public event EventHandler<LocationChangedEventArgs>? NavigationBlocked;
    public event EventHandler? BrowserExitBlocked;

    private void BrowserExit_Blocked(object? sender, EventArgs e)
    {
        BrowserExitBlocked?.Invoke(this, e);
    }

    private void Parent_LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (_IsBlindNavigation)
        {
            _IsBlindNavigation = false;
            return;
        }

        if (e.IsNavigationIntercepted && IsLocked)
        {
            _IsBlindNavigation = true;
            _Parent.NavigateTo(Uri, false, true);

            NavigationBlocked?.Invoke(this, e);
            return;
        }

        _IsBlindNavigation = false;
        Uri = e.Location;
        NotifyLocationChanged(e.IsNavigationIntercepted);
    }

    protected override void EnsureInitialized()
    {
        Initialize(_Parent.BaseUri, _Parent.Uri);

        base.EnsureInitialized();
    }

    protected override void NavigateToCore(string uri, NavigationOptions options)
    {
        if (IsLocked)
        {
            // navigation is locked; notify but otherwise do nothing
            NavigationBlocked?.Invoke(this, new LocationChangedEventArgs(uri, false));
            return;
        }

        // navigation is unlocked; pass on to parent manager
        _Parent.NavigateTo(uri, options);
    }
}