using System;

namespace VirtualPet.Math
{
    public static class Utils
    {
        public static float Radians(float degrees)
        {
            return (MathF.PI / 180.0f) * degrees;
        }
        public static float Degrees(float radians)
        {
            return (radians*180.0f)/MathF.PI;
        }
    }
}
