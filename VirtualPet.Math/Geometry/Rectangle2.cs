using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPet.Math.Geometry
{
    public class Rectangle2 : Poly2
    {
        float internalWidth;
        float internalHeight;
        public float Width { get { return internalWidth; } set { internalWidth = value; Setup();} }
        public float Height { get { return internalHeight; } set { internalHeight = value; Setup(); } }
        public Rectangle2() 
        {

        }
        public Rectangle2(float width, float height)
        {
            Width = width;
            Height = height;
        }
        private void Setup()
        {
            SetBox(internalWidth / 2, internalHeight / 2);
        }
    }
}
