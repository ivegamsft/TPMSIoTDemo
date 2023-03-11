using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace TPMSIoTDemo.Common.Converters
{
    public class VehicleTireReadingConverter : JsonConverter
    {
        public override bool CanRead => false;

        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(VehicleTireReading));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //var jsonObject = JObject.Load(reader);
            //var currentReading = default(VehicleTireReading);
            //currentReading.CurrentTires = new List<CarTire>();

            return serializer.Deserialize(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
            //Serialize the object
            serializer.Serialize(writer, value);
        }
    }
}
