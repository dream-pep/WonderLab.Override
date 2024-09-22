using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;

namespace WonderLab.Classes.Datas.TaskData;

/// <summary>
/// 可视化游戏启动任务
/// </summary>
public sealed partial class LaunchDisplayTask : ObservableObject, ITaskJobWithStep<LaunchTaskData> {
    public readonly CancellationTokenSource LaunchCancellationTokenSource = new();

    [ObservableProperty] private double _progress;
    [ObservableProperty] private bool _isIndeterminate;
    [ObservableProperty] private TaskStatus _taskStatus;

    public Exception Exception { get; set; }

    public required string JobName { get; init; }
    public CancellationToken TaskCancellationToken => LaunchCancellationTokenSource.Token;

    public event EventHandler Completed;

    public ImmutableArray<TaskStep> TaskSteps => [
        new TaskStep { StepName = "Checking required items", TaskStatus = TaskStatus.Running },
        new TaskStep { StepName = "Verifying the account" },
        new TaskStep { StepName = "Completing the game dependents" },
        new TaskStep { StepName = "Launching the game" },
    ];

    public void Report(LaunchTaskData value) {
        if(value.Step is LaunchTaskStep.Inspecting) {
            TaskSteps[0].Progress = value.Progress;
        } else if (value.Step is LaunchTaskStep.Authenticating) {
            TaskSteps[0].TaskStatus = TaskStatus.RanToCompletion;
            TaskSteps[1].TaskStatus = TaskStatus.Running;
            TaskSteps[1].Progress = value.Progress;
        } else if (value.Step is LaunchTaskStep.Completing) {
            TaskSteps[1].TaskStatus = TaskStatus.RanToCompletion;
            TaskSteps[2].TaskStatus = TaskStatus.Running;
            TaskSteps[2].Progress = value.Progress;
        } else if (value.Step is LaunchTaskStep.Launching) {
            TaskSteps[2].TaskStatus = TaskStatus.RanToCompletion;
            TaskSteps[3].TaskStatus = TaskStatus.Running;
            TaskSteps[3].Progress = value.Progress;

            Completed?.Invoke(this, EventArgs.Empty);
        }
    }

    [RelayCommand]
    private void CancelTask() {
        LaunchCancellationTokenSource.Cancel();
    }
}

public enum LaunchTaskStep {
    Inspecting,
    Authenticating,
    Completing,
    Launching,
    Faulted
}

public record struct LaunchTaskData(LaunchTaskStep Step, double Progress, Exception Exception = default) : ITaskData;