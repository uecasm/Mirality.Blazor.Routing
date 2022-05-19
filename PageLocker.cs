using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Mirality.Blazor.Routing
{
    public class PageLocker : ComponentBase, IDisposable
    {
        [Inject] private ILockableNavigationManager Nav { get; set; } = default!;

        [Parameter] public bool IsLocked { get; set; }

        [Parameter] public EventCallback<LocationChangedEventArgs> NavigationBlocked { get; set; }

        private IDisposable? _Lock;

        protected override void OnInitialized()
        {
            Nav.NavigationBlocked += Nav_NavigationBlocked;

            base.OnInitialized();
        }

        public void Dispose()
        {
            Nav.NavigationBlocked -= Nav_NavigationBlocked;

            _Lock?.Dispose();
        }

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
}
