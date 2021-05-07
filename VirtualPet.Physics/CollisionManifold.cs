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
        public List<float> Impulses { get; set; }
        public List<float> TangentImpulses { get; set; }
        public float Depth { get; set; }
        public bool IsColliding { get; set; }
        float cor = 0;
        float staticFriction = 0;
        float dynamicFriction = 0;
        public Body2 BodyA { get; private set; }
        public Body2 BodyB { get; private set; }
        public bool Accumulate { get; set; } = true;
        public CollisionManifold()
        {
            Normal = new Vec2();
            ContactPoints = new List<Vec2>();
            Impulses = new List<float>();
            TangentImpulses = new List<float>();
            Depth = 0;
            IsColliding = false;
        }
        public CollisionManifold(Body2 A, Body2 B)
        {
            Normal = new Vec2();
            ContactPoints = new List<Vec2>();
            Impulses = new List<float>();
            TangentImpulses = new List<float>();
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

        }
        public void Solve()
        {
            Collisions.Collides(BodyA, BodyB, this);
        }
        public void AddContactPoint(Vec2 contact)
        {
            ContactPoints.Add(contact);
            Impulses.Add(0);
            TangentImpulses.Add(0);
        }

        public void ApplyImpulseOld()
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


        public void ApplyImpulse()
        {
            if (BodyA.Mass + BodyB.Mass == 0.0f)
            {
                InfiniteMassCorrection();
                return;
            }

            float allowedPenetration = 0.01f;
            float biasFactor = 0.2f;

            for (int i = 0; i < ContactPoints.Count; ++i)
            {
                Vec2 ra = ContactPoints[i].Sub(BodyA.Position);
                Vec2 rb = ContactPoints[i].Sub(BodyB.Position);

                float rna = ra.Dot(Normal);
                float rnb = rb.Dot(Normal);

                float kNormal = BodyA.InverseMass + BodyB.InverseMass + (BodyA.InverseInertia * (ra.Dot(ra) - rna * rna) + BodyB.InverseInertia * (rb.Dot(rb) - rnb * rnb));

                float massNormal = 1.0f / kNormal;

                Vec2 tangent = Vec2.Cross(Normal, 1.0f);

                float rta = ra.Dot(tangent);
                float rtb = rb.Dot(tangent);

                float kTangent = BodyA.InverseMass + BodyB.InverseMass + (BodyA.InverseInertia * (ra.Dot(ra) - rta * rta) + BodyB.InverseInertia * (rb.Dot(rb) - rtb * rtb));

                float massTangent = 1 / kTangent;

                float bias = -biasFactor * MathF.Min(0.0f, Depth + allowedPenetration);

                if(Accumulate)
                {
                    Vec2 P = Normal.Mul(Impulses[i]).Add(tangent.Mul(TangentImpulses[i]));
                    BodyA.ApplyImpulse(P.Negate(), ra);
                    BodyB.ApplyImpulse(P, rb);
                }

                Vec2 dv = BodyB.LinearVelocity.Sub(Vec2.Cross(BodyB.AngularVelocity, rb)).Sub(BodyA.LinearVelocity).Sub(Vec2.Cross(BodyA.AngularVelocity, ra));

                float vn = dv.Dot(Normal);

                float dPn = massNormal * ((-vn + bias));//*(1+cor));
                if (Accumulate)
                {
                    float Pn0 = Impulses[i];
                    Impulses[i] = MathF.Max(Pn0 + dPn, 0.0f);
                    dPn = Impulses[i] - Pn0;
                }
                else
                {
                    dPn = MathF.Max(dPn, 0.0f);
                }

                Vec2 Pn = Normal.Mul(dPn);
                System.Diagnostics.Debug.WriteLine($"Impulse: {dPn}");
                BodyA.ApplyImpulse(Pn.Negate(), ra);
                BodyB.ApplyImpulse(Pn, rb);

                dv = BodyB.LinearVelocity.Sub(Vec2.Cross(BodyB.AngularVelocity, rb)).Sub(BodyA.LinearVelocity).Sub(Vec2.Cross(BodyA.AngularVelocity, ra));

                float vt = dv.Dot(tangent);

                float dPt = massTangent * (-vt);

                if (Accumulate)
                {
                    float maxPt = staticFriction * Impulses[i];

                    float oldTangent = TangentImpulses[i];
                    TangentImpulses[i] = Math.Utils.Clamp(oldTangent + dPt, -maxPt, maxPt);
                    dPt = TangentImpulses[i] - oldTangent;
                }
                else
                {
                    float maxPt = staticFriction * dPn;
                    dPt = Math.Utils.Clamp(dPt, -maxPt, maxPt);
                }

                Vec2 Pt = tangent.Mul(dPt);
                if(dPt==0)
                { return; }
                BodyA.ApplyImpulse(Pt.Negate(), ra);
                BodyB.ApplyImpulse(Pt, rb);

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
