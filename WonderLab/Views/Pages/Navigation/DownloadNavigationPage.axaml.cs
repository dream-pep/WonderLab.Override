using Avalonia.Controls;
using WonderLab.Views.Controls;
using WonderLab.ViewModels.Pages.Navigation;

namespace WonderLab.Views.Pages.Navigation;

public sealed partial class DownloadNavigationPage : UserControl {
    private DownloadNavigationPageViewModel _viewModel;

    public DownloadNavigationPage() => InitializeComponent();

    private void OnLoaded(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
        _viewModel = DataContext as DownloadNavigationPageViewModel;
        _viewModel._navigationService.NavigationRequest += x => {
            frame.Navigate(x.Page as Control);
        };
    }
}