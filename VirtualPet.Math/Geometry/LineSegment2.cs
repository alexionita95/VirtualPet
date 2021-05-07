using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualPet.Math.Vectors;

namespace VirtualPet.Math.Geometry
{
    public class LineSegment2
    {
        public Vec2 Start { get; set; }
        public Vec2 End { get; set; }

        public LineSegment2()
        {
            Start = new Vec2();
            End = new Vec2();
        }
        public LineSegment2(Vec2 start, Vec2 end)
        {
            Start = start;
            End = end;
        }
        
    }
}
