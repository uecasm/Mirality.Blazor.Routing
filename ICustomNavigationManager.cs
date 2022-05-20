using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Mirality.Blazor.Routing;

/// <summary>This is the interface of <see cref="NavigationManager"/>.  Implement it to hook into <see cref="CustomRouter"/>.</summary>
public interface ICustomNavigationManager
{
    /// <inheritdoc cref="NavigationManager.LocationChanged"/>
    event EventHandler<LocationChangedEventArgs> LocationChanged;

    /// <inheritdoc cref="NavigationManager.BaseUri"/>
    string BaseUri { get; }

    /// <inheritdoc cref="NavigationManager.Uri"/>
    string Uri { get; }

    /// <inheritdoc cref="NavigationManager.NavigateTo(string, bool, bool)"/>
    void NavigateTo(string uri, bool forceLoad = false, bool replace = false);

    /// <inheritdoc cref="NavigationManager.NavigateTo(string, NavigationOptions)"/>
    void NavigateTo(string uri, NavigationOptions options);

    /// <inheritdoc cref="NavigationManager.ToAbsoluteUri(string)"/>
    Uri ToAbsoluteUri(string relativeUri);

    /// <inheritdoc cref="NavigationManager.ToBaseRelativePath(string)"/>
    string ToBaseRelativePath(string uri);
}
