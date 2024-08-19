using System;
using WonderLab.Views.Pages;
using System.Collections.Generic;
using WonderLab.Views.Pages.Navigation;
using Avalonia.Threading;
using WonderLab.ViewModels.Pages;
using Avalonia.Controls;

namespace WonderLab.Services.Navigation;

/// <summary>
/// 主体页面导航服务
/// </summary>
public sealed class HostNavigationService(Dispatcher dispatcher) : NavigationServiceBase(dispatcher) {
    public override Dictionary<string, Func<object>> FuncPages { get; } = new() {
        { nameof(MultiplayerPage), App.GetService<MultiplayerPage> },
        { nameof(SettingNavigationPage), App.GetService<SettingNavigationPage> },
        { nameof(DownloadNavigationPage), App.GetService<DownloadNavigationPage> },
    };

    public Control NavigationToHome() {
        var page = App.GetService<HomePage>();
        page.DataContext = App.GetService<HomePageViewModel>();
        return page;
    }
}