using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Mirality.Blazor.Routing.Internals;

internal class NullLockableNavigationManager : ILockableNavigationManager
{
    public NullLockableNavigationManager(ICustomNavigationManager parent)
    {
        _Parent = parent;
    }

    private readonly ICustomNavigationManager _Parent;

    public bool IsLocked => false;

    public void RegisterBrowserExitHandler(IBrowserExitHandler handler)
    {
    }

    public void UnregisterBrowserExitHandler(IBrowserExitHandler handler)
    {
    }

    private class Unlocker : IDisposable
    {
        public void Dispose()
        {
        }
    }

    public IDisposable LockNavigation()
    {
        return new Unlocker();
    }

#pragma warning disable 67
    public event EventHandler? IsLockedChanged;
    public event EventHandler<LocationChangedEventArgs>? NavigationBlocked;
    public event EventHandler? BrowserExitBlocked;
#pragma warning restore 67

    public event EventHandler<LocationChangedEventArgs> LocationChanged
    {
        add => _Parent.LocationChanged += value;
        remove => _Parent.LocationChanged -= value;
    }

    public string BaseUri => _Parent.BaseUri;

    public string Uri => _Parent.Uri;

    public void NavigateTo(string uri, bool forceLoad = false, bool replace = false)
    {
        _Parent.NavigateTo(uri, forceLoad, replace);
    }

    public void NavigateTo(string uri, NavigationOptions options)
    {
        _Parent.NavigateTo(uri, options);
    }

    public Uri ToAbsoluteUri(string relativeUri)
    {
        return _Parent.ToAbsoluteUri(relativeUri);
    }

    public string ToBaseRelativePath(string uri)
    {
        return _Parent.ToBaseRelativePath(uri);
    }
}