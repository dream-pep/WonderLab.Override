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
using MinecraftLaunch.Components.Downloader;
using MinecraftLaunch.Utilities;
using MinecraftLaunch;
using Waher.Script.Functions.ComplexNumbers;
using MinecraftLaunch.Extensions;

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
        var data = _settingService.Data ?? new();

        try {
            //Check Java
            ReportProgress(0.35d, "正在检查 Java 相关信息");
            if (data.Javas.Count is 0) {
                var javas = await _javaFetcher.FetchAsync();
                data.Javas.AddRange(javas);
            }

            if ((data.IsAutoSelectJava && data.Javas is { Count: 0 }) || (!data.IsAutoSelectJava && data.ActiveJava is null)) {
                _notificationService.QueueJob(new NotificationViewData {
                    Title = "错误",
                    Content = data.IsAutoSelectJava ? "预启动检查失败，原因：未添加任何 Java！" : "预启动检查失败，原因：未选择任何 Java！",
                    NotificationType = NotificationType.Error
                });

                InvokeTaskFinished();
                return;
            }

            //Check Resource
            if (!await await Task.Run(_resourceChecker.CheckAsync)) {
                _notificationService.QueueJob(new NotificationViewData {
                    Title = "警告",
                    Content = $"发现了 {_resourceChecker.MissingResources.Count} 个缺失资源，正在尝试将其补全！",
                    NotificationType = NotificationType.Warning
                });

                ResourceDownloader resourceDownloader = new(new() {
                    IsPartialContentSupported = true,
                    FileSizeThreshold = 1024 * 1024 * 3,
                    MultiThreadsCount = data.MultiThreadsCount,
                    MultiPartsCount = 8
                },
                    _resourceChecker.MissingResources,
                    data.IsUseMirrorDownloadSource ? MirrorDownloadManager.Bmcl : default);

                IsIndeterminate = false;
                resourceDownloader.ProgressChanged += OnProgressChanged;
                await resourceDownloader.DownloadAsync();
                IsIndeterminate = true;
            }

            //Check Account
            if (data.ActiveAccount is null) {
                _notificationService.QueueJob(new NotificationViewData {
                    Title = "错误",
                    Content = "预启动检查失败，原因：未选择任何账户！",
                    NotificationType = NotificationType.Error
                });
            }
        } catch (Exception) {
            _notificationService.QueueJob(new NotificationViewData {
                Title = "错误",
                Content = "预启动检查失败，原因：遭遇了未知错误！",
                NotificationType = NotificationType.Error
            });

            InvokeTaskFinished();
        }
    }

    private void OnProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
        ReportProgress(e.ToPercentage() * 100, $"{e.ToPercentage() * 100:0.00}% - {e.CompletedCount}/{e.TotalCount}");
    }
}