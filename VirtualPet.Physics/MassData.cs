using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualPet.Math.Geometry;
using VirtualPet.Math.Vectors;

namespace VirtualPet.Physics
{
    public class MassData
    {
        private float internalMass;
        private float internalInertia;
        float Density { get; set; }
        public float Mass
        {
            get { return internalMass; }
            set
            {
                internalMass = value;
                if (value != 0.0f)
                {
                    InverseMass = 1.0f / value;
                }
            }
        }
        public float InverseMass { get; private set; } = 0;

        public float Inertia
        {
            get { return internalInertia; }
            set
            {
                internalInertia = value;
                if (value != 0)
                {
                    InverseInertia = 1 / value;
                }
            }
        }

        public float InverseInertia
        {
            get; set;
        } = 0;

        public MassData()
        {
            Mass = 0;
            InverseMass = 0;
            Inertia = 0;
            InverseInertia = 0;
        }

        public MassData(float mass, float inertia)
        {
            Mass = mass;
            Inertia = inertia;
        }

        public static MassData GetMassData(Circle c, float density)
        {
            MassData result = new MassData();
            result.Mass = MathF.PI * c.Radius * c.Radius * density;
            result.Inertia = result.Mass * c.Radius * c.Radius;
            return result;



        }
        public static MassData GetMassData(Poly2 p, float density)
        {
            MassData result = new MassData();
            result.Mass = density * p.Area;
            result.Inertia = density * p.I;
            return result;
        }
    }
}
