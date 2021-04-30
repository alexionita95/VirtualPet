using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPet.Math.Matrix
{
    public class Mat2 : Mat
    {
        public Mat2():base(2)
        {
            
        }
        public Mat2(float m00, float m01, float m10, float m11) : base(new float[,] { { m00, m01 }, { m10, m11 } })
        {

        }
        public new Mat2 Abs()
        {
            return ToMat2(base.Abs());
        }
        public new Mat2 Transpose()
        {
            return ToMat2(base.Transpose());
        }
        public new Mat2 Invert()
        {
            return ToMat2(base.Invert());
        }

        public new Mat2 Mul(float scalar)
        {
            return ToMat2(base.Mul(scalar));
        }

        public Mat2 Mul(Mat2 m)
        {
            return ToMat2(base.Mul(m));
        }

        public Mat2 MulLeft(Mat2 m)
        {
            return ToMat2(m.Mul(this));
        }

        public Vectors.Vec2 Mul(Vectors.Vec2 v)
        {
            return Vectors.Vec2.ToVec2(base.Mul(v));
        }

        public static Mat2 Identity()
        {
            
            return ToMat2(Identity(2));
        }

        public static Mat2 ToMat2(Mat m)
        {
            return new Mat2( m[0, 0], m[0, 1], m[1, 0], m[1, 1]);
        }

        public static Mat2 Rotation(float angleDegrees)
        {
            float radians = Utils.Radians(angleDegrees);
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);
            return new Mat2(cos, -sin, sin, cos);
        }
    }
}
