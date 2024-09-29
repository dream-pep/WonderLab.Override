using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Datas.TaskData;

namespace WonderLab.Classes.Interfaces;

public interface ITaskJob<in T> : IProgress<T> {
    string JobName { get; }
    double MaxProgress { get; }
    string ProgressText { get; }
    double Progress { get; set; }
    bool IsIndeterminate { get; set; }
    Exception Exception { get; }
    TaskStatus TaskStatus { get; set; }
    IRelayCommand CancelTaskCommand { get; }
    CancellationToken TaskCancellationToken { get; }

    ImmutableArray<TaskStep> TaskSteps { get; }

    event EventHandler Completed;
}
