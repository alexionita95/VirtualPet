using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualPet.Math.Vectors;
namespace VirtualPet.Physics
{
    public class System2
    {
        public ForceRegistry Registry { get; set; }
        public List<Body2> Bodies { get; set; }
        public float UpdateInterval { get; set; }
        public Gravity2 Gravity { get; private set; }
        public int ImpulseIterations { get; set; } = 10;


        public List<CollisionManifold> collisions;

        public System2()
        {

        }
        public System2(Vec2 gravity, float interval = 1 / 60f)
        {
            Registry = new ForceRegistry();
            Bodies = new List<Body2>();
            UpdateInterval = interval;
            Gravity = new Gravity2(gravity);
            collisions = new List<CollisionManifold>();
        }
        public void Update(float dt)
        {
            FixedUpdate();
        }
        void Clear()
        {
            collisions.Clear();
        }
        public void FixedUpdate()
        {
            Registry.Update(UpdateInterval);
            int size = Bodies.Count;
            Clear();

            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    if (i == j)
                        continue;
                    CollisionManifold result = new CollisionManifold();
                    Body2 b1 = Bodies[i];
                    Body2 b2 = Bodies[j];

                    if (b1.HasInfiniteMass && b2.HasInfiniteMass)
                    {
                        continue;
                    }
                    CollisionManifold m = new CollisionManifold(b1, b2);
                    m.Solve();
                    if (m.ContactPoints.Count != 0)
                    {
                        collisions.Add(m);
                    }
                }
            }
            for (int i = 0; i < Bodies.Count; ++i)
            {
                Bodies[i].IntegrateForces(UpdateInterval);
            }
            //find collisions
            for (int i = 0; i < collisions.Count; ++i)
            {
                collisions[i].Initialize();
            }
            //resolve collisions via iterative 

            for (int k = 0; k < ImpulseIterations; ++k)
            {
                for (int i = 0; i < collisions.Count; ++i)
                {
                    collisions[i].ApplyImpulse();
                }
            }


            foreach (Body2 body in Bodies)
            {
                body.Update(UpdateInterval);
            }
            for (int i = 0; i < collisions.Count; ++i)
            {
                collisions[i].PositionalCorrection();
            }
        }
        public void AddBody(Body2 body, bool addGravity = true)
        {
            Bodies.Add(body);
            if (addGravity)
            {
                Registry.Add(Gravity, body);
            }
        }
        public void ApplyForce(Body2 b, ForceGenerator gen)
        {
            Registry.Add(gen, b);
        }
    }
}
