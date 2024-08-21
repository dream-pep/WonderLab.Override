using WonderLab.ViewModels.Pages;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Classes.Interfaces;
using WonderLab.Services.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using WonderLab.ViewModels.Pages.Navigation;
using System.Collections.ObjectModel;
using WonderLab.Services;
using WonderLab.Classes.Datas.TaskData;
using WonderLab.Services.UI;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.MessageData;
using WonderLab.Classes.Enums;
using Avalonia.Controls;
using System.Diagnostics;
using Avalonia.Input;
using Avalonia.Interactivity;
using WonderLab.ViewModels.Dialogs;
using System.Linq;

namespace WonderLab.ViewModels.Windows;

public sealed partial class MainWindowViewModel : ViewModelBase {
    private readonly TaskService _taskService;
    private readonly DialogService _dialogService;
    private readonly SettingService _settingService;
    private readonly LanguageService _languageService;
    private readonly NotificationService _notificationService;

    public readonly HostNavigationService _navigationService;

    [ObservableProperty] private string _imagePath;

    [ObservableProperty] private int _blurRadius;

    [ObservableProperty] private object _activePage;
    [ObservableProperty] private ParallaxMode _parallaxMode;

    [ObservableProperty] private bool _isEnableBlur;
    [ObservableProperty] private bool _isOpenTaskListPanel;
    [ObservableProperty] private bool _isOpenBackgroundPanel;

    [ObservableProperty] private ReadOnlyObservableCollection<ITaskJob> _tasks;
    [ObservableProperty] private ReadOnlyObservableCollection<INotification> _notifications;

    [ObservableProperty] private Control _homePage;

    public MainWindowViewModel(
        TaskService taskService,
        DialogService dialogService,
        SettingService settingService,
        LanguageService languageService,
        HostNavigationService navigationService,
        NotificationService notificationService) {
        _taskService = taskService;
        _dialogService = dialogService;
        _settingService = settingService;
        _languageService = languageService;
        _navigationService = navigationService;
        _notificationService = notificationService;

        WeakReferenceMessenger.Default.Register<BlurEnableMessage>(this, BlurEnableValueHandle);
        WeakReferenceMessenger.Default.Register<BlurRadiusChangeMessage>(this, BlurRadiusChangeHandle);
        WeakReferenceMessenger.Default.Register<ParallaxModeChangeMessage>(this, ParallaxModeChangeHandle);
    }

    [RelayCommand]
    private void ControlTaskListPanel() {
        IsOpenTaskListPanel = !IsOpenTaskListPanel;
    }

    [RelayCommand]
    private void NavigationTo(string pageKey) {
        IsOpenBackgroundPanel = pageKey switch {
            "HomePage" => false,
            "MultiplayerPage" => true,
            "SettingNavigationPage" => true,
            "DownloadNavigationPage" => true,
            _ => false
        };

        switch (pageKey) {
            case "HomePage":
                HomePage = _navigationService.NavigationToHome();
                break;
            case "MultiplayerPage":
                _navigationService.NavigationTo<MultiplayerPageViewModel>();
                break;
            case "SettingNavigationPage":
                _navigationService.NavigationTo<SettingNavigationPageViewModel>();
                break;
            case "DownloadNavigationPage":
                _navigationService.NavigationTo<DownloadNavigationPageViewModel>();
                break;
        }
    }

    private void BlurEnableValueHandle(object obj, BlurEnableMessage blurEnable) {
        IsEnableBlur = blurEnable.IsEnableBlur;
    }

    private void BlurRadiusChangeHandle(object obj, BlurRadiusChangeMessage blurRadiusChange) {
        BlurRadius = blurRadiusChange.BlurRadius;
    }

    private void ParallaxModeChangeHandle(object obj, ParallaxModeChangeMessage parallaxModeChange) {
        ParallaxMode = parallaxModeChange.ParallaxMode switch {
            0 => ParallaxMode.None,
            1 => ParallaxMode.Flat,
            2 => ParallaxMode.Solid,
            _ => ParallaxMode.None
        };
    }

    public void OnLoaded(object sender, object args) {
        _taskService.QueueJob(new InitTask(_languageService, _settingService, _dialogService, _notificationService));

        Tasks = new(_taskService.TaskJobs);
        Notifications = new(_notificationService.Notifications);

        ParallaxMode = _settingService.Data.ParallaxMode switch {
            0 => ParallaxMode.None,
            1 => ParallaxMode.Flat,
            2 => ParallaxMode.Solid,
            _ => ParallaxMode.None,
        };

        HomePage = _navigationService.NavigationToHome();
    }

    public void OnDrop(object sender, DragEventArgs args) {
        var file = args.Data.GetFiles().First();
        _dialogService.ShowContentDialog<FileDropDialogViewModel>(file);
    }

    public void OnDragEnter(object sender, DragEventArgs args) {
        _dialogService.ShowContentDialog<FileDropDialogViewModel>();
    }

    public void OnDragLeave(object sender, DragEventArgs args) {
        _dialogService.CloseContentDialog();
    }
}