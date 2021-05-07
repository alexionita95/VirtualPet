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
            return (radians * 180.0f) / MathF.PI;
        }
        public static float Clamp(float value, float min, float max)
        {
            float result = value;
            if (value <= min)
            {
                value = min;
            }
            else
            {
                if (value >= max)
                {
                    value = max;
                }
            }
            return value;
        }

        public static bool BiasGt(float a, float b)
        {
            const float k_biasRelative = 0.95f;
            const float k_biasAbsolute = 0.01f;
            return a >= b * k_biasRelative + a * k_biasAbsolute;
        }

        public static bool IsBetween(float value, float min, float max)
        {
            return ((value >= min) && (value <= max));
        }
    }
}
