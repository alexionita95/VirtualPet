using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPet.Physics
{
    public class ForceRegistration
    {
        public ForceGenerator Generator { get; set; }
        public Body2 Body { get; set; }

        public ForceRegistration()
        {
            Generator = null;
            Body = null;
        }
        public ForceRegistration(ForceGenerator generator, Body2 body)
        {
            Generator = generator;
            Body = body;
        }
        public void Update(float dt)
        {
            Generator.UpdateForce(Body, dt);
        }
    }
}
