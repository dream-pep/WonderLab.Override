using Avalonia.Threading;
using MinecraftLaunch.Classes.Models.Event;
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Waher.Script.Functions.ComplexNumbers;
using System.Threading.Tasks;
using MinecraftLaunch.Utilities;

namespace WonderLab.Services.Download;

/// <summary>
/// 后台程序处理器
/// </summary>
public sealed partial class BackendService {
    private readonly DispatcherTimer _timer = new() {
        Interval = TimeSpan.FromSeconds(0.25d)
    };
    private readonly string _backendDllPath = Path
        .Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Blessing-Studio", 
            "wonderlab",
            "backend",
            "WonderLab.Desktop.Backend.dll");

    public event EventHandler Completed;
    public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

    [GeneratedRegex(@"\[(\d+/\d+)\]\[(\d+\.\d+)\]")]
    private static partial Regex ExtractProgressRegex();

    /// <summary>
    /// 以资源补全模式启动后台程序
    /// </summary>
    public void RunResourceComplete(string id, string path, int thread, string source = "mojang") {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(path)) {
            ArgumentException.ThrowIfNullOrEmpty(id, nameof(id));
        }

        StringBuilder stringBuilder = new($"dotnet {_backendDllPath} --completion ");
        stringBuilder.Append("-id ")
            .Append(id)
            .Append(" -thread ")
            .Append(thread)
            .Append(" -source ")
            .Append(source)
            .Append(" -path ")
            .Append(path);

        RunCore(stringBuilder.ToString());
    }

    private void RunCore(string args) {
        var process = Process.Start(new ProcessStartInfo {
            FileName = EnvironmentUtil.IsWindow ? "powershell.exe" : "/bin/bash",
            Arguments = args,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
        });

        string arg = null;
        _timer.Tick += (sender, e) => {
            if (string.IsNullOrEmpty(arg)) {
                return;
            }

            var matches = ExtractProgressRegex().Match(arg);
            if (matches.Success) {
                ProgressChanged?.Invoke(this, new(TaskStatus.Running, Convert.ToDouble(matches.Groups[2].Value), matches.Groups[1].Value));
            }
        };

        process.Exited += (s, a) => {
            Completed?.Invoke(this, EventArgs.Empty);
            _timer.Stop();
        };

        process.OutputDataReceived += (sender, args) => {
            if (string.IsNullOrEmpty(args.Data)) {
                return;
            }

            arg = args.Data;
        };

        _timer.Start();

        process.BeginOutputReadLine();
        process.WaitForExit();
    }
}