using Newtonsoft.Json;

namespace ZeepSDK.Versioning;

internal class ModioResponse<TData>
{
    [JsonProperty("data")]
    public TData[] Data
    {
        get; set;
    }
    [JsonProperty("result_count")]
    public int ResultCount
    {
        get; set;
    }
    [JsonProperty("result_offset")]
    public int ResultOffset
    {
        get; set;
    }
    [JsonProperty("result_limit")]
    public int ResultLimit
    {
        get; set;
    }
    [JsonProperty("result_total")]
    public int ResultTotal
    {
        get; set;
    }
}
