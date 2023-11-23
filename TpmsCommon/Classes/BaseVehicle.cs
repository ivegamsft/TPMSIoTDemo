using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace TPMSIoTDemo.Common
{
    public class BaseVehicle
    {
        public Guid Id { get; set; }
        public virtual List<BaseVehicleTire> Tires { get; set; }
        public double OdometerInMiles { get; set; }
        public virtual void Move(double Increment) { }
        public string FactoryName { get; set; }
        public VehicleType VehicleType { get; set; }
        public string VehicleClass { get; set; }
        public DateTime CreationDate { get; set; }
        public VehicleState State { get; set; }
        public virtual void Stop() { }
        public virtual VehicleTireReading ReadTires() { return new VehicleTireReading(); }
        public virtual void ReplaceFlat() { }
        public virtual void Slow(double Increment) { }
    }
}
