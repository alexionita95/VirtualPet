using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace VirtualPet.Core
{
    [Flags]
    public enum EntityAttribute
    {
        None = 1 << 0,
        Collidable = 1 << 1
    }
    public class Entity
    {
        public class Collider
        {
            public CollisionDirection Direction { get; set; }
            public Rectangle Bounds { get; set; }
        }
        public static class CoreAttributes
        {
            public const string Walkable = "walkable";
            public const string Climbable = "climbable";
            public const string Collidable = "collidable";
            public const string WalkOnWindows = "walkOnWindows";
            public const string CollideWithWindows = "collideWithWindows";
            public const string ClimbOnWindows = "climbOnWindows";
        }
        public static class States
        {
            public const string Neutral = "neutral";
            public const string Falling = "falling";
            public const string Sitting = "sitting";
            public const string Walking = "walking";
            public const string Climbing = "climbing";
            public const string Dragging = "dragging";

        }
        public Point Position { get { return internalPosition; } set { internalPosition = value; UpdateBounds(); } }
        public Rectangle Bounds { get; set; }
        public Rectangle AllowedArea { get; set; }
        public List<string> Attributes { get; set; }
        public string State { get; set; }
        public IntPtr ParentHandle { get; set; }

        int BoundOffset = 5;
        public int Width = 40;

        public int WalkingDirection { get; set; } = 1;
        public CollisionDirection Direction { get { return (WalkingDirection == 1) ? CollisionDirection.Left : CollisionDirection.Right; } }

        Point internalPosition;
        bool onOnBottom = false;
        public bool IsOnBottom { get { return onOnBottom; } }
        void Initialize()
        {
            Position = new Point(0, 0);

            Attributes = new List<string>();
            State = States.Falling;
        }
        private void UpdateBounds()
        {
            Bounds = new Rectangle(new Point(Position.X + BoundOffset, Position.Y + BoundOffset), new Size(Width - 2 * BoundOffset, Width - 2 * BoundOffset));
        }
        public bool HasAttribute(string attribute)
        {
            return Attributes.Contains(attribute);
        }
        public void Bounce()
        {
            WalkingDirection *= -1;
        }
        public bool AddAttribute(string attribute)
        {
            if (HasAttribute(attribute))
            {
                return false;
            }
            Attributes.Add(attribute);
            return true;
        }
        public Entity()
        {
            Initialize();
        }
        public void Reset()
        {
            onOnBottom = false;
            State = States.Falling;
        }

        public void Tick(int milliseconds)
        {
            Point p = Position;
            switch(State)
            {
                case States.Falling:
                    {
                        p.Y += 5;
                        if(p.Y+ Width > AllowedArea.Bottom)
                        {
                            p.Y = AllowedArea.Bottom - Width;
                            onOnBottom = true;
                            State = States.Walking;
                        }
                    }
                    break;
                case States.Climbing:
                    {
                        p.Y -= 5;
                        onOnBottom = false;
                    }
                    break;
                case States.Walking:
                    {
                        p.X += 5*WalkingDirection;
                        if(p.X < AllowedArea.Left)
                        {
                            p.X = AllowedArea.Left;
                            Bounce();
                        }
                        if(p.X + Width > AllowedArea.Right)
                        {
                            p.X = AllowedArea.Right - Width;
                            Bounce();
                        }

                    }
                    break;
            }
            this.Position = p;
            UpdateBounds();
        }
    }
}
