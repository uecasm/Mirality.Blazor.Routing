using Microsoft.AspNetCore.Components;

namespace Mirality.Blazor.Routing
{
    public class PageLocker : ComponentBase, IDisposable
    {
        [Inject] private ILockableNavigationManager LockableNav { get; set; } = default!;

        [Parameter] public bool IsLocked { get; set; }

        private IDisposable? _Lock;

        public void Dispose()
        {
            _Lock?.Dispose();
        }

        protected override void OnParametersSet()
        {
            if (_Lock == null && IsLocked)
            {
                _Lock = LockableNav.LockNavigation();
            }
            else if (_Lock != null && !IsLocked)
            {
                _Lock.Dispose();
                _Lock = null;
            }

            base.OnParametersSet();
        }
    }
}
