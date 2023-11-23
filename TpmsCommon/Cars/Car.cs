using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TPMSIoTDemo.Common
{

    public class Car : BaseVehicle
    {
        readonly List<CarTire> _tires = new List<CarTire>();
        TimeSpan _TimeInMotion = TimeSpan.Zero;
        DateTime LastStartTime; 
        double _maxSpeed = 0;
        double _lastSpeed = 0;
        //int _currentSpeed = 0;

        public Car(string CarMaker, string CarClass)
        {
            InitCar(CarMaker, CarClass);
        }

        private void InitCar(string CarMaker, string CarClass)
        {
            Id = Guid.NewGuid();
            VehicleType = VehicleType.Car;
            VehicleClass = CarClass;
            FactoryName = CarMaker;
            _maxSpeed = CalcMaxSpeedForVehicleType();
            CreationDate = DateTime.UtcNow;
            CurrentSpeed = 0;
            _lastSpeed = 0;
            DateTime installDate = DateTime.UtcNow;
            Tires = _tires.Cast<BaseVehicleTire>().ToList();
            for (int i = 0; i < 4; i++)
            {
                //Create 4 tires
                CarTire newTire = new CarTire(this, i, installDate);
                Tires.Add(newTire);
            }
        }

        public double CurrentSpeed { get; private set; }

        int CalcMaxSpeedForVehicleType()
        {
            int maxSpeed;
            switch (Enum.Parse(typeof(CarType), VehicleClass))
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
                BaseVehicleTire tire = Tires[Position];
                tire.AddPressure(Increment);
            }
        }

        public int CalcMaxTireSpeed()
        {
            int maxTireSpeed = 0; //This will reset with the max tire rating
            
           // var min = (from ct in Tires where ct.CalcMaxSpeedRating() > 0 select ct).Min();

            foreach (CarTire ct in Tires)
            {
                //The max speed is the lowest of all of the tire ratings in case of a flat or bad tire
                maxTireSpeed = ct.CalcMaxSpeedRating();
                if (maxTireSpeed <= _maxSpeed)
                {
                    _maxSpeed = maxTireSpeed;
                }
            }
            return maxTireSpeed;
        }

        public override void Move(double Increment)
        {
            //Make the car move and continue moving until it reaches max speed
            if (State == VehicleState.Stopped || State == VehicleState.Parked)
            {
                LastStartTime = DateTime.UtcNow;
                State = VehicleState.Moving;
            }
            _TimeInMotion  = DateTime.UtcNow - LastStartTime;
            double newSpeed = Increment + _lastSpeed;
            CurrentSpeed = newSpeed;
            
            //Set the max speed here
            if (newSpeed > _maxSpeed)
            {
                CurrentSpeed = _maxSpeed;
            }

            //Distance = speed * time
            //1 mile / second = 3600 miles / hour
            //Calc the current miles per second
            OdometerInMiles += Math.Round(CurrentSpeed/3600 * _TimeInMotion.Seconds, MidpointRounding.AwayFromZero);

            foreach (CarTire ct in Tires.Cast<CarTire>())
            {
                ct.Move(CurrentSpeed);
            }

            _lastSpeed = CurrentSpeed;
        }

        public override void ReplaceFlat()
        {
            int replacementTirePos = int.MinValue;
            CarTire newTire = null;
            List<BaseVehicleTire> newTires = new List<BaseVehicleTire>();

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
                BaseVehicleTire tire = Tires[Position];
                tire.DecreasePressure(Increment);
            }
        }

        public override void Stop()
        {
            foreach (CarTire ct in Tires)
            {
                ct.Stop();
            }
            State = VehicleState.Stopped;
            _TimeInMotion = TimeSpan.Zero;
        }

        public override void Slow(double Increment)
        {
            //slow the car down
            foreach (CarTire ct in Tires)
            {
                ct.Slow(Increment);
                CurrentSpeed = ct.GetCurrentSpeed();
            }
        }

        public override VehicleTireReading ReadTires()
        {
            VehicleTireReading currentReading = new VehicleTireReading(this);
            return currentReading;
        }
    }
}
