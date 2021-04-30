using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPet.Physics
{
    public class ForceRegistry
    {
        public List<ForceRegistration> Regisry { get; private set; }

        public ForceRegistry()
        {
            Regisry = new List<ForceRegistration>();
        }
        public void Add(ForceGenerator generator, Body2 body)
        {
            Regisry.Add(new ForceRegistration(generator, body));
        }

        public void Remove(ForceGenerator generator, Body2 body)
        {
            ForceRegistration reg= new ForceRegistration(generator, body);
            if (Regisry.Contains(reg))
            {
                Regisry.Remove(reg);
            }
        }
        public void clear()
        {
            Regisry.Clear();
        }
        public void Update(float dt)
        {
            foreach(ForceRegistration reg in Regisry)
            {
                reg.Update(dt);
            }
        }

        public void ZeroForces()
        {
            foreach (ForceRegistration reg in Regisry)
            {
                //TODO:
                //reg.Update(dt);
            }
        }

    }
}
