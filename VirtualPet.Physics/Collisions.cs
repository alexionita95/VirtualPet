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
    public class Projection
    {
        public float Min { get; set; }
        public float Max { get; set; }
        public Projection()
        {
            Min = float.MaxValue;
            Max = float.MinValue;
        }
        public Projection(float min, float max)
        {
            Min = min;
            Max = max;
        }
        public bool Overlap(Projection p)
        {
            return Math.Utils.IsBetween(Min, p.Min, p.Max) || Math.Utils.IsBetween(Max, p.Min, p.Max);
        }
        public float GetOverlap(Projection p)
        {
            return MathF.Min(Max, p.Max) - MathF.Max(Min, p.Min);
        }
    }

    public class Edge
    {
        public LineSegment2 Segment { get; set; }
        public Vec2 Vec { get; set; }
    }
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
            if (A.Shape is Poly2 && B.Shape is Poly2)
            {
                PolyToPoly(A, B, result);
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

        public static Projection ProjectOnAxis(Body2 A, Vec2 axis)
        {
            Poly2 polyA = A.Shape as Poly2;
            Mat2 rotA = Mat2.Rotation(A.Rotation);
            float min = axis.Dot(rotA.Mul(polyA.Vertices[0]).Add(A.Position));
            float max = min;
            for (int i = 0; i < polyA.Vertices.Count; ++i)
            {
                float p = axis.Dot(rotA.Mul(polyA.Vertices[i]).Add(A.Position));
                if (p < min)
                {
                    min = p;
                }
                else if (p > max)
                {
                    max = p;
                }
            }
            return new Projection(min, max);
        }

        public static CollisionManifold FindLeastPenetratingAxis(Body2 A, Body2 B)
        {
            Vec2 normal = new Vec2();
            float overlap = float.MaxValue;
            Poly2 polyA = A.Shape as Poly2;
            Poly2 polyB = B.Shape as Poly2;
            Mat2 rotA = Mat2.Rotation(A.Rotation);
            Mat2 rotB = Mat2.Rotation(B.Rotation);
            CollisionManifold result = new CollisionManifold(A, B);
            for (int i = 0; i < polyA.Normals.Count; ++i)
            {
                Vec2 axis = rotA.Mul(polyA.Normals[i]);
                Projection p1 = ProjectOnAxis(A, axis.Normalize());
                Projection p2 = ProjectOnAxis(B, axis.Normalize());
                if (!p1.Overlap(p2))
                {
                    return null;
                }
                float o = p1.GetOverlap(p2);
                if (o < overlap)
                {
                    overlap = o;
                    normal = new Vec2(axis);
                }



            }

            for (int i = 0; i < polyB.Normals.Count; ++i)
            {
                Vec2 axis = rotB.Mul(polyB.Normals[i]);
                Projection p1 = ProjectOnAxis(A, axis.Normalize());
                Projection p2 = ProjectOnAxis(B, axis.Normalize());
                if (!p1.Overlap(p2))
                {
                    return null;
                }
                float o = p1.GetOverlap(p2);
                if (o < overlap)
                {
                    overlap = o;
                    normal = new Vec2(axis);
                }

            }
            result.Depth = overlap;
            result.Normal = normal;
            return result;
        }
        public static Edge FindBestEdge(Body2 body, Vec2 normal)
        {
            Poly2 poly = body.Shape as Poly2;
            Mat2 rot = Mat2.Rotation(body.Rotation);
            Edge result = new Edge();
            float max = float.MinValue;
            int index = 0;
            for (int i = 0; i < poly.Vertices.Count(); ++i)
            {
                float projection = normal.Dot(rot.Mul(poly.Vertices[i]).Add(body.Position));
                if (projection > max)
                {
                    max = projection;
                    index = i;
                }
            }

            int j = index + 1 < poly.Vertices.Count ? index + 1 : 0;
            int k = index - 1 >= 0 ? index - 1 : poly.Vertices.Count - 1;
            Vec2 v = rot.Mul(poly.Vertices[index]);
            Vec2 prev = rot.Mul(poly.Vertices[k]);
            Vec2 next = rot.Mul(poly.Vertices[j]);

            Vec2 l = v.Sub(next).Normalize();
            Vec2 r = v.Sub(prev).Normalize();

            if (r.Add(body.Position).Dot(normal) <= l.Add(body.Position).Dot(normal))
            {
                result.Segment = new LineSegment2(prev, v);
                result.Vec = new Vec2(v);
                return result;
            }
            else
            {
                result.Segment = new LineSegment2(v, next);
                result.Vec = new Vec2(v);
                return result;
            }
        }
        static List<Vec2> Clip(Vec2 v1, Vec2 v2, Vec2 n, float o)
        {
            List<Vec2> result = new List<Vec2>();
            float d1 = n.Dot(v1) - o;
            float d2 = n.Dot(v2) - o;

            if (d1 >= 0)
            {
                result.Add(v1);
            }
            if (d2 >= 0)
            {
                result.Add(v2);
            }
            if (d1 * d2 < 0)
            {
                Vec2 e = v2.Sub(v1);
                float u = d1 / (d2 - d1);
                e.Mul(u);
                e.Add(v1);
                result.Add(e);
            }
            return result;
        }
        public static void PolyToPoly(Body2 a, Body2 b, CollisionManifold result)
        {
            CollisionManifold inResult = FindLeastPenetratingAxis(a, b);
            Mat2 rotA = Mat2.Rotation(a.Rotation);
            Mat2 rotB = Mat2.Rotation(b.Rotation);

            if (inResult == null)
            {
                return;
            }
            result.Normal = inResult.Normal;
            result.Depth = inResult.Depth;
            Edge e1 = FindBestEdge(a, result.Normal);
            Edge e2 = FindBestEdge(b, result.Normal.Negate());
            Edge refE;
            Edge incE;
            Body2 incB;
            Body2 refB;
            bool flip = false;
            if (MathF.Abs(e1.Vec.Add(a.Position).Dot(result.Normal)) <= MathF.Abs(e2.Vec.Add(b.Position).Dot(result.Normal)))
            {
                refE = e1;
                incE = e2;
                refB = a;
                incB = b;
            }
            else
            {
                flip = true;
                refE = e2;
                incE = e1;
                refB = b;
                incB = a;
            }
            Vec2 refV = refE.Vec.Add(refB.Position).Normalize();
            float o1 = refV.Dot(refE.Segment.Start.Add(refB.Position));
            Vec2 v1 = incE.Segment.Start.Add(incB.Position);
            Vec2 v2 = incE.Segment.End.Add(incB.Position);
            List<Vec2> cp = Clip(v1, v2, refV, o1);
            if(cp.Count < 2)
            {
                return;
            }

            float o2 = refV.Dot(refE.Segment.End.Add(refB.Position));
            cp = Clip(cp[0], cp[1], refV.Negate(), -o2);
            if(cp.Count < 2)
            {
                return;
            }

            Vec2 refN = Vec2.Cross(refV, -1.0f);
            if(flip)
            {
                refN.Negate();
            }
            float max = refN.Dot(refE.Vec.Add(refB.Position));

            for(int i=0;i<cp.Count;++i)
            {
                result.AddContactPoint(cp[i]);
            }


        }
        public static void PolyToPolyOld(Body2 a, Body2 b, CollisionManifold result)
        {
            Poly2 A = a.Shape as Poly2;
            Poly2 B = b.Shape as Poly2;

            int faceA;
            float penetrationA = FindAxisLeastPenetration(a, b, out faceA);
            if (penetrationA >= 0)
            {
                return;
            }

            int faceB;
            float penetrationB = FindAxisLeastPenetration(b, a, out faceB);

            if (penetrationB >= 0)
            {
                return;
            }
            System.Diagnostics.Debug.WriteLine($"PenA:{penetrationA}");
            System.Diagnostics.Debug.WriteLine($"PenB:{penetrationB}");
            int refIndex;
            bool flip;


            Body2 RefBody;
            Body2 IncBody;
            Poly2 refPoly;
            Poly2 incPoly;
            if (Math.Utils.BiasGt(penetrationA, penetrationB))
            {
                RefBody = a;
                IncBody = b;
                refIndex = faceA;
                refPoly = A;
                incPoly = B;
                flip = false;
            }
            else
            {
                refPoly = B;
                incPoly = A;
                RefBody = b;
                IncBody = a;
                refIndex = faceB;
                flip = true;
            }

            List<Vec2> incidentFace;

            FindIncidentFace(RefBody, IncBody, refIndex, out incidentFace);

            Vec2 v1 = refPoly.Vertices[refIndex];
            refIndex = refIndex + 1 < refPoly.Vertices.Count ? refIndex + 1 : 0;
            Vec2 v2 = refPoly.Vertices[refIndex];

            Vec2 slidePlanNormal = v2.Sub(v1).Normalize();

            Vec2 refFaceNormal = new Vec2(-slidePlanNormal.Y, slidePlanNormal.X);


            float refC = refFaceNormal.Dot(v1);
            float negSide = -slidePlanNormal.Dot(v1);
            float posSlide = slidePlanNormal.Dot(v2);
            if (Clip(slidePlanNormal.Negate(), negSide, ref incidentFace) < 2)
            {
                return;
            }

            if (Clip(slidePlanNormal, -posSlide, ref incidentFace) < 2)
            {
                return;
            }

            result.Normal = flip ? refFaceNormal.Negate() : new Vec2(refFaceNormal);

            int cp = 0;
            float separation = refFaceNormal.Dot(incidentFace[0]) - refC;

            if (separation <= 0.0f)
            {
                result.AddContactPoint(new Vec2(incidentFace[0]));
                result.Depth = -separation;
            }
            else
            {
                result.Depth = 0;
            }

            separation = refFaceNormal.Dot(incidentFace[1]) - refC;

            if (separation <= 0.0f)
            {
                result.AddContactPoint(new Vec2(incidentFace[1]));
                result.Depth += -separation;
            }
            result.Depth /= result.ContactPoints.Count;


        }

        static float FindAxisLeastPenetration(Body2 a, Body2 b, out int faceIndex)
        {
            Poly2 A = a.Shape as Poly2;
            Poly2 B = b.Shape as Poly2;

            float bestDist = float.MinValue;
            int bestIndex = -1;
            Mat2 rotA = Mat2.Rotation(a.Rotation);
            Mat2 rotB = Mat2.Rotation(b.Rotation);
            Mat2 rotBT = rotB.Transpose();


            for (int i = 0; i < A.Vertices.Count; ++i)
            {
                Vec2 n = A.Normals[i];

                Vec2 nw = rotA.Mul(n);

                n = rotBT.Mul(nw);

                Vec2 s = B.GetSupportPoint(n.Negate());

                Vec2 v = A.Vertices[i];
                v = rotA.Mul(v).Add(a.Position).Sub(b.Position);
                v = rotBT.Mul(v);

                float d = n.Dot(s.Sub(v));

                if (d > bestDist)
                {
                    bestDist = d;
                    bestIndex = i;
                }



            }

            faceIndex = bestIndex;
            return bestDist;
        }

        static void FindIncidentFace(Body2 refBody, Body2 incBody, int refFace, out List<Vec2> result)
        {
            Poly2 refPoly = refBody.Shape as Poly2;
            Poly2 incPoly = incBody.Shape as Poly2;

            Mat2 refRot = Mat2.Rotation(refBody.Rotation);
            Mat2 refRotT = refRot.Transpose();
            Mat2 incRot = Mat2.Rotation(incBody.Rotation);
            Mat2 incRotT = incRot.Transpose();

            Vec2 refNormal = refRot.Mul(refPoly.Normals[refFace]);
            refNormal = incRotT.Mul(refNormal);

            int incidentFace = 0;
            float minDot = float.MaxValue;

            for (int i = 0; i < incPoly.Vertices.Count; ++i)
            {
                float dot = refNormal.Dot(incPoly.Normals[i]);
                if (dot < minDot)
                {
                    minDot = dot;
                    incidentFace = i;
                }
            }

            result = new List<Vec2>();

            result.Add(incRot.Mul(incPoly.Vertices[incidentFace]).Add(incBody.Position));
            incidentFace = incidentFace + 1 < incPoly.Vertices.Count ? incidentFace + 1 : 0;
            result.Add(incRot.Mul(incPoly.Vertices[incidentFace]).Add(incBody.Position));
        }

        static int Clip(Vec2 n, float c, ref List<Vec2> face)
        {
            int sp = 0;
            List<Vec2> result = new List<Vec2>(face.ToArray());

            float d1 = n.Dot(face[0]) - c;
            float d2 = n.Dot(face[1]) - c;

            if (d1 < 0.0f)
            {
                result[sp++] = face[0];
            }
            if (d2 < 0.0f)
            {
                result[sp++] = face[1];
            }
            if (d1 * d2 < 0.0f)
            {
                float alpha = d1 / (d1 - d2);
                result[sp] = face[0].Add(alpha).Mul(face[1].Sub(face[0]));
                ++sp;
            }
            face[0] = result[0];
            face[1] = result[1];
            return sp;

        }


    }
}
