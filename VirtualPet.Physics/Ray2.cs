using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualPet.Math.Vectors;
namespace VirtualPet.Physics
{
    public class Ray2
    {
        public Vec2 Origin { get;  private set; }
        public Vec2 Direction { get; private set; }
        Ray2()
        {

        }
        public Ray2(Vec2 origin, Vec2 direction)
        {
            Origin = origin;
            Direction = direction.Normalize();
        }
    }
    public class RaycastResult
    {
        public Vec2 Point { get; set; }
        public Vec2 Normal { get; set; }
        public float T { get; set; }
        public bool Hit { get; set; }

        public RaycastResult()
        {
            Point = new Vec2();
            Normal = new Vec2();
            T = -1;
            Hit = false;
        }
        public RaycastResult(Vec2 point, Vec2 normal, float t, bool hit)
        {
            Point = point;
            Normal = normal;
            T = t;
            Hit = hit;
        }
        public static void Reset(RaycastResult result)
        {
            if(result  != null)
            {
                result.Point=new Vec2();
                result.Normal = new Vec2();
                result.T = -1;
                result.Hit = false;
            }
        }
    }
}
