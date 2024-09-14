using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections;
using WonderLab.Services;
using WonderLab.Services.UI;

namespace WonderLab.ViewModels.Dialogs;

public sealed partial class AccountDropDialogViewModel : DialogViewModelBase {
    private readonly SettingService _settingService;
    private readonly DialogService _dialogService;

    [ObservableProperty] private IEnumerable _accounts;

    public AccountDropDialogViewModel(SettingService settingService, DialogService dialogService) {
        _dialogService = dialogService;
        _settingService = settingService;

        Accounts = _settingService.Data.Accounts;
    }

    [RelayCommand]
    private void Cancel() {
        _dialogService.CloseContentDialog();
    }

    [RelayCommand]
    private void Confirm() {
        _dialogService.CloseContentDialog();
    }
}