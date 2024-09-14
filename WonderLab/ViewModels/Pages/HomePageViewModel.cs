using System.Linq;
using WonderLab.Services;
using WonderLab.Extensions;
using System.Threading.Tasks;
using WonderLab.Services.Game;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WonderLab.Classes.Datas.TaskData;
using WonderLab.Classes.Datas.ViewData;
using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Components.Fetcher;
using WonderLab.Services.UI;
using WonderLab.Services.Auxiliary;
using WonderLab.Services.Download;
using CommunityToolkit.Mvvm.Messaging;
using Avalonia.Controls.Notifications;
using WonderLab.ViewModels.Dialogs;
using System;

namespace WonderLab.ViewModels.Pages;

public sealed partial class HomePageViewModel : ViewModelBase {
    private readonly GameService _gameService;
    private readonly TaskService _taskService;
    private readonly SettingService _settingService;
    private readonly DialogService _dialogService;
    private readonly NotificationService _notificationService;

    [ObservableProperty] private bool _isGameEmpty;
    [ObservableProperty] private GameViewData _activeGameEntry;
    [ObservableProperty] private ObservableCollection<GameViewData> _gameEntries;

    /// <inheritdoc />
    public HomePageViewModel(
        GameService gameService,
        TaskService taskService,
        DialogService dialogService,
        SettingService settingService,
        NotificationService notificationService) {
        _gameService = gameService;
        _taskService = taskService;
        _dialogService = dialogService;
        _settingService = settingService;
        _notificationService = notificationService;

        GameEntries = _gameService.GameEntries.ToObservableList();
        IsGameEmpty = GameEntries.Count == 0;

        RunBackgroundWork(async() => {
            await Task.Delay(250);
            ActiveGameEntry = _gameService.ActiveGameEntry;
        });
    }

    partial void OnActiveGameEntryChanged(GameViewData value) {
        _gameService.ActivateGameViewEntry(value);
    }

    [RelayCommand]
    private void Launch() {
        if (_gameService.ActiveGameEntry is null) {
            _notificationService.QueueJob(new NotificationViewData {
                Title = "错误",
                Content = "无法启动，原因：未选择任何游戏实例！",
                NotificationType = NotificationType.Error
            });
            
            return;
        }

        _settingService.Data.ActiveAccount = default;
        _dialogService.ShowContentDialog<AccountDropDialogViewModel>();

        var preCheckTask = new PreLaunchCheckTask(App.GetService<JavaFetcher>(),
            _gameService,
            App.GetService<DialogService>(),
            _settingService, App.GetService<AccountService>(),
            App.GetService<BackendService>(),
            _notificationService,
            App.GetService<WeakReferenceMessenger>());

        _taskService.QueueJob(preCheckTask);
    }
}