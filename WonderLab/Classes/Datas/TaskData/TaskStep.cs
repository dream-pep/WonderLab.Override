using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace WonderLab.Classes.Datas.TaskData;

/// <summary>
/// 可视化任务步骤项
/// </summary>
public partial class TaskStep : ObservableObject {
    [ObservableProperty] private string _stepName;
    [ObservableProperty] private double _maxProgress = 1d;
    [ObservableProperty] private TaskStatus _taskStatus = TaskStatus.WaitingToRun;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressText))]
    private double _progress;

    public double ProgressValue => Progress * 100;
    public string ProgressText => Progress.ToString("P2");
}