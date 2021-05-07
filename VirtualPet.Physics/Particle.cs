using System;
using VirtualPet.Math.Vectors;

namespace VirtualPet.Physics
{
    public class Particle
    {
        public Vec2 Position { get; set; }
        public Vec2 Velocity { get; set; }
        public Vec2 Acceleration { get; set; }
        public float Damping { get; set; }
        public float InverseMass { get; set; }
        public float Mass { get => InverseMass == 0 ? float.MaxValue : 1.0f / InverseMass; set { InverseMass = value == 0 ? 0 : 1.0f / value; } }
        private Vec2 forceAcumulator = new Vec2();

        public Particle()
        {
            Position = new Vec2();
            Velocity = new Vec2();
            Acceleration = new Vec2();
            InverseMass = 0;
            Damping = 1;
        }
        public void Clear()
        {
            forceAcumulator.Zero();
        }

        public Particle(Vec2 position, Vec2 velcity, Vec2 acceleration, float invMass = 0, float damping = 1)
        {
            Position = position;
            Velocity = velcity;
            Acceleration = acceleration;
            InverseMass = invMass;
            Damping = damping;
        }

        public void Integrate(float dt)
        {
            Vec2 scaledVel = Vec2.Mul(Velocity, dt);
            Position.Add(scaledVel);
            Vec2 acc = new Vec2(Acceleration);
            Vec2 forceEffect = Vec2.Mul(forceAcumulator, InverseMass);
            acc.Add(forceEffect);
            acc.Mul(dt);
            Velocity.Add(acc);
            Velocity.Mul(MathF.Pow(Damping, dt));
        }
    }
}
