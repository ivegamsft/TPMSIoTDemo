using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMSIoTDemo.Common
{
    public static class CarFactory
    {
        static Random RandomPressure = new Random();
        static Random RandomType = new Random();
        public static List<Car> Cars = new List<Car>();
        public static Car CreateCar(string Factory)
        {

            //Creates a random car
            string[] _carTypes = (typeof(CarType)).GetEnumNames();
            int nextTypeIdx = RandomType.Next(0, _carTypes.Length);
            string CarClass = _carTypes[nextTypeIdx];
            //CarType newCarType = (CarType) Enum.Parse(typeof(CarType), _carType);
            Car newCar = new Car(Factory, CarClass);
            foreach (CarTire ct in newCar.Tires)
            {
                //DO something for each tire if we have to
            }

            //Put the car on the road and start it moving
            newCar.Move(new Random().Next(1, 15));
            Cars.Add(newCar);
            return newCar;
        }
    }
}
