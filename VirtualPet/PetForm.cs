using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VirtualPet.Core;

namespace VirtualPet
{
    public partial class PetForm : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                cp.ExStyle |= 0x00000080; // WS_EX_TOOLWINDOW             <- remove from ALT-TAB list
                cp.ExStyle |= 0x00000008; // WS_EX_TOPMOST                <- set on TopMost
                cp.ExStyle |= 0x00080000; // WS_EX_LAYERED                <- increase paint performance
                //cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT            <- Do not draw window -> unclickable
                //cp.Style |= 0x80000000; // WS_POPUP

                if (Name.IndexOf("child") == 0)
                {
                    //cp.ExStyle |= 0x08000000;   //WS_EX_NOACTIVATE  <- prevent focus when created
                }
                return cp;
            }
        }
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        Entity pet = new Entity();
        public PetForm()
        {
            InitializeComponent();
            ResizeForm();

        }

        public PetForm(PetInfo info)
        {
            InitializeComponent();
            ResizeForm();


        }

        Point ScreenToWorld(int x, int y)
        {
            return new Point(x - this.Location.X, y - this.Location.Y);
        }



        private void ResizeForm()
        {
            int x = 0;
            int y = 0;
            int widh = 0;
            int height = 0;
            foreach (Screen s in Screen.AllScreens)
            {
                widh += s.Bounds.Width;
                if (height == 0 || s.WorkingArea.Height < height)
                    height = s.WorkingArea.Height;
                if (s.Bounds.X < x)
                    x = s.Bounds.X;
                if (s.Bounds.Y < y)
                {
                    y = s.Bounds.Y;
                }

            }
            this.Size = new Size(widh, height);
            this.Location = new Point(x, y);

            this.Refresh();
            this.Update();
            pet.AllowedArea = this.Bounds;
        }
        public void SpawnDialog(UserControl dialog)
        {
            Controls.Add(dialog);
            dialog.Show();
        }

        public void Fall()
        {
            ResizeForm();
            pet.AddAttribute(Entity.CoreAttributes.WalkOnWindows);
            pet.AddAttribute(Entity.CoreAttributes.ClimbOnWindows);
            int x = 10;//currentScreen.Bounds.Width / 2 - this.Width / 2;
            int y = 0;
            pet.Position = this.PointToScreen(new Point(x, y));
            petPanel.Visible = true;
            pet.AllowedArea = this.Bounds;
            animTimer.Start();
        }
        public void Stay()
        {
            pet.State = Entity.States.Sitting;
        }
        public void SpawnToy(Toy t)
        {
            this.Controls.Add(t);
            t.Location = new Point(0, 0);
            t.Show();
        }

        bool checkTopWindow(Rectangle collider, NativeMethods.RECT rctO, IntPtr currentWindowHandle)
        {
            NativeMethods.RECT rct;


            IntPtr nextWindow = NativeMethods.GetTopWindow(IntPtr.Zero);

            StringBuilder sb = new StringBuilder(128);
            while (nextWindow != IntPtr.Zero)
            {
                if (nextWindow == currentWindowHandle)
                {
                    return false;
                }
                if (NativeMethods.IsWindowVisible(nextWindow) && !NativeMethods.IsIconic(nextWindow) && nextWindow != Handle)
                {
                    NativeMethods.GetWindowText(nextWindow, sb, 128);
                    NativeMethods.GetWindowRect(new System.Runtime.InteropServices.HandleRef(this, nextWindow), out rct);
                    CollisionDirection windowCollision = Collisions.CheckCollision(Collisions.GetRectangleFromNative(rctO), Collisions.GetRectangleFromNative(rct), CollisionDirection.All);
                    CollisionDirection myCollision = Collisions.CheckCollision(collider, Collisions.GetRectangleFromNative(rct), CollisionDirection.All);
                    if (NativeMethods.IsZoomed(nextWindow) && windowCollision != CollisionDirection.None && myCollision != CollisionDirection.None && sb.Length != 0)
                    {
                        return true;
                    }
                    if (myCollision != CollisionDirection.None && sb.Length != 0)
                    {
                        return true;
                    }

                }
                nextWindow = NativeMethods.GetWindow(nextWindow, 2);
            }

            return false;
        }

        Entity.Collider checkWindowCollision(Rectangle collider, CollisionDirection direction = CollisionDirection.None, bool bounce = false)
        {
            Entity.Collider collided = new Entity.Collider { Direction = CollisionDirection.None, Bounds = new Rectangle() };
            NativeMethods.EnumWindows(delegate (IntPtr hWnd, int lParam)
            {

                if (hWnd == Handle)
                {
                    return true;
                }
                NativeMethods.TITLEBARINFO info = new NativeMethods.TITLEBARINFO();
                NativeMethods.GetTitleBarInfo(hWnd, ref info);
                if ((info.rgstate[0] & 0x00008000) > 0) // invisible
                {
                    return true;
                }
                if (!NativeMethods.IsWindowVisible(hWnd))
                {
                    return true;
                }
                StringBuilder sb = new StringBuilder(128);

                NativeMethods.GetWindowText(hWnd, sb, 128);
                if (sb.Length == 0)
                {
                    return true;
                }
                if (sb.ToString() == Text)
                {
                    return true;
                }
                if (NativeMethods.IsZoomed(hWnd))
                {
                    return true;
                }
                if (NativeMethods.IsIconic(hWnd))
                {
                    return true;
                }
                NativeMethods.RECT rect = new NativeMethods.RECT();
                NativeMethods.GetWindowRect(new System.Runtime.InteropServices.HandleRef(this, hWnd), out rect);

                CollisionDirection collides = Collisions.CheckCollision(collider, Collisions.GetRectangleFromNative(rect), direction);

                if (collides != CollisionDirection.None && Collisions.isInLimits(collider, Collisions.GetRectangleFromNative(rect), collides))
                {
                    if (!checkTopWindow(collider, rect, hWnd))
                    {
                        collided.Bounds = Collisions.GetRectangleFromNative(rect);
                        collided.Direction = collides;

                    }
                }


                return true;
            }, IntPtr.Zero);

            return collided;
        }

        public bool IsEntityOnTop(Rectangle entity, Rectangle obstacle)
        {
            return Collisions.isBetween(entity.Bottom, obstacle.Top, obstacle.Top + Collisions.CollisionOffset);
        }
        public bool IsEntityOnEdge(Rectangle entity, Rectangle obstacle, CollisionDirection direction, bool inside)
        {
            if (direction.HasFlag(CollisionDirection.Left))
            {
                if (!inside)
                    return Collisions.isBetween(entity.Right, obstacle.Left, obstacle.Left + Collisions.CollisionOffset);
                return Collisions.isBetween(entity.Left, obstacle.Left, obstacle.Left + Collisions.CollisionOffset);

            }
            if (direction.HasFlag(CollisionDirection.Right))
            {
                if (!inside)
                    return Collisions.isBetween(entity.Left, obstacle.Right - Collisions.CollisionOffset, obstacle.Right);
                return Collisions.isBetween(entity.Right, obstacle.Right - Collisions.CollisionOffset, obstacle.Right);

            }
            return false;
        }
        void PutEntityOnEdge(Entity e, Rectangle bounds, CollisionDirection direction, bool bounce = false, bool inside = true)
        {
            int horizontalOffset = 0;
            int verticalOffset = 0;
            if (bounce)
            {
                horizontalOffset = Collisions.CollisionOffset;
                verticalOffset = Collisions.CollisionOffset;
            }
            if (!inside)
            {
                if (direction.HasFlag(CollisionDirection.Left) || direction.HasFlag(CollisionDirection.Right))
                {
                    horizontalOffset += e.Bounds.Width;
                }
                if (direction.HasFlag(CollisionDirection.Top) || direction.HasFlag(CollisionDirection.Bottom))
                {
                    verticalOffset += e.Bounds.Height;
                }
            }
            Point p = new Point(e.Position.X, e.Position.Y);
            if (!Collisions.isInLimits(e.Bounds, bounds, direction))
            {
                return;
            }
            if (direction.HasFlag(CollisionDirection.Left))
            {
                p.X = bounds.Left - horizontalOffset;
            }
            if (direction.HasFlag(CollisionDirection.Right))
            {
                p.X = bounds.Right + (horizontalOffset - e.Width);
            }

            if (direction.HasFlag(CollisionDirection.Top))
            {
                p.Y = bounds.Top - verticalOffset;
            }

            e.Position = p;
        }
        void PutEntityOnTop(Entity e, Rectangle obstacle, CollisionDirection direction)
        {
            Point p = e.Position;

            switch (direction)
            {
                case CollisionDirection.Left:
                    {
                        p.X = obstacle.Left + Collisions.CollisionOffset + 1;
                        e.WalkingDirection = 1;
                    }
                    break;
                case CollisionDirection.Right:
                    {
                        p.X = obstacle.Right - e.Bounds.Width - Collisions.CollisionOffset - 1;
                        e.WalkingDirection = -1;
                    }
                    break;
            }
            e.Position = p;
        }
        private string checkEntityWindowCollision(Entity e)
        {
            Entity.Collider result;


            string state = Entity.States.Falling;
            if (e.HasAttribute(Entity.CoreAttributes.WalkOnWindows) && !e.IsOnBottom)
            {
                result = checkWindowCollision(e.Bounds, CollisionDirection.Top);
                if (result.Direction != CollisionDirection.None)
                {
                    state = Entity.States.Walking;
                    if (state != e.State)
                    {
                        PutEntityOnEdge(e, result.Bounds, result.Direction, false, false);
                    }
                }
                else
                {
                    if (e.IsOnBottom)
                    {
                        state = Entity.States.Walking;
                    }
                }
            }
            else
            {
                if (e.IsOnBottom)
                {
                    state = Entity.States.Walking;
                }
                else
                {
                    state = Entity.States.Falling;
                }
            }
            result = checkWindowCollision(e.Bounds, CollisionDirection.Left | CollisionDirection.Right);
            if (result.Direction != CollisionDirection.None)
            {
                if (e.HasAttribute(Entity.CoreAttributes.ClimbOnWindows))
                {

                    if (result.Direction == e.Direction)
                    {
                        state = Entity.States.Climbing;
                        PutEntityOnEdge(e, result.Bounds, result.Direction, false, false);
                    }
                    if (IsEntityOnTop(e.Bounds, result.Bounds))
                    {
                        if (IsEntityOnEdge(e.Bounds, result.Bounds, result.Direction, false) && e.State != Entity.States.Climbing)
                        {
                            PutEntityOnEdge(e, result.Bounds, result.Direction, true, false);

                            state = Entity.States.Falling;
                        }
                        else
                        {
                            PutEntityOnTop(e, result.Bounds, result.Direction);
                            state = Entity.States.Walking;
                        }
                    }

                }
                else
                {
                    PutEntityOnEdge(e, result.Bounds, result.Direction, true, false);
                    e.Bounce();
                }
            }
            else
            {
                if (e.IsOnBottom)
                {
                    state = Entity.States.Walking;
                }
            }





            return state;
        }
        private void animTimer_Tick(object sender, EventArgs e)
        {
            pet.State = checkEntityWindowCollision(pet);

            pet.Tick(0);
            petPanel.Location = this.PointToClient(pet.Position);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            //grab pet
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            //release pet
            if (e.Button == MouseButtons.Left)
            {

            }
            if (e.Button == MouseButtons.Right)
            {

            }
        }
        Point precpos;
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            Size delta = new Size(e.X - precpos.X, e.Y - precpos.Y);
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                //update pet position
                petPanel.BringToFront();
                precpos = e.Location - delta;
            }
            else
            {
                precpos = e.Location;
            }
        }
    }
}
