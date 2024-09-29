using WonderLab.Classes.Interfaces;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Avalonia.Threading;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Classes.Datas.TaskData;

namespace WonderLab.Services;

public sealed partial class TaskService(ILogger<TaskService> logger) : ObservableObject {
    private readonly ILogger<TaskService> _logger = logger;

    [ObservableProperty] private ObservableCollection<TaskViewData> _displayTasks = [];

    public void QueueJob(ITaskJob<TaskProgressData> job) {
        _ = Task.Run(async () => {
            job.Completed += (_, _) => {
                DisplayTasks.Remove(new(job));
            };
            
            await Dispatcher.UIThread.InvokeAsync(() => {
                DisplayTasks.Add(new(job));
            }, DispatcherPriority.Background);
        });
    }
}