using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Mirality.Blazor.Routing.Internals;

internal class DefaultNavigationManager : ICustomNavigationManager
{
    private readonly NavigationManager _Parent;

    public DefaultNavigationManager(NavigationManager parent)
    {
        _Parent = parent;
    }

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

    public string BaseUri => _Parent.BaseUri;

    public string Uri => _Parent.Uri;

    public event EventHandler<LocationChangedEventArgs> LocationChanged
    {
        add => _Parent.LocationChanged += value;
        remove => _Parent.LocationChanged -= value;
    }
}