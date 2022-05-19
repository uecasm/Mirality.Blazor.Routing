This is a custom Blazor router implementation that allows navigation to be cancelled (for example, to prevent data loss if a form has unsaved changes).

It is inspired by [Blazr.Demo.Routing](https://github.com/ShaunCurtis/Blazr.Demo.Routing) and uses a similar technique to hook the routing and actually cancel navigation; though it does not copy any of the actual code directly, and it uses a completely different front-end that I happen to prefer over that version.

It embeds (almost entirely verbatim) a large chunk of ASP.NET 6 Router code, since these were implemented as `internal` and there isn't currently any other way to hook it.

# Basic Setup

1. Download this project and unpack it to a suitable location in your source tree (or reference it as a git submodule).

2. Add it to your solution if needed, and add it as a project reference to your application.

3. Optionally, add `@using Mirality.Blazor.Routing` to your `_Imports.razor` file (otherwise you'll need to add this to any file that uses components from this library).

4. In your `Program.cs`, add this near the top of your services (and add the corresponding `using` to the top of the file):

   ```c#
   builder.Services.AddLockableNavigationManager();
   ```

5. In your `App.razor`, swap `Router` to `CustomRouter` (adding the `@using` if you didn't already).

6. Optional, but recommended: swap anywhere you're injecting `NavigationManager` with `ICustomNavigationManager`.  (See below for more details.)

# Controlling Navigation Blocking

### The Simple Way

Simply add the following to any page where you want to be able to block navigation:

```c#
<PageLocker IsLocked="@_IsLocked" NavigationBlocked="OnNavigationBlocked" />
```

In your code-behind, declare:

```c#
private bool _IsLocked;

private async Task OnNavigationBlocked(LocationChangedEventArgs e)
{
    //...
}
```

(changing names if you prefer.)  Then simply set this field to `true` whenever you want to block navigation and to false when you want to enable it again.  You'll receive a callback if navigation is attempted while locked.

The actual lock/unlock is deferred until the end of the current event (whenever it next synchronises parameters), so this is not suitable if you want to unlock navigation and then immediately `NavigateTo`, for example.

You can also use any other valid syntax for passing parameters to a child component, as you'd expect; for example, you can use some existing property of your model or call a method instead of introducing a new field.

### The Advanced Way

If you need more control over locking than the above provides (for example, if you want to unlock-and-navigate as mentioned above), then **instead** of doing the above, do this:

```c#
@inject ILockableNavigationManager Nav
```

or

```c#
[Inject] private ILockableNavigationManager Nav { get; set; } = default!;
```

and then also declare

```c#
private IDisposable? _Lock;
```

and then finally whenever you want to change the state, use:

```c#
Nav.SetLockState(ref _Lock, true/false);
```

Note that when using this method, you must remember to implement `IDisposable` and/or `IAsyncDisposable` and either dispose the lock or set it to false when your component is disposed.  (While ordinarily you would not expect your component to be disposed when navigation is locked, it's not entirely impossible.  And it's just good code hygiene.)

# Blocking closing tabs

By default, nothing stops closing the browser tab, even if navigation is locked.  If you want to intercept that as well, then include this component somewhere:

```c#
<BrowserExitLock />
```

Usually the best place to put this is in your `MainLayout.razor`, but you could put it in your `App` or in your individual page or subcomponent if you like.  But there can only be one such component at a time -- it's ok to have one in each page, but it's not ok to have one in the layout and one in the page, for example.

Also note that browsers are free to ignore this if they feel like it, and in particular will usually prompt the user to confirm exiting or not before this gets any say in the matter -- so you will typically only receive the corresponding event after the user has already agreed to remain on the page; thus there's usually no need to actually react to it.

(If you're migrating from `Blazr.Demo.Routing`, note that this is the approximate equivalent of the `PageLocker` component from that project -- the `PageLocker` here serves a different role.)

# NavigationManager

The regular `NavigationManager` methods and properties are also available from both `ICustomNavigationManager` and `ILockableNavigationManager`, so you can (mostly) use them interchangeably, including calling `NavigateTo` on any of them.

One caveat is the `LocationChanged` event -- due to how things work internally, if you listen directly to `NavigationManager.LocationChanged` then you will receive some spurious "navigated away and then back" notifications whenever navigation is cancelled, because the default manager does not (yet) support actual cancellation.  It's strongly recommended to subscribe to the event on either of the interfaces instead, which will only receive the non-cancelled navigations.

# Additional events

[`ILockableNavigationManager`](ILockableNavigationManager.cs) implements some additional properties and events that may be of interest.

The main one is the event `NavigationBlocked`, which is raised when a navigation is attempted while locked; you can use this to display some kind of warning or confirmation.

Another is the event `BrowserExitBlocked`, which is raised when the browser tab is closed while locked *and the user has already confirmed that they want to stay on the page*.  As noted above, this is usually less interesting in most cases, but is available in case it is needed.

To subscribe to these, just inject the interface as shown above.  Don't forget to unsubscribe on Dispose too.

# Advanced usage

If you want to use the custom router but don't want the lockable navigation for some reason, you can call `AddDefaultNavigationManager` instead of `AddLockableNavigationManager` in your `Program.cs`.  Both interfaces will still exist in this case, but locking is disabled and does nothing.

You can alternatively implement your own version of any of these interfaces or components and they should be able to talk to each other as you'd expect.

You can even make your own custom navigation manager and delegate to the `ILockableNavigationManager` interface to combine your own logic with lockability.

