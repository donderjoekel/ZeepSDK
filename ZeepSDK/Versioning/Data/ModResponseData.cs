using Newtonsoft.Json;

namespace ZeepSDK.Versioning.Data;

internal class ModResponseData
{
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("modfile")] public ModFileResponseData ModFile { get; set; }
}
