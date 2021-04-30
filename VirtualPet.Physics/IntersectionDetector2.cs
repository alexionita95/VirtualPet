using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualPet.Math.Vectors;
using VirtualPet.Math.Geometry;
namespace VirtualPet.Physics
{
    public class IntersectionDetector2
    {
        /*
        private static bool IsBetween(float value, float min, float max)
        {
            if (max < min)
            {
                float temp = min;
                min = max;
                max = temp;
            }
            return (value <= max && min <= value);
        }
        public static bool IsPointOnLine(Vec2 point, Line2 line)
        {
            if (!IsBetween(point.X, line.Start.X, line.End.X) || !IsBetween(point.Y, line.Start.Y, line.End.Y))
            {
                return false;
            }
            if (line.IsVertical())
            {
                return point.X.Equals(line.Start.X);
            }
            if (line.IsHorizontal())
            {
                return point.Y.Equals(line.Start.Y);
            }
            return point.Y.Equals(line.Slope * point.X + line.YIntercept);
        }

        public static bool IsPointInCircle(Vec2 point, Circle circle)
        {
            Vec2 centerToPoint = point.Sub(circle.Center);
            return centerToPoint.LengthSquared <= MathF.Pow(circle.Radius, 2);
        }

        public static bool IsPointInAABB(Vec2 point, AABB box)
        {
            Vec2 min = box.Min;
            Vec2 max = box.Max;
            return (IsBetween(point.X, min.X, max.X) && IsBetween(point.Y, min.Y, max.Y));

        }

        public static bool IsPointInBox(Vec2 point, Box2 box)
        {
            Vec2 min = box.Min;
            Vec2 max = box.Max;
            Vec2 localPoint = point.RotateAround(box.Center, box.Body.Rotation);
            return (IsBetween(localPoint.X, min.X, max.X) && IsBetween(localPoint.Y, min.Y, max.Y));

        }

        public static bool IsLineInCircle(Line2 line, Circle circle)
        {
            if (IsPointInCircle(line.Start, circle) || IsPointInCircle(line.End, circle))
            {
                return true;
            }

            Vec2 ab = line.End.Sub(line.Start);

            Vec2 circleCenter = circle.Center;
            Vec2 centerToStart = circleCenter.Sub(line.Start);
            float t = centerToStart.Dot(ab) / ab.Dot(ab);
            if (!IsBetween(t, 0.0f, 1.0f))
            {
                return false;
            }

            Vec2 closestPoint = line.Start.Add(ab.Mul(t));
            return IsPointInCircle(closestPoint, circle);

        }

        public static bool LineIntersectsAABB(Line2 line, AABB box)
        {
            if (IsPointInAABB(line.Start, box) || IsPointInAABB(line.End, box))
            {
                return true;
            }
            Vec2 unit = line.End.Sub(line.Start).Normalize();
            unit.X = (unit.X != 0) ? 1.0f / unit.X : 0f;
            unit.Y = (unit.Y != 0) ? 1.0f / unit.Y : 0f;
            Vec2 min = box.Min.Sub(line.Start).Mul(unit);
            Vec2 max = box.Max.Sub(line.Start).Mul(unit);

            float tmin = MathF.Max(MathF.Min(min.X, max.X), MathF.Min(min.Y, max.Y));
            float tmax = MathF.Min(MathF.Max(min.X, max.X), MathF.Max(min.Y, max.Y));
            if (tmax < 0 || tmin > tmax)
            {
                return false;
            }
            float t = tmin < 0f ? tmax : tmin;
            return t > 0 && MathF.Pow(t, 2) < line.LengthSquared;
        }

        public static bool LineIntersectsAABB(Line2 line, Box2 box)
        {
            float rotation = -box.Body.Rotation;
            Vec2 center = box.Center;
            Vec2 localStart = line.Start.RotateAround(center, rotation);
            Vec2 localEnd = line.End.RotateAround(center, rotation);
            Line2 localLine = new Line2(localStart, localEnd);
            AABB aabb = new AABB(box.Min, box.Max);
            aabb.Body = box.Body;
            return LineIntersectsAABB(localLine, aabb);
        }


        public static RaycastResult Raycast(Circle circle, Ray2 ray)
        {
            RaycastResult result = new RaycastResult();

            Vec2 originToCircle = circle.Center.Sub(ray.Origin);
            float radiusSq = circle.Radius * circle.Radius;
            float originToCenterLengthSq = originToCircle.LengthSquared;

            float a = originToCircle.Dot(ray.Direction);
            float bSq = originToCenterLengthSq - (a * a);
            if (radiusSq - bSq < 0.0f)
            {
                return result;
            }
            float f = MathF.Sqrt(radiusSq - bSq);
            float t = 0;
            if (originToCenterLengthSq < radiusSq)
            {
                t = a + f;
            }
            else
            {
                t = a - f;
            }

            result.Point = ray.Direction.Add(ray.Direction.Mul(t));
            result.Normal = result.Point.Sub(circle.Center).Normalize();
            result.Hit = true;
            result.T = t;
            return result;
        }

        public static RaycastResult raycast(AABB box, Ray2 ray)
        {
            RaycastResult result = new RaycastResult();
            Vec2 unit = ray.Direction;
            unit.X = (unit.X != 0) ? 1.0f / unit.X : 0f;
            unit.Y = (unit.Y != 0) ? 1.0f / unit.Y : 0f;
            Vec2 min = box.Min.Sub(ray.Origin).Mul(unit);
            Vec2 max = box.Max.Sub(ray.Origin).Mul(unit);

            float tmin = MathF.Max(MathF.Min(min.X, max.X), MathF.Min(min.Y, max.Y));
            float tmax = MathF.Min(MathF.Max(min.X, max.X), MathF.Max(min.Y, max.Y));
            if (tmax < 0 || tmin > tmax)
            {
                return result;
            }
            float t = tmin < 0f ? tmax : tmin;
            bool hit = t > 0f;
            if (!hit)
            {
                return result;
            }
            result.Point = ray.Origin.Add(ray.Direction.Mul(t));
            result.Normal = ray.Origin.Sub(result.Point).Normalize();
            result.Hit = hit;
            result.T = t;
            return result;
        }

        public static RaycastResult raycast(Box2 box, Ray2 ray)
        {
            RaycastResult result = new RaycastResult();
            Vec2 size = box.Size.Mul(.5f);
            Vec2 xAxis = new Vec2(1, 0).RotateAround(new Vec2(0, 0), -box.Body.Rotation);
            Vec2 yAxis = new Vec2(0, 1).RotateAround(new Vec2(0, 0), -box.Body.Rotation);
            Vec2 p = box.Body.Position.Sub(ray.Origin);
            Vec2 f = new Vec2(xAxis.Dot(ray.Direction), yAxis.Dot(ray.Direction));

            Vec2 e = new Vec2(xAxis.Dot(p), yAxis.Dot(p));
            float[] t = { 0, 0, 0, 0 };
            for (int i = 0; i < 2; ++i)
            {
                if (f[i].Equals(0))
                {
                    if (-e[i] - size[i] > 0 || -e[i] + size[i] < 0)
                    {
                        return result;
                    }
                    f[i] = 0.0001f; ;
                }
                t[i * 2 + 0] = (e[i] + size[i]) / f[i];

                t[i * 2 + 1] = (e[i] - size[i]) / f[i];
            }
            float tmin = MathF.Max(MathF.Min(t[0], t[1]), MathF.Min(t[2], t[3]));
            float tmax = MathF.Min(MathF.Max(t[0], t[1]), MathF.Max(t[2], t[3]));

            float trueT = tmin < 0f ? tmax : tmin;
            bool hit = trueT > 0f;
            if (!hit)
            {
                return result;
            }
            result.Point = ray.Origin.Add(ray.Direction.Mul(trueT));
            result.Normal = ray.Origin.Sub(result.Point).Normalize();
            result.Hit = hit;
            result.T = trueT;
            return result;
        }

        public static bool Intersect(Circle c1, Circle c2)
        {
            Vec2 vector = c1.Center.Sub(c1.Center);
            float rSq = MathF.Pow(c1.Radius + c2.Radius, 2);
            return vector.LengthSquared <= rSq;
        }

        public static bool Intersect(Circle circle, AABB box)
        {
            Vec2 min = box.Min;
            Vec2 max = box.Max;

            Vec2 closest = circle.Center;
            if (closest.X < min.X)
            {
                closest.X = min.X;
            }
            else if (closest.X > max.X)
            {
                closest.X = max.X;
            }

            if (closest.Y < min.Y)
            {
                closest.Y = min.Y;
            }
            else if (closest.Y > max.Y)
            {
                closest.Y = max.Y;
            }

            Vec2 circleToBox = circle.Center.Sub(closest);
            return circleToBox.LengthSquared <= circle.Radius * circle.Radius;
        }

        public static bool Intersect(Circle circle, Box2 box)
        {
            Vec2 min = new Vec2();
            Vec2 max = box.Size.Mul(.5f).Mul(2);
            Vec2 r = circle.Center.Sub(box.Body.Position).RotateAround(new Vec2(0, 0), -box.Body.Rotation);
            Vec2 localCirclePos = r.Add(box.Size.Mul(.5f));

            Vec2 closest = localCirclePos;
            if (closest.X < min.X)
            {
                closest.X = min.X;
            }
            else if (closest.X > max.X)
            {
                closest.X = max.X;
            }

            if (closest.Y < min.Y)
            {
                closest.Y = min.Y;
            }
            else if (closest.Y > max.Y)
            {
                closest.Y = max.Y;
            }
            Vec2 circleToBox = localCirclePos.Sub(closest);
            return circleToBox.LengthSquared <= circle.Radius * circle.Radius;

        }

        public static bool Intersect(AABB box1, AABB box2)
        {
            Vec2[] axis = { new Vec2(0, 1), new Vec2(1, 0) };
            for (int i = 0; i < axis.Length; ++i)
            {
                if (!OverlapOnAxis(box1, box2, axis[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool Intersect(AABB box1, Box2 box2)
        {
            Vec2[] axis = { new Vec2(0, 1), new Vec2(1, 0),
                new Vec2(0,1).RotateAround(new Vec2(0,0),box2.Body.Rotation), new Vec2(1, 0).RotateAround(new Vec2(0,0), box2.Body.Rotation) };
            for (int i = 0; i < axis.Length; ++i)
            {
                if (!OverlapOnAxis(box1, box2, axis[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool Intersect(Box2 box1, Box2 box2)
        {
            Vec2[] axis = {  new Vec2(0,1).RotateAround(new Vec2(0,0),box1.Body.Rotation), new Vec2(1, 0).RotateAround(new Vec2(0,0), box1.Body.Rotation),
                new Vec2(0,1).RotateAround(new Vec2(0,0),box2.Body.Rotation), new Vec2(1, 0).RotateAround(new Vec2(0,0), box2.Body.Rotation) };
            for (int i = 0; i < axis.Length; ++i)
            {
                if (!OverlapOnAxis(box1, box2, axis[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool OverlapOnAxis(AABB b1, AABB b2, Vec2 axis)
        {
            Vec2 interval1 = GetInterval(b1, axis);
            Vec2 interval2 = GetInterval(b2, axis);
            return ((interval2.X <= interval2.Y) && (interval1.X <= interval2.Y));
        }


        private static Vec2 GetInterval(AABB box, Vec2 axis)
        {
            Vec2 result = new Vec2(0, 0);
            Vec2 min = box.Min;
            Vec2 max = box.Max;

            Vec2[] vertices = {
            new Vec2(min.X,min.Y),
            new Vec2(min.X,max.Y),
            new Vec2(max.X,min.Y),
            new Vec2(max.X,max.Y),
            };
            result.X = axis.Dot(vertices[0]);
            result.Y = result.X;
            for (int i = 0; i < vertices.Length; ++i)
            {
                float projection = axis.Dot(vertices[i]);
                if (projection < result.X)
                {
                    result.X = projection;
                }
                if (projection > result.Y)
                {
                    result.Y = projection;
                }
            }
            return result;
        }
        private static bool OverlapOnAxis(AABB b1, Box2 b2, Vec2 axis)
        {
            Vec2 interval1 = GetInterval(b1, axis);
            Vec2 interval2 = GetInterval(b2, axis);
            return ((interval2.X <= interval2.Y) && (interval1.X <= interval2.Y));
        }
        private static bool OverlapOnAxis(Box2 b1, Box2 b2, Vec2 axis)
        {
            Vec2 interval1 = GetInterval(b1, axis);
            Vec2 interval2 = GetInterval(b2, axis);
            return ((interval2.X <= interval2.Y) && (interval1.X <= interval2.Y));
        }
        private static Vec2 GetInterval(Box2 box, Vec2 axis)
        {
            Vec2 result = new Vec2(0, 0);

            Vec2[] vertices = box.Vertices;
            result.X = axis.Dot(vertices[0]);
            result.Y = result.X;
            for (int i = 0; i < vertices.Length; ++i)
            {
                float projection = axis.Dot(vertices[i]);
                if (projection < result.X)
                {
                    result.X = projection;
                }
                if (projection > result.Y)
                {
                    result.Y = projection;
                }
            }
            return result;
        }*/
    }
}
