﻿using Microsoft.Extensions.DependencyInjection;
using Mirality.Blazor.Routing.Internals;

namespace Mirality.Blazor.Routing;

/// <summary>Extension class for dependency injection registration.</summary>
public static class NavigationManagerExtensions
{
    /// <summary>Adds services for the default (non-lockable) navigation manager.</summary>
    /// <remarks>Locking interfaces are still registered, but do nothing.</remarks>
    /// <param name="services">The service collection to register with.</param>
    public static void AddDefaultNavigationManager(this IServiceCollection services)
    {
        if (OperatingSystem.IsBrowser())
        {
            services.AddSingleton<ICustomNavigationManager, DefaultNavigationManager>();
            services.AddSingleton<ILockableNavigationManager, NullLockableNavigationManager>();
        }
        else
        {
            services.AddScoped<ICustomNavigationManager, DefaultNavigationManager>();
            services.AddScoped<ILockableNavigationManager, NullLockableNavigationManager>();
        }
    }

    /// <summary>Adds services for the lockable navigation manager.</summary>
    /// <param name="services">The service collection to register with.</param>
    public static void AddLockableNavigationManager(this IServiceCollection services)
    {
        if (OperatingSystem.IsBrowser())
        {
            services.AddSingleton<LockableNavigationManager>();
            services.AddSingleton<ICustomNavigationManager>(sp => sp.GetRequiredService<LockableNavigationManager>());
            services.AddSingleton<ILockableNavigationManager>(sp => sp.GetRequiredService<LockableNavigationManager>());
        }
        else
        {
            services.AddScoped<LockableNavigationManager>();
            services.AddScoped<ICustomNavigationManager>(sp => sp.GetRequiredService<LockableNavigationManager>());
            services.AddScoped<ILockableNavigationManager>(sp => sp.GetRequiredService<LockableNavigationManager>());
        }
    }
}