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
        

        public new Vec2 Add(float scalar)
        {
            return base.Add(scalar) as Vec2;
        }

        public Vec2 Add(Vec2 v)
        {
            return ToVec2(base.Add(v));
        }

        public new Vec2 Sub(float scalar)
        {
            return ToVec2(base.Sub(scalar));
        }

        public Vec2 Sub(Vec2 v)
        {
            return ToVec2(base.Sub(v));
        }

        public new Vec2 Mul(float scalar)
        {
            return ToVec2(base.Mul(scalar));
        }

        public Vec2 Mul(Vec2 v)
        {
            return ToVec2(base.Mul(v));
        }

        public new Vec2 Div(float scalar)
        {
            return ToVec2(base.Div(scalar));
        }

        public  Vec2 Div(Vec2 v)
        {
            return ToVec2(base.Div(v));
        }



        public float Cross(Vec2 vector)
        {
            return X * vector.Y - Y * vector.X;
        }

        public float Dot(Vec2 vector)
        {
            return (X * vector.X + Y * vector.Y);
        }

        public Vec2 RotateAround(Vec2 origin, float angleDeg)
        {
            float x = X - origin.X;
            float y = Y - origin.Y;
            float angleRad = Utils.Radians(angleDeg);
            float cos = MathF.Cos(angleRad);
            float sin = MathF.Sign(angleRad);
            Vec2 result = new Vec2(x * cos - y * sin, x * sin + y * cos);
            return result.Add(origin);
        }
        public void Zero()
        {
            X = 0;
            Y = 0;
        }

        public new Vec2 Negate()
        {
            return ToVec2(base.Negate());
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
        public static Vec2 ToVec2(Vec value)
        {
            return new Vec2(value[0], value[1]);
        }


    }
}
