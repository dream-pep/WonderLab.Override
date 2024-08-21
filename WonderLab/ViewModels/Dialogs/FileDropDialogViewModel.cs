using WonderLab.ViewModels;
using WonderLab.Services.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Platform.Storage;

namespace WonderLab.ViewModels.Dialogs;
public sealed partial class FileDropDialogViewModel : DialogViewModelBase {
    [ObservableProperty] private string _fileName;

    public FileDropDialogViewModel(DialogService dialogService) {
        Initialized += OnInitialized;
    }

    private void OnInitialized(object sender, System.EventArgs e) {
        FileName = (Parameter as IStorageItem).Name;
    }
}