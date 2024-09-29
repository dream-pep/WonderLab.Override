using Avalonia.Threading;
using MinecraftLaunch;
using MinecraftLaunch.Classes.Enums;
using MinecraftLaunch.Classes.Models.Auth;
using MinecraftLaunch.Classes.Models.Launch;
using MinecraftLaunch.Components.Checker;
using MinecraftLaunch.Components.Downloader;
using MinecraftLaunch.Components.Launcher;
using MinecraftLaunch.Extensions;
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

        await Task.Run(async () => await LaunchAsync(account, task, task.LaunchCancellationTokenSource));
    }

    public async Task LaunchAsync(
        Account account,
        IProgress<TaskProgressData> progress,
        CancellationTokenSource cancellationToken) {
        try {
            var settingData = _settingService.Data;

            if (settingData.ActiveJava is null) {
                //TODO: Text translation
                throw new ArgumentException("未选择任何 Java 实例");
            }

            var javaPath = settingData.IsAutoSelectJava ? settingData.ActiveJava : settingData.ActiveJava;
            progress.Report(new(1, 1d));
            cancellationToken.Token.ThrowIfCancellationRequested();

            //Auth
            await RefreshAccountAsync(account);

            progress.Report(new(2, 1d));
            cancellationToken.Token.ThrowIfCancellationRequested();

            //Complete
            await ResolveAndDownloadResourceAsync(progress);

            progress.Report(new(3, 1d));
            cancellationToken.Token.ThrowIfCancellationRequested();

            //Launch
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

    private async Task ResolveAndDownloadResourceAsync(IProgress<TaskProgressData> progress, CancellationTokenSource cancellationTokenSource = default) {
        ResourceChecker resourceChecker = new(_gameService.ActiveGameEntry.Entry);
        if (await resourceChecker.CheckAsync()) {
            return;
        }

        ResourceDownloader resourceDownloader = new(new() {
            MultiThreadsCount = _settingService.Data.MultiThreadsCount
        }, resourceChecker.MissingResources, _settingService.Data.IsUseMirrorDownloadSource ? MirrorDownloadManager.Bmcl : default, cancellationTokenSource);

        resourceDownloader.ProgressChanged += (_, arg) => {
            progress.Report(new(3, arg.ToPercentage()));
        };

        await resourceDownloader.DownloadAsync();
    }
}