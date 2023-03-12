using System;

namespace TPMSIoTDemo.Common
{
    public class TireReading
    {
        readonly BaseVehicleTire _currentTire = null;
        readonly VehicleTireReading _parentReading = null;
        
        //Handles the case where the object is being deserialized from a message
        public TireReading()
        {
        }

        public TireReading(VehicleTireReading Reading, BaseVehicleTire Tire)
        {
            _parentReading = Reading;
            _currentTire = Tire;
            TireId = _currentTire.Id;
            CurrentPressure = _currentTire.GetCurrentPressure();
            Position = _currentTire.PositionNumber;
            CurrentSpeed = _currentTire.GetCurrentSpeed();
            CurrrentDistanceTraveled = _currentTire.DistanceTraveledInMiles;
            IsFlat = _currentTire.IsFlat();
            IsOverInflated = _currentTire.IsOverInflated();
            IsUnderInflated = _currentTire.IsUnderInflated();
            RelativeTireAge = _currentTire.CalcRelativeTireAge().TotalDays.ToString();
        }


        public Guid TireId
        {
            get;
            set;
        }

        public int CurrentPressure
        {
            get;
            set;
        }

        public int Position
        {
            get;
            set;
        }

        public int CurrentSpeed
        {
            get;
            set;
        }

        public int CurrrentDistanceTraveled
        {
            get;
            set;
        }
        public bool IsFlat { get; set; }

        public bool IsOverInflated { get; set; }

        public bool IsUnderInflated { get; set; }

        public string RelativeTireAge { get; set; }
    }
}
