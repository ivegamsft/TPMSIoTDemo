using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TpmsDemoClasses
{
    public enum TruckType
    {
        Pickup,
        Semi
    }
    public class Truck : IVehicle
    {
        string _carType = string.Empty;
        List<IVehicleTire> _tires = new List<IVehicleTire>();

        public Truck(TruckType NewTruckType)
        {
            TypeOfTruck = NewTruckType;
            Guid Id = Guid.NewGuid();
            List<IVehicleTire> Tires = new List<IVehicleTire>();
            for(int i=0;i==3;i++)
            {
                //Create 4 tires
                TruckTire currentTire = new TruckTire(this, i);
                _tires.Add(currentTire);
            }
        }
        public Guid Id
        {
            get; set;
        }

        public TruckType TypeOfTruck
        {
            get; set;
        }

        public List<IVehicleTire> Tires
        {
            get; set;
        }

        public int OdometerInMiles
        {
            get;
            set;
        }

        public string VehicleType
        {
            get;
            set;
        }

        public string FactoryName
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

        public VehicleState State
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

        public async void Move(int Increment)
        {
            //Make the car move and continue moving until it reaches max speed
            await Task.Run(
                async () =>
                {
                    foreach (CarTire ct in Tires)
                    {
                        do
                        {
                            await Task.Delay(1000);
                        }
                        while (ct.GetCurrentSpeed() == ct.MaxSpeedRating);
                        {
                            ct.Move(Increment);
                        };
                    }
                }
            );
        }

        public void Stop()
        {
            foreach (CarTire ct in Tires)
            {
                ct.Stop();
            }
        }

        public async void Slow(int Increment)
        {
            //slow the car down
            await Task.Run(
                async () =>
                {
                    foreach (CarTire ct in Tires)
                    {
                        do
                        {
                            await Task.Delay(1000);
                        }
                        while (ct.GetCurrentSpeed() == 0);
                        {
                                ct.Slow(Increment);
                        };
                    }
                }
            );
        }

        public VehicleTireReading ReadTires()
        {
            throw new NotImplementedException();
        }

        public void ReplaceFlat()
        {
            throw new NotImplementedException();
        }
    }
}
