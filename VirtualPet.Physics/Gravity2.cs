using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualPet.Math.Vectors;
namespace VirtualPet.Physics
{
    public class Gravity2 : ForceGenerator
    {
        Vec2 Gravity;
        public Gravity2(Vec2 force)
        {
            Gravity = force;
        }
        public void UpdateForce(Body2 body, float dt)
        {
            body.AddForce(new Vec2(Gravity).Mul(body.Mass));
        }
    }
}
