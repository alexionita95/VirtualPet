using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPet.Physics
{
    public interface ForceGenerator
    {
        void UpdateForce(Body2 body, float dt);
    }
}
