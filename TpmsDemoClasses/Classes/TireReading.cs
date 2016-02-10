using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TpmsDemoClasses
{
    public class TireReading
    {
        IVehicleTire _currentTire = null;
        VehicleTireReading _parentReading = null;
        //Handles the case where the object is being deserialized from a message
        [JsonConstructor]
        public TireReading()
        {
        }
        public TireReading(VehicleTireReading Reading, IVehicleTire Tire)
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
