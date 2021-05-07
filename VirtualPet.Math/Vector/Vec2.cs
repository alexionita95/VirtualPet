using System;

namespace VirtualPet.Math.Vectors
{
    public class Vec2 : Vec
    {
        public float X { get { return components[0]; } set { components[0] = value; CalculateMagnitudeSquared(); } }
        public float Y { get { return components[1]; } set { components[1] = value; CalculateMagnitudeSquared(); } }
        public Vec2() : base(new float[] { 0, 0 })
        {

        }
        public Vec2(float x, float y) : base(new float[] { x, y })
        {

        }
        public Vec2(Vec2 v) : this(v.X, v.Y)
        {

        }
        public new Vec2 Normalize()
        {
            return ToVec2(base.Normalize());
        }


        public new void Add(float scalar)
        {
            base.Add(scalar);
        }

        public void Add(Vec2 v)
        {
            base.Add(v);
        }

        public new void Sub(float scalar)
        {
            base.Sub(scalar);
        }

        public void Sub(Vec2 v)
        {
            base.Sub(v);
        }

        public new void Mul(float scalar)
        {
            base.Mul(scalar);
        }

        public void Mul(Vec2 v)
        {
            base.Mul(v);
        }

        public new void Div(float scalar)
        {
            base.Div(scalar);
        }

        public void Div(Vec2 v)
        {
            base.Div(v);
        }



        public float Cross(Vec2 vector)
        {
            return X * vector.Y - Y * vector.X;
        }

        public float Dot(Vec2 vector)
        {
            return (X * vector.X + Y * vector.Y);
        }

        public void RotateAround(Vec2 origin, float angleDeg)
        {
            float x = X - origin.X;
            float y = Y - origin.Y;
            float angleRad = Utils.Radians(angleDeg);
            float cos = MathF.Cos(angleRad);
            float sin = MathF.Sign(angleRad);
            X = x * cos - y * sin;
            Y = x * sin + y * cos;
            Add(origin);
        }
        public void Zero()
        {
            X = 0;
            Y = 0;
        }

        public new void Negate()
        {
            base.Negate();
        }
        public static Vec2 Cross(float a, Vec2 v)
        {
            return new Vec2(v.Y * (-a), v.X * a);
        }
        public static Vec2 Cross(Vec2 v, float a)
        {
            return new Vec2(v.Y * a, v.X * (-a));
        }

        public static float DistSqr(Vec2 v1, Vec2 v2)
        {
            float x = v2.X - v1.X;
            float y = v2.Y - v1.Y;
            return (x * x + y * y);
        }

        public static Vec2 Add(Vec2 v, float scalar)
        {
            return ToVec2(Vec.Add(v, scalar));
        }

        public static Vec2 Add(Vec2 v1, Vec2 v2)
        {
            return ToVec2(Vec.Add(v1, v2));
        }

        public static Vec2 Sub(Vec2 v, float scalar)
        {
            return ToVec2(Vec.Sub(v, scalar));
        }

        public static Vec2 Sub(Vec2 v1, Vec2 v2)
        {
            return ToVec2(Vec.Sub(v1, v2));
        }

        public static Vec2 Mul(Vec2 v, float scalar)
        {
            return ToVec2(Vec.Mul(v, scalar));
        }

        public static Vec2 Mul(Vec2 v1, Vec2 v2)
        {
            return ToVec2(Vec.Mul(v1, v2));
        }

        public static Vec2 Div(Vec2 v, float scalar)
        {
            return ToVec2(Vec.Div(v, scalar));
        }

        public static Vec2 Div(Vec2 v1, Vec2 v2)
        {
            return ToVec2(Vec.Div(v1, v2));
        }

        public static Vec2 ToVec2(Vec value)
        {
            return new Vec2(value[0], value[1]);
        }


    }
}
