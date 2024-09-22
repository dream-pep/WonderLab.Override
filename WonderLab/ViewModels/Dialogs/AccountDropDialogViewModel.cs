using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.Linq;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Services;
using WonderLab.Services.Auxiliary;
using WonderLab.Services.UI;

namespace WonderLab.ViewModels.Dialogs;

public sealed partial class AccountDropDialogViewModel : DialogViewModelBase {
    private readonly SettingService _settingService;
    private readonly DialogService _dialogService;
    private readonly AccountService _accountService;

    [ObservableProperty] private IEnumerable _accounts;
    [ObservableProperty] private AccountViewData _activeAccount;

    public AccountDropDialogViewModel(SettingService settingService, DialogService dialogService, AccountService accountService) {
        _dialogService = dialogService;
        _accountService = accountService;
        _settingService = settingService;

        Accounts = _settingService.Data.Accounts.Select(x => new AccountViewData(x));
    }

    [RelayCommand]
    private void Cancel() {
        _dialogService.CloseContentDialog();
    }

    [RelayCommand]
    private void Confirm() {
        _dialogService.CloseContentDialog();
    }

    partial void OnActiveAccountChanged(AccountViewData value) {
        _accountService.AccountViewData = value;
    }
}