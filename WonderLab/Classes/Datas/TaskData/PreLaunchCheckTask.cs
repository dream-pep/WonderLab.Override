using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Services;
using WonderLab.Services.UI;
using WonderLab.Services.Game;
using WonderLab.Classes.Datas.ViewData;
using MinecraftLaunch.Components.Fetcher;
using System.Collections.Immutable;
using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Components.Checker;
using Avalonia.Controls.Notifications;
using WonderLab.Services.Download;
using MinecraftLaunch.Classes.Enums;
using WonderLab.Services.Auxiliary;
using MinecraftLaunch.Components.Authenticator;
using MinecraftLaunch.Classes.Models.Auth;
using WonderLab.ViewModels.Dialogs.Setting;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.MessageData;
using MinecraftLaunch.Classes.Models.Event;
using System.Diagnostics;

namespace WonderLab.Classes.Datas.TaskData;

public sealed class PreLaunchCheckTask : TaskBase {
    private readonly JavaFetcher _javaFetcher;
    private readonly ResourceChecker _resourceChecker;
    private readonly WeakReferenceMessenger _weakReferenceMessenger;

    private readonly GameService _gameService;
    private readonly DialogService _dialogService;
    private readonly AccountService _accountService;
    private readonly SettingService _settingService;
    private readonly BackendService _backendService;
    private readonly NotificationService _notificationService;

    private bool _isReturnTrue;
    private ImmutableArray<JavaEntry> _javas;
    private CancellationTokenSource _accountRefreshCancellationToken;

    public event EventHandler<bool> CanLaunch;

    public CancellationTokenSource CheckTaskCancellationToken { get; private set; }

    public PreLaunchCheckTask(
        JavaFetcher javaFetcher,
        GameService gameService,
        DialogService dialogService,
        SettingService settingService,
        AccountService accountService,
        BackendService backendService,
        NotificationService notificationService,
        WeakReferenceMessenger weakReferenceMessenger) {
        _gameService = gameService;
        _dialogService = dialogService;
        _settingService = settingService;
        _accountService = accountService;
        _backendService = backendService;
        _notificationService = notificationService;

        _javaFetcher = javaFetcher;
        _weakReferenceMessenger = weakReferenceMessenger;
        _resourceChecker = new(_gameService.ActiveGameEntry.Entry);

        JobName = "预启动检查任务";
        IsIndeterminate = true;
        CheckTaskCancellationToken = new();
        _accountRefreshCancellationToken = new();

        _weakReferenceMessenger.Register<RefreshAccountFinishMessage>(this, (_, args) => {
            _accountRefreshCancellationToken.Cancel();
            _accountRefreshCancellationToken.Dispose();
        });
    }

    public override async ValueTask BuildWorkItemAsync(CancellationToken token) {
        try {
            _isReturnTrue = true;

            _notificationService.QueueJob(new NotificationViewData {
                Title = "信息",
                Content = $"开始启动游戏实例  {_gameService.ActiveGameEntry.Entry.Id}，稍安勿躁！",
                NotificationType = NotificationType.Information, 
                JumpAction = () => { Debug.WriteLine("Jump"); }
            });

            await Task.Run(async () => {
                await CheckJavaAndExecuteAsync();
                await CheckResourcesAndExecuteAsync();
                await CheckAccountAndExecuteAsync();
            }, token).ContinueWith(x => {
                IsIndeterminate = false;
                ReportProgress(1, "预启动检查完成");
                CanLaunch?.Invoke(this, _isReturnTrue);
            }, token);
        } catch (Exception) {
            _notificationService.QueueJob(new NotificationViewData {
                Title = "错误",
                Content = "预启动检查失败，原因：遭遇了未知错误！",
                NotificationType = NotificationType.Error
            });

            await CheckTaskCancellationToken.CancelAsync();
            return;
        }
    }

    private async Task CheckJavaAndExecuteAsync() {
        ReportProgress("正在检查 Java 相关信息");
        var resultJava = await CheckJavaAsync();
        if (!resultJava.value && !resultJava.canExecute) {
            _notificationService.QueueJob(new NotificationViewData {
                Title = "错误",
                Content = "未找到 Java，请先添加一个 Java 后再尝试启动游戏!",
                NotificationType = NotificationType.Error
            });

            _isReturnTrue = false;
            CanBeCancelled = true;
            await CheckTaskCancellationToken.CancelAsync();
        } else if (!resultJava.value && resultJava.canExecute) {
            _settingService.Data.Javas = [.. _javas];
            _settingService.Data.ActiveJava = _javas.FirstOrDefault();
        }
    }

    private async Task CheckResourcesAndExecuteAsync() {
        ReportProgress("正在检查游戏本体资源完整性");

        var resultResource = await CheckResourcesAsync();

        if (!resultResource) {
            IsIndeterminate = false;
            _notificationService.QueueJob(new NotificationViewData {
                Title = "警告",
                Content = $"发现了 {_resourceChecker.MissingResources.Count} 个缺失资源，正在尝试将其补全！",
                NotificationType = NotificationType.Warning
            });

            var downloadSource = _settingService.Data.IsUseMirrorDownloadSource
                ? "bmcl"
                : "mojang";

            _backendService.Completed += OnCompleted;
            _backendService.ProgressChanged += OnProgressChanged;
            _backendService.RunResourceComplete(_gameService.ActiveGameEntry.Entry.Id,
                _gameService.ActiveGameEntry.Entry.GameFolderPath,
                _settingService.Data.MultiThreadsCount,
                downloadSource);


            void OnCompleted(object obj, EventArgs args) {
                ReportProgress(0);
            }

            void OnProgressChanged(object obj, ProgressChangedEventArgs args) {
                ReportProgress(args.Progress, $"{args.Progress}% - {args.ProgressStatus}");
            }
        }
    }

    private async Task CheckAccountAndExecuteAsync() {
        IsIndeterminate = true;
        ReportProgress("正在验证账户信息");
        var (value, canExecute) = await CheckAccountAsync();
        if (!value && canExecute) {
            ReportProgress("账户信息过期，正在执行刷新");
            _dialogService.ShowContentDialog<RefreshAccountDialogViewModel>(_settingService.Data.ActiveAccount);
            await WaitForRunAsync(_accountRefreshCancellationToken.Token);
        } else if (!value && !canExecute) {
            _notificationService.QueueJob(new NotificationViewData {
                Title = "错误",
                Content = "未找到账户信息，请先添加一个账户后再尝试启动游戏！",
                NotificationType = NotificationType.Error
            });

            CanBeCancelled = true;
            _isReturnTrue = false;
            await CheckTaskCancellationToken.CancelAsync();
            return;
        }
    }

    private async ValueTask<(bool value, bool canExecute)> CheckJavaAsync() {
        _javas = await _javaFetcher.FetchAsync();
        var data = _settingService.Data;
        if (data is null) {
            return (false, !_javas.IsEmpty);
        }

        return data.IsAutoSelectJava
            ? (data.Javas?.Count != 0, !_javas.IsEmpty)
            : (data.ActiveJava is not null, !_javas.IsEmpty);
    }

    private async ValueTask<bool> CheckResourcesAsync() {
        return await _resourceChecker.CheckAsync();
    }

    private async ValueTask<(bool value, bool canExecute)> CheckAccountAsync() {
        var activeAccount = _settingService.Data.ActiveAccount;

        if (activeAccount is null) {
            return (false, false);
        }

        try {
            switch (activeAccount.Type) {
                case AccountType.Offline:
                    return (true, false);
                case AccountType.Yggdrasil:
                    _accountService.InitializeComponent(new YggdrasilAuthenticator(activeAccount as YggdrasilAccount), AccountType.Yggdrasil);
                    _settingService.Data.ActiveAccount = (await _accountService.AuthenticateAsync(3)).FirstOrDefault();
                    break;
                case AccountType.Microsoft:
                    _accountService.InitializeComponent(new MicrosoftAuthenticator(activeAccount as MicrosoftAccount,
                        "9fd44410-8ed7-4eb3-a160-9f1cc62c824c", true),
                        AccountType.Microsoft,
                        true);

                    _settingService.Data.ActiveAccount = (await _accountService.AuthenticateAsync(2)).FirstOrDefault();
                    break;
            }

            return (true, false);
        } catch (Exception) {
            _notificationService.QueueJob(new NotificationViewData
            {
                Title = "错误",
                Content = "在检查账户时发生了错误！",
                NotificationType = NotificationType.Error
            });
            return (false, false);
        }
    }
}