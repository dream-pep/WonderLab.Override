using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using WonderLab.ViewModels.Windows;

namespace WonderLab.Views.Windows;

public sealed partial class MainWindow : Window {
    private MainWindowViewModel _viewModel;

    public MainWindow() => InitializeComponent();

    private void OnLoaded(object sender, RoutedEventArgs e) {
        _viewModel = DataContext as MainWindowViewModel;

        _viewModel._navigationService.NavigationRequest += x => {
            frame.Navigate(x.Page as Control);
        };

        AddHandler(DragDrop.DropEvent, _viewModel.OnDrop);
    }
}