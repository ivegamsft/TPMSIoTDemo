using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TpmsDemoClasses
{
    public static class CarFactory
    {
        static Random randPressure = new Random();
        static Random randType = new Random();
        public static List<Car> Cars = new List<Car>();
        public static Car CreateCar(string Factory)
        {

            //Creates a random car
            string[] _carTypes = (typeof(CarType)).GetEnumNames();
            int nextTypeIdx = randType.Next(0, _carTypes.Length);
            string _carType = _carTypes[nextTypeIdx];
            CarType newCarType = (CarType) Enum.Parse(typeof(CarType), _carType);
            Car newCar = new Car(Factory, newCarType);
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
