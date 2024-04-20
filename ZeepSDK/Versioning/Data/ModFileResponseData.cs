using Newtonsoft.Json;

namespace ZeepSDK.Versioning.Data;

internal class ModFileResponseData
{
    [JsonProperty("id")]
    public int Id
    {
        get; set;
    }
}
