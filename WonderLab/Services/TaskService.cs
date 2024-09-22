using WonderLab.Classes.Interfaces;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Avalonia.Threading;
using WonderLab.Classes.Datas.ViewData;

namespace WonderLab.Services;

public sealed partial class TaskService(ILogger<TaskService> logger) : ObservableObject {
    private readonly ILogger<TaskService> _logger = logger;

    [ObservableProperty] private ObservableCollection<TaskViewData> _displayTasks = [];

    public void QueueJob<T>(ITaskJob<T> job) where T : ITaskData {
        if (job is not ITaskJob<ITaskData> jobData) {
            return;
        }

        _ = Task.Run(async () => {
            job.Completed += (_, _) => {
                DisplayTasks.Remove(new(jobData));
            };
            
            await Dispatcher.UIThread.InvokeAsync(() => {
                DisplayTasks.Add(new(jobData));
            }, DispatcherPriority.Background);
        });
    }

    public void QueueJobWithStep<T>(ITaskJob<T> job) where T : ITaskData {
        if (job is not ITaskJobWithStep<ITaskData> jobData) {
            return;
        }

        _ = Task.Run(async () => {
            job.Completed += (_, _) => {
                DisplayTasks.Remove(new(jobData));
            };

            await Dispatcher.UIThread.InvokeAsync(() => {
                DisplayTasks.Add(new(jobData));
            }, DispatcherPriority.Background);
        });
    }
}