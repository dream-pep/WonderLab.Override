using CommunityToolkit.Mvvm.Input;
using WonderLab.Classes.Interfaces;
using WonderLab.Services.Navigation;
using WonderLab.ViewModels.Pages.Setting;
using CommunityToolkit.Mvvm.ComponentModel;
using WonderLab.Classes.Datas;
using Avalonia.Threading;

namespace WonderLab.ViewModels.Pages.Navigation;

public sealed partial class SettingNavigationPageViewModel : ViewModelBase {
    public readonly SettingNavigationService _navigationService;

    [ObservableProperty] private object _activeItem;
    [ObservableProperty] private NavigationPageData _activePage;

    public SettingNavigationPageViewModel(SettingNavigationService navigationService, Dispatcher dispatcher) {
        _navigationService = navigationService;
        _navigationService.NavigationTo<LaunchSettingPageViewModel>();
    }

    [RelayCommand]
    private void NavigationTo(string pageKey) {
        switch (pageKey) {
            case "AboutPage":
                _navigationService.NavigationTo<AboutPageViewModel>();
                break;
            case "LaunchSettingPage":
                _navigationService.NavigationTo<LaunchSettingPageViewModel>(); 
                break;
            case "DetailSettingPage":
                _navigationService.NavigationTo<DetailSettingPageViewModel>();
                break;
            case "AccountSettingPage":
                _navigationService.NavigationTo<AccountSettingPageViewModel>(); 
                break;
            case "NetworkSettingPage":
                _navigationService.NavigationTo<NetworkSettingPageViewModel>(); 
                break;
        }
    }
}