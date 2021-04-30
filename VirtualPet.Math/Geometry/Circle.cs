using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPet.Math.Geometry
{
    public class Circle : Shape2
    {
        public float Radius { get; set; }

        public Circle(float radius = 0)
        {
            Radius = radius;
        }
    }
}
