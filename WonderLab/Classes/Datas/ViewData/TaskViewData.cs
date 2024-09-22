using WonderLab.Classes.Interfaces;

namespace WonderLab.Classes.Datas.ViewData;

public record TaskViewData(ITaskJob<ITaskData> TaskJob);