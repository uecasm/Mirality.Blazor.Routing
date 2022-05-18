using Microsoft.Extensions.DependencyInjection;
using Mirality.Blazor.Routing.Internals;

namespace Mirality.Blazor.Routing;

public static class NavigationManagerExtensions
{
    public static void AddDefaultNavigationManager(this IServiceCollection services)
    {
        services.AddSingleton<ICustomNavigationManager, DefaultNavigationManager>();
        services.AddSingleton<ILockableNavigationManager, NullLockableNavigationManager>();
    }

    public static void AddLockableNavigationManager(this IServiceCollection services)
    {
        services.AddSingleton<LockableNavigationManager>();
        services.AddSingleton<ICustomNavigationManager>(sp => sp.GetRequiredService<LockableNavigationManager>());
        services.AddSingleton<ILockableNavigationManager>(sp => sp.GetRequiredService<LockableNavigationManager>());
    }
}