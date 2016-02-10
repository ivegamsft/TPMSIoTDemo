using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TpmsDemoClasses
{
    public class TruckTire : IVehicleTire
    {
        const int MAX_SPEED = 100;
        const int MIN_SPEED = 0;
        int _lastSpeed = 0;
        int _currentSpeed = 0;
        Random randSpeed = new Random(MAX_SPEED);
        Random randPressure = new Random();
        public TruckTire(Truck ParentTruck, int Position)
        {
            Id = Guid.NewGuid();
            CurrentTruck = ParentTruck;
            PositionNumber = Position;
            initTire();
        }

        private void initTire()
        {
            Diameter = CalcDiameter();
            MaxSpeedRating = CalcMaxSpeedRating();
            MinPressure = CalcMinPressure();
            MaxPressure = CalcMaxPressure();
            CurrentPressure = MinPressure; //Assume the tire is at least the minimum pressure
        }

        public Truck CurrentTruck
        {
            get { return CurrentTruck; }
            set
            {
                CurrentVehicle = (IVehicle)value;
                CurrentTruck = (Truck)CurrentVehicle;
            }

        }
        public IVehicle CurrentVehicle
        {
            get { return (IVehicle)CurrentTruck; }
            set { CurrentVehicle = (IVehicle)value ; }
        }
        public int MaxPressure
        {
            get; set;
        }

        public int MinPressure
        {
            get; set;
        }

        public int CurrentPressure
        {
            get; set;
        }

        public int PositionNumber
        {
            get; set;
        }

        public int MaxSpeedRating
        {
            get; set;
        }

        public int Diameter
        {
            get; set;
        }

        public Guid Id
        {
            get; set;
        }

        public int DistanceTraveledInMiles
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime InstallDate
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int MaxDistanceRating
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Guid CurrentVehicleId
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int GetCurrentPressure()
        {
            //Check to see if we are travleling at speed. Then check the pressure
            int curPressure = MinPressure;

            return MinPressure;
        }

        public bool IsUnderInflated()
        {
            return (GetCurrentPressure() < MinPressure);
        }

        public bool IsOverInflated()
        {
            return (GetCurrentPressure() > MaxPressure);
        }

        public bool IsFlat()
        {
            return (GetCurrentPressure() < MinPressure - 10);
        }

        public TireReading Read(VehicleTireReading ParentReading)
        {
            TireReading currentReading = new TireReading(ParentReading, this);
            return currentReading;
        }

        public int GetCurrentSpeed()
        {
            return _currentSpeed;
        }

        public int CalcMaxSpeedRating()
        {
            int maxSpeed;
            switch (CurrentTruck.TypeOfTruck)
            {
                case TruckType.Pickup:
                    {
                        maxSpeed = 100;
                        break;
                    }
                case TruckType.Semi:
                    {
                        maxSpeed = 90;
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

        public int CalcMaxPressure()
        {
            int maxPressure;
            int diam = Diameter;
            switch (CurrentTruck.TypeOfTruck)
            {
                case TruckType.Pickup:
                    {
                        maxPressure = Diameter * 12;
                        break;
                    }
                case TruckType.Semi:
                    {
                        maxPressure = Diameter * 14;
                        break;

                    }
                default:
                    {
                        maxPressure = Diameter * 9;
                        break;
                    }
            }
            return maxPressure;
        }

        public int CalcDiameter()
        {
            int diam;
            switch (CurrentTruck.TypeOfTruck)
            {
                case TruckType.Pickup:
                    {
                        diam = 17;
                        break;
                    }
                case TruckType.Semi:
                    {
                        diam = 22;
                        break;

                    }
                default:
                    {
                        diam = 20;
                        break;
                    }
            }
            return diam;
        }
        public int CalcMinPressure()
        {
            int minPressure;
            switch (CurrentTruck.TypeOfTruck)
            {
                case TruckType.Pickup:
                    {
                        minPressure = Diameter / MaxPressure;
                        break;
                    }
                case TruckType.Semi:
                    {
                        minPressure = Diameter / MaxPressure;
                        break;

                    }
                default:
                    {
                        minPressure = Diameter / MaxPressure;
                        break;
                    }
            }
            return minPressure;
        }

        public void Move(int Increment)
        {
            int newSpeed = Increment + _lastSpeed;

            if (newSpeed > MAX_SPEED)
            {
                _currentSpeed = MAX_SPEED;
            }
            else
            {
                _currentSpeed = newSpeed;
            }
            _lastSpeed = _currentSpeed;
        }

        public void Stop()
        {
            _currentSpeed = 0;
            _lastSpeed = 0;
        }

        public void Slow(int Increment)
        {
            int newSpeed = _lastSpeed - Increment;
            if (newSpeed < 0)
            {
                _currentSpeed = 0;
            }

            if (newSpeed > MAX_SPEED)
            {
                _currentSpeed = MAX_SPEED;
            }
            
            _lastSpeed = _currentSpeed;
        }

        public int CalcMaxDistanceRating()
        {
            throw new NotImplementedException();
        }

        public TimeSpan CalcRelativeTireAge()
        {
            throw new NotImplementedException();
        }

        public void AddPressure(int Increment)
        {
            throw new NotImplementedException();
        }

        public void DecreasePressure(int Increment)
        {
            throw new NotImplementedException();
        }

        public void Flatten()
        {
            throw new NotImplementedException();
        }
    }
}