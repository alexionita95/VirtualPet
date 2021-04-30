using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualPet.Math.Vectors;
using VirtualPet.Math.Geometry;
using VirtualPet.Math.Matrix;

namespace VirtualPet.Physics
{
    public class Collisions
    {
        static float EPSILON = 0.0001f;

        public static void Collides(Body2 A, Body2 B, CollisionManifold result)
        {
            if (A.Shape is Circle && B.Shape is Circle)
            {
                CircleToCircle(A, B, result);
                return;
            }

            if (A.Shape is Circle && B.Shape is Poly2)
            {
                CircleToPoly(A, B, result);
                return;
            }

            if (A.Shape is Poly2 && B.Shape is Circle)
            {
                PolyToCircle(A, B, result);
                return;
            }
        }

        public static void CircleToCircle(Body2 a, Body2 b, CollisionManifold result)
        {
            Circle A = a.Shape as Circle;
            Circle B = b.Shape as Circle;

            Vec2 normal = b.Position.Sub(a.Position);
            float distSqr = normal.LengthSquared;
            float radius = A.Radius + B.Radius;
            if (distSqr > radius * radius)
            {
                result.IsColliding = false;
                return;
            }
            float distance = MathF.Sqrt(distSqr);
            if (distance == 0.0f)
            {
                result.Depth = A.Radius;
                result.Normal = new Vec2(1, 0);
                result.AddContactPoint(new Vec2(a.Position));
                result.IsColliding = true;
            }
            else
            {
                result.Depth = radius - distance;
                result.Normal = normal.Div(distance);
                result.AddContactPoint(result.Normal.Mul(A.Radius).Add(a.Position));
            }

        }

        public static void CircleToPoly(Body2 a, Body2 b, CollisionManifold result)
        {
            Circle A = a.Shape as Circle;
            Poly2 B = b.Shape as Poly2;


            Vec2 center = a.Position;
            Mat2 rot = Mat2.Rotation(b.Rotation);

            center = rot.Transpose().Mul(center.Sub(b.Position));

            float separation = float.MinValue;
            int faceNormal = 0;

            for (int i = 0; i < B.Vertices.Count; ++i)
            {
                float s = B.Normals[i].Dot(center.Sub(B.Vertices[i]));

                if (s > A.Radius)
                {
                    return;
                }
                if (s > separation)
                {
                    separation = s;
                    faceNormal = i;
                }
            }
            Vec2 v1 = B.Vertices[faceNormal];
            int i2 = faceNormal + 1 < B.Vertices.Count ? faceNormal + 1 : 0;
            Vec2 v2 = B.Vertices[i2];

            if (separation < EPSILON)
            {
                result.Normal = rot.Mul(B.Normals[faceNormal]).Negate();
                result.AddContactPoint(result.Normal.Mul(A.Radius).Add(a.Position));
                result.Depth = A.Radius;
                return;
            }

            float dot1 = center.Sub(v1).Dot(v2.Sub(v1));
            float dot2 = center.Sub(v2).Dot(v1.Sub(v2));
            result.Depth = separation;

            if (dot1 <= 0.0f)
            {
                if (Vec2.DistSqr(center, v1) > A.Radius * A.Radius)
                {
                    return;
                }

                Vec2 n = v1.Sub(center);
                n = rot.Mul(n).Normalize();
                result.Normal = n;
                v1 = rot.Mul(v1).Add(b.Position);
                result.AddContactPoint(v1);
            }
            else if (dot2 <= 0.0f)
            {
                if (Vec2.DistSqr(center, v2) > A.Radius * A.Radius)
                {
                    return;
                }

                Vec2 n = v2.Sub(center);
                n = rot.Mul(n).Normalize();
                result.Normal = n;
                v2 = rot.Mul(v2).Add(b.Position);
                result.AddContactPoint(v2);

            }
            else
            {
                Vec2 n = B.Normals[faceNormal];
                if (center.Sub(v1).Dot(n) > A.Radius)
                {
                    return;
                }

                n = rot.Mul(n);
                result.Normal = n.Negate();

                result.AddContactPoint(result.Normal.Mul(A.Radius).Add(a.Position));
            }
        }

        public static void PolyToCircle(Body2 a, Body2 b, CollisionManifold result)
        {
            CircleToPoly(b, a, result);
            result.Normal = result.Normal.Negate();
        }

    }
}
