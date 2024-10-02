using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Classes.Enums;
using MinecraftLaunch.Classes.Interfaces;
using MinecraftLaunch.Classes.Models.Auth;
using MinecraftLaunch.Components.Authenticator;
using WonderLab.Classes.Datas.ViewData;

namespace WonderLab.Services.Auxiliary;

/// <summary>
/// 游戏账户服务类
/// </summary>
public sealed class AccountService {
    private const string CLIENT_ID = "9fd44410-8ed7-4eb3-a160-9f1cc62c824c";

    private readonly SettingService _settingService;
    private readonly ILogger<AccountService> _logger;

    public AccountViewData AccountViewData { get; set; }

    public AccountService(ILogger<AccountService> logger, SettingService settingService) {
        _logger = logger;
        _settingService = settingService;
    }

    public async IAsyncEnumerable<AccountViewData> InitializeAccountsAsync() {
        var accounts = _settingService.Data.Accounts;

        foreach (var account in accounts) {
            yield return await Task.Run(() => new AccountViewData(account));
        }
    }

    /// <summary>
    /// 异步验证
    /// </summary>
    public async ValueTask<IEnumerable<YggdrasilAccount>> AuthenticateYggdrasilAsync(string url, string email, string password, YggdrasilAccount account = default) {
        YggdrasilAuthenticator yggdrasilAuthenticator = account is null ? new(url, email, password) : new(account);
        return await yggdrasilAuthenticator.AuthenticateAsync();
    }

    /// <summary>
    /// 异步验证
    /// </summary>
    public async ValueTask<MicrosoftAccount> AuthenticateMicrosoftAsync(Account account = default, Action<DeviceCodeResponse> action = default, CancellationTokenSource tokenSource = default) {
        MicrosoftAuthenticator microsoftAuthenticator = new(account as MicrosoftAccount, CLIENT_ID, true);

        if (account is null) {
            await microsoftAuthenticator.DeviceFlowAuthAsync(action, tokenSource);
        }

        return await microsoftAuthenticator.AuthenticateAsync();
    }

    /// <summary>
    /// 异步验证
    /// </summary>
    public OfflineAccount AuthenticateOffline(string name, Guid uuid = default) {
        return new OfflineAuthenticator(name, uuid).Authenticate();
    }
}