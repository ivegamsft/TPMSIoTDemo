
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TPMSIoTDemo.Common
{
    class Common
    {
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CarType
    {
        //[EnumMember(Value = "Sedan")]
        Sedan,
        //[EnumMember(Value = "SUV")]
        SUV,
        //[EnumMember(Value = "Compact")]
        Compact,
        //[EnumMember(Value = "Coupe")]
        Coupe
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TruckType
    {
        //[EnumMember(Value = "SemiNoTrailer")]
        SemiNoTrailer,
        //[EnumMember(Value = "SemiWithTrailer")]
        SemiWithTrailer
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum VehicleType
    {
        //[EnumMember(Value = "Car")]
        Car,
        //[EnumMember(Value = "Truck")]
        Truck
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum VehicleState
    {
        //[EnumMember(Value = "Stopped")]
        Stopped,
        //[EnumMember(Value = "Moving")]
        Moving,
        //[EnumMember(Value = "Parked")]
        Parked
    }
}
