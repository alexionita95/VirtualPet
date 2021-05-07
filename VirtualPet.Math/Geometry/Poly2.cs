using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualPet.Math.Vectors;

namespace VirtualPet.Math.Geometry
{
    public class Poly2 : Shape2
    {
        private List<Vec2> internalVertices;
        public List<Vec2> Vertices { get { return internalVertices; } set { internalVertices = value; Setup(internalVertices.ToArray()); } }
        public List<Vec2> Normals { get; private set; }
        public float I { get; set; }
        public float Area { get; set; }

        public Poly2()
        {
            Vertices = new List<Vec2>();
            Normals = new List<Vec2>();
        }

        public Poly2(Vec2[] vertices)
        {
            Vertices = new List<Vec2>();
            Normals = new List<Vec2>();
            Setup(vertices);
        }


        private void Setup(Vec2[] vertices)
        {
            internalVertices.Clear();
            if (vertices.Length <= 2)
            {
                return;
            }
            Normals.Clear();
            internalVertices.Clear();
            int rightMost = 0;
            float highestX = vertices[0].X;

            for (int i = 0; i < vertices.Length; ++i)
            {
                float x = vertices[0].X;
                if (x > highestX)
                {
                    highestX = x;
                    rightMost = i;
                }
                else if (x == highestX)
                {
                    if (vertices[i].Y < vertices[rightMost].Y)
                    {
                        rightMost = i;
                    }
                }
            }
            int[] hull = new int[vertices.Length];
            int count = 0;
            int indexHull = rightMost;

            for (; ; )
            {
                hull[count] = indexHull;

                int nextHullIndex = 0;

                for (int i = 1; i < count; ++i)
                {
                    if (nextHullIndex == indexHull)
                    {
                        nextHullIndex = i;
                        continue;
                    }


                    Vec2 e1 = vertices[nextHullIndex].Sub(vertices[hull[count]]);
                    Vec2 e2 = vertices[i].Sub(vertices[hull[count]]);
                    float c = e1.Cross(e2);
                    if (c < 0.0f)
                    {
                        nextHullIndex = i;
                    }

                    if (c.Equals(0.0f) && e2.LengthSquared > e1.LengthSquared)
                    {
                        nextHullIndex = i;
                    }
                }
                ++count;
                indexHull = nextHullIndex;

                if (nextHullIndex == rightMost)
                {
                    System.Diagnostics.Debug.WriteLine($"Vertices: {count}");
                    break;
                }
            }

            for (int i = 0; i < count; ++i)
            {
                internalVertices.Add(new Vec2(vertices[hull[i]]));
            }

            for (int i = 0; i < Vertices.Count; ++i)
            {
                int j = i < Vertices.Count ? i + i : 0;
                Vec2 face = Vertices[j].Sub(Vertices[i]);
                if (face.LengthSquared <= 0)
                {
                    return;
                }
                Normals.Add(new Vec2(face.Y, -face.X).Normalize());
            }
            Initialize();
        }
        public void Initialize()
        {
            Vec2 c = new Vec2();
            I = 0;
            Area = 0;
            float k_inv3 = 1.0f / 3.0f;

            for (int i = 0; i < Vertices.Count; ++i)
            {
                Vec2 p1 = new Vec2(Vertices[i]);
                int j = i + 1 < Vertices.Count ? i + 1 : 0;
                Vec2 p2 = new Vec2(Vertices[j]);

                float D = p1.Cross(p2);
                float triangleArea = 0.5f * D;
                Area += triangleArea;

                c = c.Add(p1.Add(p2).Mul(triangleArea * k_inv3));

                float x2 = p1.X * p1.X + p2.X * p1.X + p2.X * p2.X;
                float y2 = p1.Y * p1.Y + p2.Y * p1.Y + p2.Y * p2.Y;
                I += (0.25f * k_inv3 * D) * (x2 + y2);
            }
            c = c.Mul(1.0f / Area);

            for (int i = 0; i < Vertices.Count; ++i)
            {
               // internalVertices[i].Sub(c);
            }
        }
        protected void SetBox(float hw, float hh)
        {
            internalVertices.Clear();
            Normals.Clear();
            internalVertices.Add(new Vec2(-hw, -hh));
            internalVertices.Add(new Vec2(hw, -hh));
            internalVertices.Add(new Vec2(hw, hh));
            internalVertices.Add(new Vec2(-hw, hh));

            Normals.Add(new Vec2(0.0f, -1.0f));
            Normals.Add(new Vec2(1.0f, 0.0f));
            Normals.Add(new Vec2(0.0f, 1.0f));
            Normals.Add(new Vec2(-1.0f, 0.0f));
            Initialize();
        }
        public Vec2 GetSupportPoint(Vec2 dir)
        {
            float best = float.MinValue;
            Vec2 result = new Vec2();

            for (int i = 0; i < Vertices.Count; ++i)
            {
                Vec2 v = Vertices[i];
                float projection = v.Dot(dir);
                if (projection > best)
                {
                    result = new Vec2(v);
                    best = projection;
                }
            }

            return result;
        }

    }
}
