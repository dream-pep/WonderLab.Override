using Avalonia.Controls;
using WonderLab.ViewModels.Windows;
using WonderLab.Views.Controls;

namespace WonderLab.Views.Windows;

public partial class MainWindow : Window {
    private MainWindowViewModel _viewModel;

    public MainWindow() => InitializeComponent();

    private void OnLoaded(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
        _viewModel = DataContext as MainWindowViewModel;
        _viewModel._navigationService.NavigationRequest += x => {
            frame.Navigate(x.Page as Control);
        };
    }
}