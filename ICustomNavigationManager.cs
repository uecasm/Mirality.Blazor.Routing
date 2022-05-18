using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Mirality.Blazor.Routing;

/// <summary>This is the interface of <see cref="NavigationManager"/>.  Implement it to hook into <see cref="CustomRouter"/>.</summary>
public interface ICustomNavigationManager
{
    event EventHandler<LocationChangedEventArgs> LocationChanged;

    string BaseUri { get; }
    string Uri { get; }

    void NavigateTo(string uri, bool forceLoad = false, bool replace = false);

    void NavigateTo(string uri, NavigationOptions options);

    Uri ToAbsoluteUri(string relativeUri);
    string ToBaseRelativePath(string uri);
}
