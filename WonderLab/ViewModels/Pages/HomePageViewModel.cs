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
using System.Threading;
using WonderLab.Classes.Datas.MessageData;

namespace WonderLab.ViewModels.Pages;

public sealed partial class HomePageViewModel : ViewModelBase {
    private readonly GameService _gameService;
    private readonly TaskService _taskService;
    private readonly LaunchService _launchService;
    private readonly DialogService _dialogService;
    private readonly AccountService _accountService;
    private readonly SettingService _settingService;
    private readonly NotificationService _notificationService;
    private readonly WeakReferenceMessenger _weakReferenceMessenger;

    [ObservableProperty] private bool _isGameEmpty;
    [ObservableProperty] private GameViewData _activeGameEntry;
    [ObservableProperty] private ObservableCollection<GameViewData> _gameEntries;

    /// <inheritdoc />
    public HomePageViewModel(
        GameService gameService,
        TaskService taskService,
        LaunchService launchService,
        DialogService dialogService,
        AccountService accountService,
        SettingService settingService,
        NotificationService notificationService,
        WeakReferenceMessenger weakReferenceMessenger) {
        _gameService = gameService;
        _taskService = taskService;
        _dialogService = dialogService;
        _launchService = launchService;
        _accountService = accountService;
        _settingService = settingService;
        _notificationService = notificationService;

        _weakReferenceMessenger = weakReferenceMessenger;
        _weakReferenceMessenger.Register<ActiveGameEntryMessage>(this, (_, args) => {
            ActiveGameEntry = args.Data;
        });

        ActiveGameEntry = gameService.ActiveGameEntry;
    }

    partial void OnActiveGameEntryChanged(GameViewData value) {
        _gameService.ActivateGameViewEntry(value);
    }

    [RelayCommand]
    private async void Launch() {
        if (_gameService.ActiveGameEntry is null) {
            _notificationService.QueueJob(new NotificationViewData {
                Title = "错误",
                Content = "无法启动，原因：未选择任何游戏实例！",
                NotificationType = NotificationType.Error
            });
            
            return;
        }

        _settingService.Data.ActiveAccount = default;
        var task = _dialogService.ShowContentDialog<AccountDropDialogViewModel>();
        await task.WaitAsync(new CancellationToken());

        if (_accountService.AccountViewData is null) {
            return;
        }

        await _launchService.LaunchWithDisplayTaskAsync(_accountService.AccountViewData.Account);
    }

    [RelayCommand]
    private void OpenGameManager() {
        _weakReferenceMessenger.Send(new GameManagerMessage());
    }
}