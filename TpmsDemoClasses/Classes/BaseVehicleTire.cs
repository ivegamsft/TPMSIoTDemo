using System;

namespace TPMSIoTDemo.Common
{
    public class BaseVehicleTire
    {
        public Guid Id { get; set; }
        public int MaxPressure { get; set; }
        public int MinPressure { get; set; }
        public int PositionNumber { get; set; }
        public int DistanceTraveledInMiles { get; set; }
        public DateTime InstallDate { get; set; }
        public int MaxSpeedRating { get; set; }
        public int MaxDistanceRating { get; set; }
        public int Diameter { get; set; }
        public virtual int GetCurrentPressure() { return 0; }
        public virtual int GetCurrentSpeed() { return 0; }
        public virtual bool IsUnderInflated() { return false; }
        public virtual bool IsOverInflated() { return false; }
        public virtual bool IsFlat() { return false; }
        public Guid CurrentVehicleId { get; set; }
        public virtual TireReading Read(VehicleTireReading ParentReading) { return new TireReading(); }
        public virtual void Move(int Increment) { }
        public virtual void Stop(){ }
        public virtual void AddPressure(int Increment) { }
        public virtual void DecreasePressure(int Increment) { }
        public virtual void Flatten() { }
        public virtual void Slow(int Increment) { }
        public virtual int CalcMaxSpeedRating() { return 0; }
        public virtual int CalcMaxPressure() { return 0; }
        public virtual int CalcDiameter() { return 0; }
        public virtual int CalcMinPressure() { return 0; }
        public virtual int CalcMaxDistanceRating() { return 0; }
        public virtual TimeSpan CalcRelativeTireAge() { return new TimeSpan(); }
    }
}
