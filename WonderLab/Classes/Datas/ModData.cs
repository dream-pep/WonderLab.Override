using System.Text.Json.Serialization;

namespace WonderLab.Classes.Datas;

public sealed record ModData {
    [JsonPropertyName("modrinthId")] public string ModrinthId { get; set; }
    [JsonPropertyName("chineseName")] public string ChineseName { get; set; }
    [JsonPropertyName("curseforgeId")] public string CurseforgeId { get; set; }

    [JsonIgnore] public bool IsModrinthHas => !string.IsNullOrEmpty(ModrinthId);
    [JsonIgnore] public bool IsCurseforgeHas => !string.IsNullOrEmpty(CurseforgeId);

    public override string ToString() {
        if (!string.IsNullOrEmpty(ChineseName)) {
            return ChineseName;
        }

        return !string.IsNullOrEmpty(ModrinthId) ? ModrinthId :
            (!string.IsNullOrEmpty(CurseforgeId) ? CurseforgeId : "FUCK");
    }
}