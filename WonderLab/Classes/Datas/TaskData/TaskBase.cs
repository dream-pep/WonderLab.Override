using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;

namespace WonderLab.Classes.Datas.TaskData;

/// <summary>
/// 任务基类
/// </summary>
public abstract partial class TaskBase : ObservableObject, ITaskJob, IDisposable {
    private bool _insideIsIndeterminate;
    private bool _isTaskFinishedEventFired;

    private double _insideProgress;
    private string _insideProgressDetail;

    private readonly DispatcherTimer _timer = new(DispatcherPriority.Background) {
        Interval = TimeSpan.FromSeconds(0.35d),
    };

    [ObservableProperty] private double _progress;

    [ObservableProperty] private string _jobName;
    [ObservableProperty] private string _progressDetail;

    [ObservableProperty] private bool _canBeCancelled;
    [ObservableProperty] private bool _isIndeterminate;
    [ObservableProperty] private bool _isDeletedRequested;

    [ObservableProperty] private TaskStatus _taskStatus;
    [ObservableProperty] private ValueTask? _workingTask;

    public CancellationTokenSource CancellationTokenSource => new();

    public event EventHandler<EventArgs> TaskFinished;

    public TaskBase() {
        _timer.Tick += async (_, args) => {
            await _timer.Dispatcher.InvokeAsync(() => {
                Progress = _insideProgress;
                ProgressDetail = _insideProgressDetail;
            });
        };

        _timer.Start();
    }

    public async ValueTask<TaskStatus> WaitForRunAsync(CancellationToken token) {
        await Task.Delay(200, token);
        while (!token.IsCancellationRequested) {
            if ((uint)(TaskStatus - 5) <= 2u) {
                break;
            }

            await Task.Delay(250, token);
        }

        return TaskStatus;
    }

    public void InvokeTaskFinished() {
        if (!_isTaskFinishedEventFired) {
            _timer.Stop();
            TaskFinished?.Invoke(this, EventArgs.Empty);
            _isTaskFinishedEventFired = true;
        }
    }

    public abstract ValueTask BuildWorkItemAsync(CancellationToken token);

    public virtual void Dispose() {
        CancellationTokenSource.Dispose();
    }

    [RelayCommand(CanExecute = nameof(CanCancelTask))]
    private void CancelTask() {
        CanBeCancelled = false;
        TaskStatus = TaskStatus.Canceled;
        CancellationTokenSource?.Cancel();
    }

    [RelayCommand(CanExecute = nameof(CanDeleteTask))]
    private void RequestDelete() {
        IsDeletedRequested = true;
        CanBeCancelled = false;
    }

    private bool CanCancelTask() {
        return CanBeCancelled;
    }

    private bool CanDeleteTask() {
        return !IsDeletedRequested;
    }

    protected void ReportProgress(string detail) {
        if (!string.IsNullOrEmpty(detail)) {
            _insideProgressDetail = detail;
        }
    }

    protected void ReportProgress(double progress) {
        if (progress < 0.0) {
            IsIndeterminate = true;
        }

        _insideProgress = progress;
    }

    protected void ReportProgress(double progress, string detail) {
        ReportProgress(progress);
        ReportProgress(detail);
    }
}