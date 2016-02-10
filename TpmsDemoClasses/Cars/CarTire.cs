using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TpmsDemoClasses
{
    public class CarTire : IVehicleTire
    {
        const int MAX_SPEED = 180;
        const int MIN_SPEED = 0;
        int _lastSpeed = 0;
        int _currentSpeed = 0;
        int _lastPressure = 0;
        Car _currentCar;
        Random randSpeed = new Random(MAX_SPEED);
        Random randPressure = new Random();
        public CarTire(Car ParentCar, int Position, DateTime InstallationDate)
        {
            Id = Guid.NewGuid();
            CurrentVehicleId = ParentCar.Id;
            _currentCar = ParentCar;
            PositionNumber = Position;
            InstallDate = InstallationDate;
            initTire();
        }

        private void initTire()
        {
            Diameter = CalcDiameter();
            MaxPressure = CalcMaxPressure();
            MinPressure = CalcMinPressure();
            MaxSpeedRating = CalcMaxSpeedRating();
            MaxDistanceRating = CalcMaxDistanceRating();
            CurrentPressure = MinPressure; //Assume the tire is at least the minimum pressure
            _lastPressure = CurrentPressure;
            DistanceTraveledInMiles = 0;
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
            get;
            set;
        }

        public DateTime InstallDate
        {
            get;
            set;
        }

        public int MaxDistanceRating
        {
            get;
            set;
        }

        public TimeSpan RelativeTireAge
        {
            get { return CalcRelativeTireAge(); }
            internal set { }
        }

        public Guid CurrentVehicleId
        {
            get;
            set;
        }

        public int GetCurrentPressure()
        {
            //Check to see if we are travleling at speed. Then check the pressure
            int currentPressure = _lastPressure;
            int currentSpeed = GetCurrentSpeed();
            int currentMiles = DistanceTraveledInMiles;

            if (currentSpeed == 0)
            {
                //We are stopped, just return the pressure reading
                currentPressure = CurrentPressure;
            }
            else
            {
                if (DistanceTraveledInMiles > MaxDistanceRating)
                {
                    //Pressure varies as more wear on the tire
                    currentPressure = CurrentPressure - 5;
                }
                else
                {
                    //set the pressure to last one. We only adjust the pressure for external events
                    currentPressure = _lastPressure;
                }
            }

            _lastPressure = currentPressure;

            return currentPressure;
        }

        public bool IsUnderInflated()
        {
            int currentPressure = GetCurrentPressure();
            int minPressure = MinPressure;
            bool isUnder = false;
            if (currentPressure < 0 || currentPressure < MinPressure)
            {
                isUnder = true;
            }
            return isUnder;
        }

        public bool IsOverInflated()
        {
            return (GetCurrentPressure() > MaxPressure);
        }

        public bool IsFlat()
        {
            int currentPressure = GetCurrentPressure();
            bool isFlat = false;
            if (currentPressure <= 0)
            {
                isFlat = true;
            }
            return isFlat;
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
            switch (_currentCar.TypeOfCar)
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


        public int CalcMaxDistanceRating()
        {
            int maxDistance = Diameter * MaxSpeedRating * 25;
            return maxDistance;
        }

        public int CalcMaxPressure()
        {
            int maxPressure;
            int diam = Diameter;
            switch (_currentCar.TypeOfCar)
            {
                case CarType.Sedan:
                    {
                        maxPressure = Diameter * 2;
                        break;
                    }
                case CarType.SUV:
                    {
                        maxPressure = Diameter * 3;
                        break;

                    }
                case CarType.Coupe:
                    {
                        maxPressure = Diameter * 2;
                        break;
                    }
                default:
                    {
                        maxPressure = Diameter * 2;
                        break;
                    }
            }
            return maxPressure;
        }
        public int CalcDiameter()
        {
            int diam = 13;
            switch (_currentCar.TypeOfCar)
            {
                case CarType.Sedan:
                    {
                        diam = 14;
                        break;
                    }
                case CarType.SUV:
                    {
                        diam = 16;
                        break;

                    }
                case CarType.Coupe:
                    {
                        diam = 15;
                        break;
                    }
                default:
                    {
                        diam = 13;
                        break;
                    }
            }
            return diam;
        }
        public int CalcMinPressure()
        {
            int minPressure;
            switch (_currentCar.TypeOfCar)
            {
                case CarType.Sedan:
                    {
                        minPressure = (Diameter * 2) - 7;
                        break;
                    }
                case CarType.SUV:
                    {
                        minPressure = (Diameter * 3) - 10;
                        break;

                    }
                case CarType.Coupe:
                    {
                        minPressure = (Diameter * 2) -12;
                        break;
                    }
                default:
                    {
                        minPressure = (Diameter * 2) - 5;
                        break;
                    }
            }
            return minPressure;
        }

        public void Move(int CarSpeed)
        {
            int newSpeed = CarSpeed;

            //If we have a flat tire. we cannot go very fast
            if (IsFlat())
            {
                newSpeed = 0;
            }

            if (newSpeed > MAX_SPEED)
            {
                _currentSpeed = MAX_SPEED;
                _lastSpeed = _currentSpeed;
            }
            else
            {
                _currentSpeed = newSpeed;
                _lastSpeed = newSpeed;
            }
            //Update the miles traveled
            DistanceTraveledInMiles = DistanceTraveledInMiles + CarSpeed;
        }

        public void Stop()
        {
            _currentSpeed = 0;
            _lastSpeed = 0;
        }

        public void Slow(int Increment)
        {
            int newSpeed = _lastSpeed - Increment;
            _currentSpeed = newSpeed;
            if (newSpeed < 0)
            {
                _currentSpeed = 0;
            }
            _lastSpeed = _currentSpeed;
        }

        public TimeSpan CalcRelativeTireAge()
        {
            int currentMiles = DistanceTraveledInMiles;
            TimeSpan tireAge = (InstallDate - DateTime.Now);
            TimeSpan currentRelativeAge = new TimeSpan();
            TimeSpan agePenalty = calcAgePenalty();
                       
            currentRelativeAge = tireAge.Add(agePenalty);
            return currentRelativeAge;
        }

        private TimeSpan calcAgePenalty()
        {
            TimeSpan agePenalty = new TimeSpan();
            int currentMiles = DistanceTraveledInMiles;
            int penaltyMiles = DistanceTraveledInMiles - MaxDistanceRating;

            if (penaltyMiles > 0 && DistanceTraveledInMiles < MaxDistanceRating)
            {
                //No penalty. Tires are good
                agePenalty = new TimeSpan(0, 0, 0, 0);
            }

            if (penaltyMiles < 2001 && penaltyMiles > 4999)
            {
                agePenalty = new TimeSpan(50, 0, 0, 0);
            }

            if (penaltyMiles > 5000)
            {
                agePenalty = new TimeSpan(365, 0, 0, 0);
            }

            return agePenalty;
        }

        public void AddPressure(int Increment)
        {
            CurrentPressure = CurrentPressure + Increment;
            if (CurrentPressure > (MaxPressure + 20))
            {
                //If the pressure is increased too much, it will cause a flat
                Flatten();
            }
        }

        public void DecreasePressure(int Increment)
        {
            CurrentPressure = CurrentPressure - Increment;
            if (CurrentPressure < 0)
            {
                CurrentPressure = 0;
            }
        }

        public void Flatten()
        {
            CurrentPressure = 0;
        }
    }
}