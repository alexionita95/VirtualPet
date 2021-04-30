using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPet.Math.Geometry
{
    public class Transform2
    {
        public Vectors.Vec2 Position { get; set; }
        public float Rotation { get; set; }

        public Transform2()
        {
            Position = new Vectors.Vec2();
            Rotation = 0;
        }
        public Transform2(Vectors.Vec2 position, float rotation = 0)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
