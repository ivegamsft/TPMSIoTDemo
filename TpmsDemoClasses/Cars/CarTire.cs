using System;

namespace TPMSIoTDemo.Common
{
    public class CarTire : BaseVehicleTire
    {
        const int MAX_SPEED = 180;
        const int MIN_SPEED = 0;
        int _lastSpeed = 0;
        int _currentSpeed = 0;
        int _lastPressure = 0;
        Car _currentCar;
        readonly Random RandomSpeed = new Random(MAX_SPEED);
        readonly Random RandomPressure = new Random();

        public CarTire(Car ParentCar, int Position, DateTime InstallationDate)
        {
            Id = Guid.NewGuid();
            CurrentVehicleId = ParentCar.Id;
            _currentCar = ParentCar;
            PositionNumber = Position;
            InstallDate = InstallationDate;
            InitTire();
        }

        private void InitTire()
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

        public int CurrentPressure
        {
            get; set;
        }

        public TimeSpan RelativeTireAge
        {
            get { return CalcRelativeTireAge(); }
            internal set { }
        }

        public override int GetCurrentPressure()
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

        public override bool IsUnderInflated()
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

        public override bool IsOverInflated()
        {
            return (GetCurrentPressure() > MaxPressure);
        }

        public override  bool IsFlat()
        {
            int currentPressure = GetCurrentPressure();
            bool isFlat = false;
            if (currentPressure <= 0)
            {
                isFlat = true;
            }
            return isFlat;
        }

        public override TireReading Read(VehicleTireReading ParentReading)
        {
            TireReading currentReading = new TireReading(ParentReading, this);
            return currentReading;
        }

        public override int GetCurrentSpeed()
        {
            return _currentSpeed;
        }

        public override int CalcMaxSpeedRating()
        {
            int maxSpeed;
            switch (Enum.Parse(typeof(CarType), _currentCar.VehicleClass))
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


        public override int CalcMaxDistanceRating()
        {
            int maxDistance = Diameter * MaxSpeedRating * 25;
            return maxDistance;
        }

        public override int CalcMaxPressure()
        {
            int maxPressure;
            int diam = Diameter;
            switch (Enum.Parse(typeof(CarType), _currentCar.VehicleClass))
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
        
        public override int CalcDiameter()
        {
            int diam = 13;
            switch (Enum.Parse(typeof(CarType), _currentCar.VehicleClass))
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

        public override int CalcMinPressure()
        {
            int minPressure;
            switch (Enum.Parse(typeof(CarType), _currentCar.VehicleClass))
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

        public override void Move(int CarSpeed)
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
            DistanceTraveledInMiles += newSpeed;
        }

        public override void Stop()
        {
            _currentSpeed = 0;
            _lastSpeed = 0;
        }

        public override void Slow(int Increment)
        {
            int newSpeed = _lastSpeed - Increment;
            _currentSpeed = newSpeed;
            if (newSpeed < 0)
            {
                _currentSpeed = 0;
            }
            _lastSpeed = _currentSpeed;
        }

        public override TimeSpan CalcRelativeTireAge()
        {
            int currentMiles = DistanceTraveledInMiles;
            TimeSpan tireAge = (InstallDate - DateTime.Now);
            TimeSpan currentRelativeAge = new TimeSpan();
            TimeSpan agePenalty = CalcAgePenalty();
                       
            currentRelativeAge = tireAge.Add(agePenalty);
            return currentRelativeAge;
        }

        private TimeSpan CalcAgePenalty()
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

        public override void AddPressure(int Increment)
        {
            CurrentPressure += Increment;
            if (CurrentPressure > (MaxPressure + 20))
            {
                //If the pressure is increased too much, it will cause a flat
                Flatten();
            }
        }

        public override void DecreasePressure(int Increment)
        {
            CurrentPressure -= Increment;
            if (CurrentPressure < 0)
            {
                CurrentPressure = 0;
            }
        }

        public override void Flatten()
        {
            CurrentPressure = 0;
        }
    }
}