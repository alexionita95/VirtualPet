using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPet.Physics
{
    public class WindTest : ForceGenerator
    {
        public void UpdateForce(Body2 body, float dt)
        {
            body.AddForce(new Math.Vectors.Vec2(2, 0).Mul(body.Mass));
        }
    }
}
