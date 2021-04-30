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
            for(int i=0;i<components.Length;++i)
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

        protected Vec Add(float scalar)
        {
            Vec result = Clone();
            for(int i=0;i<result.components.Length;++i)
            {
                result.components[i] += scalar;
            }
            return result;
        }

        protected Vec Add(Vec v)
        {
            Vec result;
            Vec operand;
            int size;
            if(v.components.Length > components.Length)
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
            for(int i=0;i<size;++i)
            {
                result.components[i] += operand.components[i];
            }
            return result;
        }

        protected Vec Sub(float scalar)
        {
            Vec result = Clone();
            for (int i = 0; i < result.components.Length; ++i)
            {
                result.components[i] -= scalar;
            }
            return result;
        }

        protected Vec Sub(Vec v)
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
                result.components[i] -= operand.components[i];
            }
            return result;
        }

        protected Vec Mul(float scalar)
        {
            Vec result = Clone();
            for (int i = 0; i < result.components.Length; ++i)
            {
                result.components[i] *= scalar;
            }
            return result;
        }

        protected Vec Mul(Vec v)
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
                result.components[i] *= operand.components[i];
            }
            return result;
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
        protected Vec Negate()
        {
            return Mul(-1);
        }
    }
}
