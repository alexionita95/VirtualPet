using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualPet.Math.Vectors;
using VirtualPet.Math.Geometry;
namespace VirtualPet.Physics
{
    public class CollisionManifold
    {
        public Vec2 Normal { get; set; }
        public List<Vec2> ContactPoints { get; set; }
        public float Depth { get; set; }
        public bool IsColliding { get; set; }
        float cor = 0;
        float staticFriction = 0;
        float dynamicFriction = 0;
        public Body2 BodyA { get; private set; }
        public Body2 BodyB { get; private set; }
        public CollisionManifold()
        {
            Normal = new Vec2();
            ContactPoints = new List<Vec2>();
            Depth = 0;
            IsColliding = false;
        }
        public CollisionManifold(Body2 A, Body2 B)
        {
            Normal = new Vec2();
            ContactPoints = new List<Vec2>();
            BodyA = A;
            BodyB = B;
        }
        public CollisionManifold(Vec2 normal, List<Vec2> contactPoints, float depth, bool colliding)
        {
            Normal = normal;
            ContactPoints = contactPoints;
            Depth = depth;
            IsColliding = colliding;
        }
        public void Initialize()
        {
            cor = MathF.Min(BodyA.CoR, BodyB.CoR);
            staticFriction = MathF.Sqrt(BodyA.StaticFriction * BodyB.StaticFriction);
            dynamicFriction = MathF.Sqrt(BodyA.DynamicFriction * BodyB.DynamicFriction);

            System.Diagnostics.Debug.WriteLine( $"CoR: {cor} Depth:{Depth}");
            if(Depth <0.001)
            {
                cor = 0;
            }

        }
        public void Solve()
        {
            Collisions.Collides(BodyA, BodyB, this);
        }
        public void AddContactPoint(Vec2 contact)
        {
            ContactPoints.Add(contact);
        }

        public void ApplyImpulse()
        {
            if (BodyA.Mass + BodyB.Mass == 0.0f)
            {
                InfiniteMassCorrection();
                return;
            }

            for (int i = 0; i < ContactPoints.Count; ++i)
            {
                Vec2 ra = ContactPoints[i].Sub(BodyA.Position);
                Vec2 rb = ContactPoints[i].Sub(BodyB.Position);

                Vec2 rv = BodyB.LinearVelocity.Add(Vec2.Cross(BodyB.AngularVelocity, rb)).Sub(BodyA.LinearVelocity).Sub(Vec2.Cross(BodyA.AngularVelocity, ra));

                float contactVel = rv.Dot(Normal);
                if (contactVel > 0)
                    return;

                Vec2 raCrossN = Vec2.Cross(ra.Cross(Normal), ra);
                Vec2 rbCrossN = Vec2.Cross(rb.Cross(Normal), rb);
                float invMassSum = BodyA.InverseMass + BodyB.InverseMass + (raCrossN.Mul(BodyA.InverseInertia).Add(rbCrossN.Mul(BodyB.InverseInertia)).Dot(Normal));
                //float invMassSum = BodyA.InverseMass + BodyB.InverseMass;

                float j = -(1.0f + cor) * contactVel;
                j /= invMassSum;
                j /= ContactPoints.Count;
                j = MathF.Max(j, 0);
                Vec2 impulse = Normal.Mul(j);
                BodyA.ApplyImpulse(impulse.Negate(), ra);
                BodyB.ApplyImpulse(impulse, rb);

                rv = BodyB.LinearVelocity.Add(Vec2.Cross(BodyB.AngularVelocity, rb)).Sub(BodyA.LinearVelocity).Sub(Vec2.Cross(BodyA.AngularVelocity, ra));

                Vec2 t = rv.Sub(Normal.Mul(rv.Dot(Normal)));
                t.Normalize();

                float jt = -rv.Dot(t);
                jt /= invMassSum;
                jt /= ContactPoints.Count;
                jt = MathF.Max(jt, 0);
                if (jt == 0.0f)
                {
                    return;
                }

                Vec2 tangentImpulse;
                if (MathF.Abs(jt) < j * staticFriction)
                {
                    tangentImpulse = t.Mul(jt);
                }
                else
                {
                    tangentImpulse = t.Mul(-j).Mul(dynamicFriction);
                }

                BodyA.ApplyImpulse(tangentImpulse.Negate(), ra);
                BodyB.ApplyImpulse(tangentImpulse, rb);

            }
        }
        public void PositionalCorrection()
        {
            float slop = 0.05f;
            float percent = 0.4f;
            float factor = MathF.Max(Depth - slop, 0.0f) / (BodyA.InverseMass + BodyB.InverseMass);
            Vec2 correction = Normal.Mul(factor).Mul(percent);
            BodyA.Position.Sub(correction.Mul(BodyA.InverseMass));
            BodyB.Position.Add(correction.Mul(BodyB.InverseMass));
        }
        void InfiniteMassCorrection()
        {
            BodyA.LinearVelocity.Zero();
            BodyB.LinearVelocity.Zero();
        }

    }
}
