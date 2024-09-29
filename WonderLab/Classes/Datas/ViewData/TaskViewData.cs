using WonderLab.Classes.Datas.TaskData;
using WonderLab.Classes.Interfaces;

namespace WonderLab.Classes.Datas.ViewData;

public sealed record TaskViewData(ITaskJob<TaskProgressData> TaskJob);