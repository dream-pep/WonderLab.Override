using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Classes.Models.Install;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using WonderLab.Services.Download;

namespace WonderLab.ViewModels.Pages.Download;
public sealed partial class MinecraftListPageViewModel : ViewModelBase {
    //private readonly IEnumerable<VersionManifestEntry> _minecraftList;

    [ObservableProperty] private IEnumerable _minecraftList;

    public MinecraftListPageViewModel(DownloadService downloadService) {
        _ = Task.Run(async () => {
            MinecraftList = await downloadService.GetMinecraftListAsync();
        });
    }
}