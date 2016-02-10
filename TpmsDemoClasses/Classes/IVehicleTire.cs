using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TpmsDemoClasses
{
    public interface IVehicleTire
    {
        Guid Id { get; set; }
        int MaxPressure { get; set; }
        int MinPressure { get; set; }
        int PositionNumber { get; set; }
        int DistanceTraveledInMiles { get; set; }
        DateTime InstallDate { get; set; }
        int MaxSpeedRating { get; set; }
        int MaxDistanceRating { get; set; }
        int Diameter { get; set; }
        int GetCurrentPressure();
        int GetCurrentSpeed();
        bool IsUnderInflated();
        bool IsOverInflated();
        bool IsFlat ();
        Guid CurrentVehicleId { get; set; }
        TireReading Read(VehicleTireReading ParentReading);
        void Move(int Increment);
        void Stop();
        void AddPressure(int Increment);
        void DecreasePressure(int Increment);
        void Flatten();
        void Slow(int Increment);
        int CalcMaxSpeedRating();
        int CalcMaxPressure();
        int CalcDiameter();
        int CalcMinPressure();
        int CalcMaxDistanceRating();
        TimeSpan CalcRelativeTireAge();
    }
}
