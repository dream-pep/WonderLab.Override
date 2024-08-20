using Avalonia.Controls;
using WonderLab.Views.Controls;
using WonderLab.ViewModels.Pages.Navigation;

namespace WonderLab.Views.Pages.Navigation;

public sealed partial class SettingNavigationPage : UserControl {
    private SettingNavigationPageViewModel _viewModel;

    public SettingNavigationPage() => InitializeComponent();

    private void OnLoaded(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
        _viewModel = DataContext as SettingNavigationPageViewModel;
        _viewModel._navigationService.NavigationRequest += x => {
            frame.Navigate(x.Page as Control);
        };
    }
}