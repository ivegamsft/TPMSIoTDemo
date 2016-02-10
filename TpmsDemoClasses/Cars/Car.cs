using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TpmsDemoClasses
{

    public class Car : IVehicle
    {
        string _carType = string.Empty;
        int _maxSpeed = 0;
        int _lastSpeed = 0;
        //int _currentSpeed = 0;

        public Car(string CarMaker, CarType NewCarType)
        {
            initCar(CarMaker, NewCarType);
        }

        void initCar(string CarMaker, CarType NewCarType)
        {
            Id = Guid.NewGuid();
            TypeOfCar = NewCarType;
            FactoryName = CarMaker;
            _maxSpeed = calcMaxSpeed();
            CurrentSpeed = 0;
            _lastSpeed = 0;
            DateTime installDate = DateTime.UtcNow;
            Tires = new List<IVehicleTire>();
            for (int i = 0; i < 4; i++)
            {
                //Create 4 tires
                CarTire newTire = new CarTire(this, i, installDate);
                Tires.Add(newTire);
            }
        }
        public Guid Id
        {
            get; set;
        }

        public CarType TypeOfCar
        {
            get; set;
        }

        [JsonIgnore]
        public List<IVehicleTire> Tires
        {
            get; set;
        }
        public int CurrentSpeed { get; private set; }

        public int OdometerInMiles
        {
            get;
            set;
        }

        public string VehicleType
        {
            get
            {
                return TypeOfCar.ToString();
            }

            set {}
        }

        public string FactoryName
        {
            get;
            set;
        }

        public VehicleState State
        {
            get;
            set;
        }

        int calcMaxSpeed()
        {
            int maxSpeed;
            switch (TypeOfCar)
            {
                case CarType.Sedan:
                    {
                        maxSpeed = 120;
                        break;
                    }
                case CarType.SUV:
                    {
                        maxSpeed = 100;
                        break;

                    }
                case CarType.Coupe:
                    {
                        maxSpeed = 160;
                        break;
                    }
                default:
                    {
                        maxSpeed = 90;
                        break;
                    }
            }
            return maxSpeed;
        }

        public void AddAirToTires(int Position, int Increment)
        {
            if (Position >= 0 || Position < 3)
            {
                IVehicleTire tire = Tires[Position];
                tire.AddPressure(Increment);
            }
        }

        public int CalcMaxSpeed()
        {
            int maxSpeed = 0; //Make this very high. This will reset with the max tire rating
            int maxTireSpeed = 0;
            foreach(CarTire ct in Tires)
            {
                //The max speed is the lowest of all of the tire ratings
                maxTireSpeed = ct.CalcMaxSpeedRating();
                if (maxTireSpeed <= maxSpeed)
                {
                    maxSpeed = maxTireSpeed;
                }
            }
            return maxSpeed;
        }

        public void Move(int Increment)
        {
            //Make the car move and continue moving until it reaches max speed
            int newSpeed = Increment + _lastSpeed;
            CurrentSpeed = newSpeed;
            OdometerInMiles = OdometerInMiles + Increment;

            if (newSpeed > _maxSpeed)
            {
                CurrentSpeed = _maxSpeed;
            }

            foreach (CarTire ct in Tires)
            {
                ct.Move(newSpeed);
            }

            _lastSpeed = CurrentSpeed;
            State = VehicleState.Moving;
        }

        public void ReplaceFlat()
        {
            int replacementTirePos = int.MinValue;
            CarTire newTire = null;
            List<IVehicleTire> newTires = new List<IVehicleTire>();

            foreach (CarTire ct in Tires)
            {
                if (ct.IsFlat())
                {
                    replacementTirePos = ct.PositionNumber;
                    newTire = new CarTire(this, ct.PositionNumber, DateTime.UtcNow);
                    break;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (i == replacementTirePos)
                {
                    newTires.Add(newTire);
                }
                else
                {
                    newTires.Add(Tires[i]);
                }
            }
            Tires = newTires;
        }

        public void RemoveAirFromTires(int Position, int Increment)
        {
            if (Position >= 0 || Position < 3)
            {
                IVehicleTire tire = Tires[Position];
                tire.DecreasePressure(Increment);
            }
        }

        public void Stop()
        {
            foreach (CarTire ct in Tires)
            {
                ct.Stop();
            }
            State = VehicleState.Stopped;
        }

        public void Slow(int Increment)
        {
            //slow the car down
            foreach (CarTire ct in Tires)
            {
                ct.Slow(Increment);
                CurrentSpeed = ct.GetCurrentSpeed();
            }
        }

        public VehicleTireReading ReadTires()
        {
            VehicleTireReading currentReading = new VehicleTireReading(this);
            return currentReading;
        }
    }
}
