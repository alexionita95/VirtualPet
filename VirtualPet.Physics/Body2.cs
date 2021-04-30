using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualPet.Math.Vectors;
using VirtualPet.Math.Geometry;
namespace VirtualPet.Physics
{
    public class Body2
    {

        private Shape2 internalShape;

        public Transform2 Transform { get; set; }
        public Vec2 Position { get => Transform.Position; set => Transform.Position = value; }
        public float Rotation { get => Transform.Rotation; set => Transform.Rotation = value; }
        public Vec2 LinearVelocity { get; set; } = new Vec2();
        public float AngularVelocity { get; set; } = 0;
        public float Torque { get; set; } = 0;
        public bool FixedRotation { get; set; } = false;
        public float Mass { get => MassData.Mass; set => MassData.Mass = value; }
        public float InverseMass { get => MassData.InverseMass; }
        public float Inertia { get => MassData.Inertia; set => MassData.Inertia = value; }
        public float InverseInertia { get => MassData.InverseInertia; }
        public MassData MassData { get; private set; }
        public float StaticFriction { get; set; }
        public float DynamicFriction { get; set; }
        public bool HasInfiniteMass { get { return MassData.Mass.Equals(0.0f); } }
        public bool EnableGravity { get; set; } = true;
        public Shape2 Shape { get=>internalShape; set {
                internalShape = value;
                ComputeMassData();
            } }
        public float Density { get; private set; }

        //coefficient of restitution
        public float CoR { get; set; } = 1.0f;

        private Vec2 forces = new Vec2();

        public Body2(Vec2 position, float rotation = 0, float mass = 0)
        {
            Position = position;
            Rotation = rotation;
            Mass = mass;
        }
        public Body2(Transform2 transform, float density)
        {
            Transform = transform;
            Density = density;
            
        }
        public Body2(Transform2 transform, float density, Shape2 shape)
        {
            Transform = transform;
            Density = density;
            Shape = shape;

        }
        public void ComputeMassData()
        {
            if(Shape is Circle)
            {
                MassData = MassData.GetMassData(Shape as Circle, Density);
                return;
            }
            if(Shape is Poly2)
            {
                MassData = MassData.GetMassData(Shape as Poly2, Density);
                return;
            }
        }
        public void SetTransfrom(Vec2 position, float rotation)
        {
            Position = position;
            Rotation = rotation;
        }
        public void SetTransfrom(Vec2 position)
        {
            Position = position;
        }

        public void ApplyImpulse(Vec2 impulse, Vec2 contactVector)
        {
            LinearVelocity = LinearVelocity.Add(impulse.Mul(InverseMass));
            AngularVelocity += (InverseInertia * contactVector.Cross(impulse));
        }

        public void Update(float dt)
        {
            if (this.Mass == 0.0f) return;


            Position = Position.Add(LinearVelocity.Mul(dt));
            Rotation += AngularVelocity;
            IntegrateForces(dt);
            ClearForces();


        }
        public void ClearForces()
        {
            forces.Zero();
            Torque = 0;
        }
        public void IntegrateForces(float dt)
        {
            if (this.Mass == 0.0f) return;
            Vec2 acceleration = forces.Mul(InverseMass);
            LinearVelocity = LinearVelocity.Add(acceleration.Mul(dt/2.0f));
            AngularVelocity += Torque * InverseInertia * (dt/2.0f);
        }
        public void AddForce(Vec2 force)
        {
            forces = forces.Add(force);
        }

    }
}
