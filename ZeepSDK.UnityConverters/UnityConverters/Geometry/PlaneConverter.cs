using Newtonsoft.Json;
using UnityEngine;
using ZeepSDK.External.Newtonsoft.Json.UnityConverters.Helpers;

namespace ZeepSDK.External.Newtonsoft.Json.UnityConverters.Geometry
{
    internal class PlaneConverter : PartialConverter<Plane>
    {
        protected override void ReadValue(ref Plane value, string name, JsonReader reader, JsonSerializer serializer)
        {
            switch (name)
            {
                case nameof(value.normal):
                    value.normal = reader.ReadViaSerializer<Vector3>(serializer);
                    break;
                case nameof(value.distance):
                    value.distance = reader.ReadAsFloat() ?? 0;
                    break;
            }
        }

        protected override void WriteJsonProperties(JsonWriter writer, Plane value, JsonSerializer serializer)
        {
            writer.WritePropertyName(nameof(value.normal));
            serializer.Serialize(writer, value.normal, typeof(Vector3));
            writer.WritePropertyName(nameof(value.distance));
            writer.WriteValue(value.distance);
        }
    }
}
