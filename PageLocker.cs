using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Mirality.Blazor.Routing;

/// <summary>Component which conditionally locks navigation on a page.</summary>
public class PageLocker : ComponentBase, IDisposable
{
    [Inject] private ILockableNavigationManager Nav { get; set; } = default!;

    /// <summary>When true, navigation is locked.</summary>
    [Parameter] public bool IsLocked { get; set; }

    /// <summary>Raised when navigation is attempted while locked.</summary>
    [Parameter] public EventCallback<LocationChangedEventArgs> NavigationBlocked { get; set; }

    private IDisposable? _Lock;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Nav.NavigationBlocked += Nav_NavigationBlocked;

        base.OnInitialized();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Nav.NavigationBlocked -= Nav_NavigationBlocked;

        _Lock?.Dispose();
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (_Lock == null && IsLocked)
        {
            _Lock = Nav.LockNavigation();
        }
        else if (_Lock != null && !IsLocked)
        {
            _Lock.Dispose();
            _Lock = null;
        }

        base.OnParametersSet();
    }

    private void Nav_NavigationBlocked(object? sender, LocationChangedEventArgs e)
    {
        _ = NavigationBlocked.InvokeAsync(e);
    }
}