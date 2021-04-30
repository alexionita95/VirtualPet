using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualPet.Math.Vectors;
namespace VirtualPet.Physics
{
    public class Circle : Collider2
    {
        public float Radius { get; set; }
        public Body2 Body { get; set; }
        public Vec2 Center { get { return CalculateCenter(); } }

        public Circle(float radius = 0)
        {
            Radius = radius;
        }

        public Circle(float radius, Body2 body)
        {
            Radius = radius;
            Body = body;
        }
        Vec2 CalculateCenter()
        {
            return new Vec2(Body.Position.X, Body.Position.Y);
        }
    }
}
