using System;
using Avalonia;
using WonderLab.Extensions;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WonderLab.Services.Download;

namespace WonderLab.Desktop;

public sealed class Program {
    [STAThread]
    public static void Main(string[] args) {
        try {
            var arg = args.FirstOrDefault() ?? "";
            UpdateService.IsDebugMode = arg.Contains("debug");
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        } catch (Exception ex) {
            var logger = App.ServiceProvider.GetService<ILogger<Program>>();
            logger!.LogError("程序遭遇了致命性错误，完整信息堆栈：{Trace}", ex.ToString());
        }
    }

    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseSystemFont()
        .LogToTrace()
        .With(new Win32PlatformOptions {
            RenderingMode = RuntimeInformation.ProcessArchitecture == Architecture.Arm || RuntimeInformation.ProcessArchitecture == Architecture.Arm64 
            ? [Win32RenderingMode.Wgl] 
            : [Win32RenderingMode.AngleEgl, Win32RenderingMode.Software]!,
        })
        .With(new MacOSPlatformOptions {
            DisableAvaloniaAppDelegate = true,
            DisableDefaultApplicationMenuItems = true,
        })
        .With(new X11PlatformOptions { 
            OverlayPopups = true,
        }).With(new SkiaOptions {
            MaxGpuResourceSizeBytes = 1073741824L
        });
}