using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace WonderLab.Classes.Datas.TaskData;

/// <summary>
/// 可视化任务步骤项
/// </summary>
public partial class TaskStep : ObservableObject {
    [ObservableProperty] private string _stepName;
    [ObservableProperty] private double _progress;
    [ObservableProperty] private double _maxProgress;
    [ObservableProperty] private TaskStatus _taskStatus = TaskStatus.WaitingToRun;

    public string ProgressText => Progress.ToString("P2");
}