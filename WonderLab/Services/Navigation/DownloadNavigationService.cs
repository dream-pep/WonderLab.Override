using System;
using Avalonia.Threading;
using System.Collections.Generic;
using WonderLab.Views.Pages.Download;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.Services.Navigation;

public sealed class DownloadNavigationService(Dispatcher dispatcher) : NavigationServiceBase(dispatcher) {
    public override Dictionary<string, Func<object>> FuncPages { get; } = new() {
        { nameof(SearchPage), App.ServiceProvider.GetRequiredService<SearchPage> },
        { nameof(MinecraftListPage), App.ServiceProvider.GetRequiredService<MinecraftListPage> },
    };
}