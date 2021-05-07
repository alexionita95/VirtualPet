using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPet.Math.Vectors
{
    public class Vec
    {
        protected float[] components;
        protected float internalMagnitudeSquared;
        public float internalMagnitude;
        bool changed = false;
        public float Length { get { CalculateMagnitude(); return internalMagnitude; } }
        public float LengthSquared { get { return internalMagnitudeSquared; } }
        public int Count { get { return components.Length; } }
        public Vec()
        {

        }
        public Vec(float[] comp)
        {
            components = new float[comp.Length];
            comp.CopyTo(components, 0);
            CalculateMagnitudeSquared();
        }
        public float this[int index]
        {
            get { return components[index]; }
            set { components[index] = value; CalculateMagnitudeSquared(); }
        }
        protected void CalculateMagnitudeSquared()
        {
            internalMagnitudeSquared = 0;
            for (int i = 0; i < components.Length; ++i)
            {
                internalMagnitudeSquared += (components[i] * components[i]);
            }
            changed = true;

        }
        protected void CalculateMagnitude()
        {
            if (changed)
            {
                internalMagnitude = MathF.Sqrt(internalMagnitudeSquared);
                changed = false;
            }
        }
        protected Vec Normalize()
        {
            return Div(Length);
        }

        public Vec Clone()
        {
            Vec result = new Vec(components);
            return result;
        }

        protected void Add(float scalar)
        {
            for (int i = 0; i < components.Length; ++i)
            {
                components[i] += scalar;
            }

        }
        protected void Add(Vec v)
        {
            Vec operand;
            int size;
            if (components.Length > v.components.Length)
            {
                operand = this;
                size = components.Length;
            }
            else
            {
                operand = v;
                size = v.components.Length;
            }
            for (int i = 0; i < size; ++i)
            {
                components[i] += operand.components[i];
            }
        }

        protected void Sub(float scalar)
        {
            for (int i = 0; i < components.Length; ++i)
            {
                components[i] -= scalar;
            }
        }

        protected void Sub(Vec v)
        {
            Vec operand;
            int size;
            if (v.components.Length > components.Length)
            {
                operand = this;
                size = components.Length;
            }
            else
            {
                operand = v;
                size = v.components.Length;
            }
            for (int i = 0; i < size; ++i)
            {
                components[i] -= operand.components[i];
            }
        }

        protected void Mul(float scalar)
        {
            for (int i = 0; i < components.Length; ++i)
            {
                components[i] *= scalar;
            }
        }



        protected void Mul(Vec v)
        {
            Vec operand;
            int size;
            if (v.components.Length > components.Length)
            {
                operand = this;
                size = components.Length;
            }
            else
            {
                operand = v;
                size = v.components.Length;
            }
            for (int i = 0; i < size; ++i)
            {
                components[i] *= operand.components[i];
            }
        }

        protected Vec Div(float scalar)
        {
            Vec result = Clone();
            for (int i = 0; i < result.components.Length; ++i)
            {
                result.components[i] /= scalar;
            }
            return result;
        }

        protected Vec Div(Vec v)
        {
            Vec result;
            Vec operand;
            int size;
            if (v.components.Length > components.Length)
            {
                result = v.Clone();
                operand = this;
                size = components.Length;
            }
            else
            {
                result = Clone();
                operand = v;
                size = v.components.Length;
            }
            for (int i = 0; i < size; ++i)
            {
                result.components[i] /= operand.components[i];
            }
            return result;
        }
        protected void Negate()
        {
            Mul(-1);
        }

        protected static Vec Add(Vec v, float scalar)
        {
            Vec result = v.Clone();
            for (int i = 0; i < result.components.Length; ++i)
            {
                result.components[i] += scalar;
            }
            return result;
        }

        protected static Vec Add(Vec v1, Vec v2)
        {
            Vec result;
            Vec operand;
            int size;
            if (v1.components.Length >= v2.components.Length)
            {
                result = v1.Clone();
                operand = v1;
                size = v1.components.Length;
            }
            else
            {
                result = v2.Clone();
                operand = v2;
                size = v2.components.Length;
            }
            for (int i = 0; i < size; ++i)
            {
                result.components[i] += operand.components[i];
            }
            return result;
        }

        protected static Vec Sub(Vec v, float scalar)
        {
            Vec result = v.Clone();
            for (int i = 0; i < result.components.Length; ++i)
            {
                result.components[i] -= scalar;
            }
            return result;
        }

        protected static Vec Sub(Vec v1, Vec v2)
        {
            Vec result;
            Vec operand;
            int size;
            if (v1.components.Length >= v2.components.Length)
            {
                result = v1.Clone();
                operand = v1;
                size = v1.components.Length;
            }
            else
            {
                result = v2.Clone();
                operand = v2;
                size = v2.components.Length;
            }
            for (int i = 0; i < size; ++i)
            {
                result.components[i] -= operand.components[i];
            }
            return result;
        }

        protected static Vec Mul(Vec v, float scalar)
        {
            Vec result = v.Clone();
            for (int i = 0; i < result.components.Length; ++i)
            {
                result.components[i] *= scalar;
            }
            return result;
        }

        protected static Vec Mul(Vec v1, Vec v2)
        {
            Vec result;
            Vec operand;
            int size;
            if (v1.components.Length > v2.components.Length)
            {
                result = v1.Clone();
                operand = v2;
                size = v2.components.Length;
            }
            else
            {
                result = v2.Clone();
                operand = v1;
                size = v1.components.Length;
            }
            for (int i = 0; i < size; ++i)
            {
                result.components[i] *= operand.components[i];
            }
            return result;
        }

        protected static Vec Div(Vec v, float scalar)
        {
            Vec result = v.Clone();
            for (int i = 0; i < result.components.Length; ++i)
            {
                result.components[i] /= scalar;
            }
            return result;
        }

        protected static Vec Div(Vec v1, Vec v2)
        {
            Vec result;
            Vec operand;
            int size;
            if (v1.components.Length > v2.components.Length)
            {
                result = v1.Clone();
                operand = v2;
                size = v2.components.Length;
            }
            else
            {
                result = v2.Clone();
                operand = v1;
                size = v1.components.Length;
            }
            for (int i = 0; i < size; ++i)
            {
                result.components[i] /= operand.components[i];
            }
            return result;
        }
    }
}
