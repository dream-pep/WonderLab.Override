using Flurl.Http;
using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Collections.Generic;
using MinecraftLaunch.Extensions;
using System.Collections.Immutable;
using WonderLab.Classes.Datas.ViewData;

namespace WonderLab.Services.Game;

public sealed class GameNewsService {
    public const string JavaPatchNotesUrl = "https://launchercontent.mojang.com/v2/javaPatchNotes.json";

    public async Task<IEnumerable<JavaPatchNoteViewData>> GetJavaPatchNotesAsync() {
        var json = await JavaPatchNotesUrl.GetStringAsync();
        var node = JsonNode.Parse(json);
        var entries = node.Select("entries").AsArray();
        return entries.Select(x => new JavaPatchNoteViewData {
            Type = x.GetString("type"),
            Title = x.GetString("title"),
            Version = x.GetString("version"),
            Description = x.GetString("shortText"),
            Date = x.Select("date").GetValue<DateTime>(),
            ImageUrl = $"https://launchercontent.mojang.com{x.Select("image").GetString("url")}",
        }).ToImmutableArray().Sort((x, y) => y.Date.CompareTo(x.Date));
    }
}