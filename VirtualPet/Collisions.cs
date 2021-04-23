using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace VirtualPet
{


    [Flags]
    public enum CollisionDirection
    {
        None = 0,
        Left = 1 << 1,
        Right = 1 << 2,
        Top = 1 << 3,
        Bottom = 1 << 4,
        All = ~(~0 << 5)
    };
    public class Collisions
    {
        public static int CollisionOffset = 10;

        public static bool isBetween(int value, int min, int max)
        {
            if (min > value || max < value)
            {
                return false;
            }
            return true;
        }

        public static Rectangle GetRectangleFromNative(NativeMethods.RECT rect)
        {
            return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }
        public static bool isInLimits(Rectangle collider, Rectangle obstacle, CollisionDirection direction)
        {
            if (direction.HasFlag(CollisionDirection.Left) && isBetween(collider.Right, obstacle.Left, obstacle.Left + CollisionOffset))
            {
                return true;
            }

            if (direction.HasFlag(CollisionDirection.Right) && isBetween(collider.Left, obstacle.Right - CollisionOffset, obstacle.Right))
            {
                return true;
            }

            if (direction.HasFlag(CollisionDirection.Top) && isBetween(collider.Bottom, obstacle.Top, obstacle.Top + CollisionOffset))
            {
                return true;
            }

            if (direction.HasFlag(CollisionDirection.Bottom) && isBetween(collider.Top, obstacle.Bottom - CollisionOffset, obstacle.Bottom))
            {
                return true;
            }
            return false;
        }
        public static CollisionDirection CheckCollision(Rectangle collider, Rectangle obstacle, CollisionDirection direction, bool isInside = false)
        {
            CollisionDirection result = CollisionDirection.None;
            if (isBetween(collider.Top, obstacle.Top, obstacle.Bottom) || isBetween(collider.Bottom, obstacle.Top, obstacle.Bottom))
            {
                if (direction.HasFlag(CollisionDirection.Left))
                {
                    if ((!isInside && isBetween(collider.Right, obstacle.Left, obstacle.Right)) || (isInside && isBetween(collider.Left, obstacle.Left - collider.Width, obstacle.Left)))
                    {
                        result |= CollisionDirection.Left;
                    }
                }

                if (direction.HasFlag(CollisionDirection.Right))
                {
                    if ((!isInside && isBetween(collider.Left, obstacle.Left, obstacle.Right)) || (isInside && isBetween(collider.Right, obstacle.Right, collider.Width + obstacle.Right)))
                    {
                        result |= CollisionDirection.Right;
                    }
                }

            }

            if (isBetween(collider.Left, obstacle.Left, obstacle.Right) || isBetween(collider.Right, obstacle.Left, obstacle.Right))
            {
                if (direction.HasFlag(CollisionDirection.Top))
                {
                    if ((!isInside && isBetween(collider.Bottom, obstacle.Top, obstacle.Bottom)) || (isInside && isBetween(collider.Top, obstacle.Top - collider.Width, obstacle.Top)))
                    {
                        result |= CollisionDirection.Top;
                    }
                }

                if (direction.HasFlag(CollisionDirection.Bottom))
                {
                    if ((!isInside && isBetween(collider.Top, obstacle.Top, obstacle.Bottom)) || (isInside && isBetween(collider.Bottom, obstacle.Bottom, collider.Height + collider.Bottom)))
                    {
                        result |= CollisionDirection.Bottom;
                    }
                }

            }
            return result;
        }
    }
}
