using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TpmsDemoClasses
{
    public interface IVehicle
    {
        Guid Id { get; set; }
        List<IVehicleTire> Tires { get; set; }
        int OdometerInMiles { get; set; }
        void Move(int Increment);
        string FactoryName { get; set; }
        string VehicleType { get; set; }
        VehicleState State { get; set; }
        void Stop();
        VehicleTireReading ReadTires();
        void ReplaceFlat();
        void Slow(int Increment);
    }
}
