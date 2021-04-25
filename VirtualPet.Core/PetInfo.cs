using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPet.Core
{
    public class PetInfo
    {
        public enum PetState
        {
            Falling,
            Neutral,
            Walking,
            Climbing,
            Dragging,
            Sitting
        }
        public PetState State { get; set; }
        public bool WalkOnWindows { get; set; }
        public bool WalkOnMultipleScreens { get; set; }
        public bool AllowClimbing { get; set; }
        public int FallSpeed { get; set; }
        public int MoveSpeed { get; set; }
    }
}
