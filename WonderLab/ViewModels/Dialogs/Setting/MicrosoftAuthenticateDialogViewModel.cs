using System.Threading.Tasks;
using WonderLab.Services.Auxiliary;
using MinecraftLaunch.Classes.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Components.Authenticator;
using System.Linq;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Services.UI;
using WonderLab.Services;
using Avalonia.Controls.Notifications;
using System.Xml.Linq;
using WonderLab.Classes.Datas.ViewData;
using MinecraftLaunch.Classes.Models.Auth;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.MessageData;

namespace WonderLab.ViewModels.Dialogs.Setting;
public sealed partial class MicrosoftAuthenticateDialogViewModel : DialogViewModelBase {
    private readonly WindowService _windowService;
    private readonly DialogService _dialogService;
    private readonly AccountService _accountService;
    private readonly SettingService _settingService;
    private readonly NotificationService _notificationService;

    [ObservableProperty] private string _deviceCode;
    [ObservableProperty] private bool _isCodeLoadFinish;

    public MicrosoftAuthenticateDialogViewModel(AccountService accountService,
        WindowService windowService,
        DialogService dialogService,
        SettingService settingService,
        NotificationService notificationService) {
        _dialogService = dialogService;
        _windowService = windowService;
        _accountService = accountService;
        _settingService = settingService;
        _notificationService = notificationService;

        _ = InitAsync();
    }

    private async ValueTask InitAsync() {
        var account = (await _accountService.AuthenticateMicrosoftAsync(default, x => {
            IsCodeLoadFinish = true;
            DeviceCode = x.UserCode;

            Process.Start(new ProcessStartInfo(x.VerificationUrl) {
                UseShellExecute = true,
                Verb = "open"
            }).Dispose();
        }));

        _settingService.Data.Accounts.Add(account);
        _notificationService.QueueJob(new NotificationViewData {
            Title = "成功",
            Content = $"已成功将账户 {account.Name} 添加至 WonderLab！",
            NotificationType = NotificationType.Success
        });

        //WeakReferenceMessenger.Default.Send(new AccountMessage(account));
        if (_dialogService.IsDialogOpen) {
            _dialogService.CloseContentDialog();
        }
    }

    [RelayCommand]
    private void CopyDeviceCode() {
        _windowService.CopyText(DeviceCode);
    }

    [RelayCommand]
    private void OpenUrl() {
        var url = "https://www.microsoft.com/link";

        Process.Start(new ProcessStartInfo(url) {
            UseShellExecute = true,
            Verb = "open"
        }).Dispose();
    }
}