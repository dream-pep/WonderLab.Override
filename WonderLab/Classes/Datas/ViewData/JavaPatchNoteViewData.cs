using System;

namespace WonderLab.Classes.Datas.ViewData;

public sealed record JavaPatchNoteViewData {
    public string Type { get; set; }
    public string Title { get; set; }
    public string Version { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; }

    public DateTime Date { get; set; }
    public string DateText => Date.ToString("yyyy/MM/dd HH:mm:ss");
}