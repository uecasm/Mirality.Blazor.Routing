using Microsoft.AspNetCore.Components.Routing;

namespace Mirality.Blazor.Routing;

/// <summary>This is the interface you should <c>@inject</c> to access the lockable navigation system.</summary>
public interface ILockableNavigationManager : ICustomNavigationManager
{
    /// <summary>Reports whether or not navigation is currently locked.</summary>
    bool IsLocked { get; }

    /// <summary>Registers a Browser Exit Handler that hooks and reports when browser exit has been blocked.</summary>
    /// <remarks>There can only be one of these at a time.  Usually <see cref="BrowserExitLock"/> handles this.</remarks>
    void RegisterBrowserExitHandler(IBrowserExitHandler handler);

    /// <summary>Unregisters a Browser Exit Handler that hooks and reports when browser exit has been blocked.</summary>
    /// <remarks>There can only be one of these at a time.  Usually <see cref="BrowserExitLock"/> handles this.</remarks>
    void UnregisterBrowserExitHandler(IBrowserExitHandler handler);

    /// <summary>Call this to lock navigation attempts.  Dispose the return value to unlock.</summary>
    /// <returns>A disposable that will unlock navigation (if no other locks still exist).</returns>
    IDisposable LockNavigation();

    /// <summary>Raised when <see cref="IsLocked"/> changes.</summary>
    event EventHandler IsLockedChanged;

    /// <summary>Raised when navigation is blocked due to an active lock.</summary>
    event EventHandler<LocationChangedEventArgs> NavigationBlocked;

    /// <summary>Raised when the user attempted to close the browser tab.</summary>
    /// <remarks>This usually only happens *after* the user has already seen the default browser UI and agreed to keep the page open.</remarks>
    event EventHandler BrowserExitBlocked;
}

/// <summary>Extension class for <see cref="ILockableNavigationManager"/>.</summary>
public static class LockableNavigationExtensions
{
    /// <summary>Sets the lock state from a boolean, via the specified lock field.</summary>
    /// <param name="locker">The navigation locker</param>
    /// <param name="lock">A field in your page/component (initially null) that represents the lock state</param>
    /// <param name="shouldLock">True to lock; false to unlock</param>
    public static void SetLockState(this ILockableNavigationManager locker, ref IDisposable? @lock, bool shouldLock)
    {
        if (@lock == null && shouldLock)
        {
            @lock = locker.LockNavigation();
        }
        else if (@lock != null && !shouldLock)
        {
            @lock.Dispose();
            @lock = null;
        }
    }
}
