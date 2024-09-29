using Avalonia.Threading;
using MinecraftLaunch;
using MinecraftLaunch.Classes.Enums;
using MinecraftLaunch.Classes.Models.Auth;
using MinecraftLaunch.Classes.Models.Launch;
using MinecraftLaunch.Components.Checker;
using MinecraftLaunch.Components.Downloader;
using MinecraftLaunch.Components.Launcher;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Datas.TaskData;
using WonderLab.Services.Auxiliary;

namespace WonderLab.Services.Game;

/// <summary>
/// 游戏启动服务类
/// </summary>
public sealed class LaunchService {
    private readonly TaskService _taskService;
    private readonly GameService _gameService;
    private readonly SettingService _settingService;
    private readonly AccountService _accountService;
    private readonly NotificationService _notificationService;

    public LaunchService(GameService gameService, TaskService taskService, AccountService accountService, SettingService settingService, NotificationService notificationService) {
        _taskService = taskService;
        _gameService = gameService;
        _accountService = accountService;
        _settingService = settingService;
        _notificationService = notificationService;
    }

    public async Task LaunchWithDisplayTaskAsync(Account account) {
        var task = new LaunchDisplayTask {
            JobName = $"{_gameService.ActiveGameEntry.Entry.Id} 的启动任务",
        };

        _taskService.QueueJob(task);
        //await LaunchAsync(account, task, task.LaunchCancellationTokenSource);
    }

    public async Task LaunchAsync(
        Account account,
        IProgress<TaskProgressData> progress,
        CancellationTokenSource cancellationToken) {
        double progressCache = 0d;
        DispatcherTimer dispatcherTimer = new(DispatcherPriority.ApplicationIdle) {
            Interval = TimeSpan.FromSeconds(0.2d),
        };

        dispatcherTimer.Tick += (_, _) => {
            progress.Report(new(3, progressCache));
        };

        try {
            var settingData = _settingService.Data;

            progress.Report(new(1, 0d));

            if (settingData.ActiveJava is null) {
                //TODO: Text translation
                throw new ArgumentException("未选择任何 Java 实例");
            }

            var javaPath = settingData.IsAutoSelectJava ? settingData.ActiveJava : settingData.ActiveJava;
            progress.Report(new(1, 1d));
            cancellationToken.Token.ThrowIfCancellationRequested();

            //Auth
            progress.Report(new(2, 0d));

            await RefreshAccountAsync(account);

            progress.Report(new(2, 1d));
            cancellationToken.Token.ThrowIfCancellationRequested();

            //Complete
            progress.Report(new(3, 0d));

            await ResolveAndDownloadResourceAsync(x => progressCache = x, cancellationToken);

            progress.Report(new(3, 1d));
            cancellationToken.Token.ThrowIfCancellationRequested();

            //Launch
            progress.Report(new(4, 0d));

            Launcher launcher = new(_gameService.GameResolver, new() {
                JvmConfig = new JvmConfig(javaPath.JavaPath) {
                    MaxMemory = _settingService.Data.MaxMemory
                },
                Account = account,
                IsEnableIndependencyCore = _settingService.Data.IsGameIndependent,
                LauncherName = "WonderLab"
            });

            await launcher.LaunchAsync(_gameService.ActiveGameEntry.Entry.Id);

            progress.Report(new(4, 1d));
        } catch (Exception ex) {
            progress.Report(new(-1, 1d, ex));
        }
    }

    private async Task RefreshAccountAsync(Account oldAccount) {
        Account account = oldAccount.Type switch {
            AccountType.Microsoft => await _accountService.AuthenticateMicrosoftAsync(oldAccount),
            AccountType.Offline => _accountService.AuthenticateOffline(oldAccount.Name, oldAccount.Uuid),
            AccountType.Yggdrasil => (await _accountService.AuthenticateYggdrasilAsync(string.Empty, string.Empty, string.Empty, (YggdrasilAccount)oldAccount)).First(),
            _ => throw new ArgumentException("")
        };
    }

    private async Task ResolveAndDownloadResourceAsync(Func<double, double> func, CancellationTokenSource cancellationTokenSource = default) {
        ResourceChecker resourceChecker = new(_gameService.ActiveGameEntry.Entry);
        if (await resourceChecker.CheckAsync()) {
            return;
        }

        ResourceDownloader resourceDownloader = new(new() {
            MultiThreadsCount = _settingService.Data.MultiThreadsCount
        }, resourceChecker.MissingResources, _settingService.Data.IsUseMirrorDownloadSource ? MirrorDownloadManager.Bmcl : default, cancellationTokenSource);

        resourceDownloader.ProgressChanged += (_, arg) => {
            func(arg.CompletedCount / arg.TotalCount);
        };

        await resourceDownloader.DownloadAsync();
    }
}