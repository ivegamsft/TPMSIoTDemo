using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TpmsDemoClasses
{
    public class VehicleTireReading
    {
        //Handles the case where the object is being deserialized from a message
        [JsonConstructor]
        public VehicleTireReading()
        {
            CurrentTires = new List<IVehicleTire>();
        }
        public VehicleTireReading(IVehicle CurrentVehicle)
        {
            Id = Guid.NewGuid();
            CurrentTires = CurrentVehicle.Tires;
            ReadingTimeStamp = DateTime.UtcNow;
            CurrrentDistanceTraveled = CurrentVehicle.OdometerInMiles;
            ReadingId = createReadingHashKey(ReadingTimeStamp, CurrentVehicle.Id);
            VehicleId = CurrentVehicle.Id;
            TypeOfCar = CurrentVehicle.VehicleType;
            Readings = new List<TireReading>();
            int maxSpeed = 0;
            int lastSpeed = 0;

            foreach (CarTire ct in CurrentTires)
            {
                TireReading currentReading = new TireReading(this, ct);
                lastSpeed = ct.GetCurrentSpeed();
                //Capture the last speed
                if (lastSpeed > CurrentSpeed)
                {
                    CurrentSpeed = lastSpeed;
                }
                maxSpeed = ct.MaxSpeedRating;
                Readings.Add(currentReading);
            }
       }

        public List<TireReading> Readings
        {
            get;
            set;
        }

        public string ReadingId
        {
            get;
            set;
        }

        
        public Guid Id
        {
            get;
            set;
        }

        public DateTime ReadingTimeStamp
        {
            get;
            set;
        }

        public Guid VehicleId
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
        public string TypeOfCar { get; set; }
        public bool HasFlat {
            get
            {
                bool hasFlat = false;
                if (CurrentTires != null)
                {
                    foreach (IVehicleTire tire in CurrentTires)
                    {
                        if (tire.IsFlat())
                        {
                            hasFlat = true;
                            break;
                        }
                    }
                }
                return hasFlat;
            }
            set { }
        }

        public List<IVehicleTire> CurrentTires {
            get;
            set;
        }

        private string createReadingHashKey(DateTime TimeStamp, Guid VehicleId)
        {
            // Convert the input string to a byte array and compute the hash.
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(TimeStamp.ToUniversalTime().Ticks, VehicleId)));

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
            }
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

    }
}
