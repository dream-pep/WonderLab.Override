using System.Threading.Tasks;
using System.Collections.Generic;
using MinecraftLaunch;
using MinecraftLaunch.Components.Installer;
using MinecraftLaunch.Classes.Models.Install;

namespace WonderLab.Services.Download;

/// <summary>
/// 下载服务类
/// </summary>
public sealed class DownloadService {
    private readonly SettingService _settingService;

    public DownloadService(SettingService settingService) {
        _settingService = settingService;
    }

    public async Task<IEnumerable<VersionManifestEntry>> GetMinecraftListAsync() {
        var result = await VanlliaInstaller.EnumerableGameCoreAsync(_settingService.Data.IsUseMirrorDownloadSource 
            ? MirrorDownloadManager.Bmcl
            : default);

        return result;
    }
}