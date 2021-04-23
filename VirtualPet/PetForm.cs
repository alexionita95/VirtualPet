using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualPet
{
    public partial class PetForm : Form
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
                    cp.ExStyle |= 0x08000000;   //WS_EX_NOACTIVATE  <- prevent focus when created
                }
                return cp;
            }
        }
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }
        Screen currentScreen;
        NativeMethods.RECT currentWindow;
        IntPtr currentWindowHandle;
        IntPtr colidedWindowHandle;
        IntPtr hWindow;
        IntPtr vWindow;
        bool walkingOnTaskbar;
        DialogControl dialog;
        Random rnd = new Random();
        Point PetRealPosition = new Point();
        Rectangle PetBounds = new Rectangle();
        CollisionDirection climbDirection;
        int initialFallSpeed;
        public PetForm()
        {
            InitializeComponent();
            WalkOnWindows = true;
            WalkOnMultipleScreens = true;
            AllowClimbing = true;
            walkingOnTaskbar = false;
            FallSpeed = 10;
            MoveSpeed = 3;
            initialFallSpeed = FallSpeed;
            //panel1.BackColor = Color.FromArgb(rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256));
            dialog = new DialogControl();
            this.Controls.Add(dialog);
            dialog.Hide();
            ResizeForm();
            UpdateRealPosition();

        }

        private void UpdateRealPosition()
        {
            PetRealPosition.X = Location.X + petPanel.Location.X;
            PetRealPosition.Y = Location.Y + petPanel.Location.Y;
            PetBounds = new Rectangle(PetRealPosition.X, PetRealPosition.Y, petPanel.Width, petPanel.Height);
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
        }

        public void Fall()
        {
            ResizeForm();

            int x = 10;//currentScreen.Bounds.Width / 2 - this.Width / 2;
            int y = 0;
            petPanel.Location = new Point(x, y);
            petPanel.Visible = true;
            State = PetState.Falling;
            currentScreen = Screen.FromControl(petPanel);
            animTimer.Start();
        }
        public void Stay()
        {
            State = PetState.Sitting;
        }

       
        CollisionDirection HitMyBounds()
        {
            return Collisions.CheckCollision(petPanel.Bounds, new Rectangle(0, 0, Width, Height), CollisionDirection.Left | CollisionDirection.Right, true);
        }
        CollisionDirection HitScreenBounds()
        {
            return Collisions.CheckCollision(PetBounds, currentScreen.Bounds, CollisionDirection.Left | CollisionDirection.Right, true);
        }
        string RectToString(NativeMethods.RECT rect)
        {
            return $"Top: {rect.Top} Bottom: {rect.Bottom} Left: {rect.Left} Right: {rect.Right}";
        }
        bool isCurrentWindowHere()
        {
            if (colidedWindowHandle == IntPtr.Zero)
            {
                return false;
            }
            NativeMethods.RECT rect;
            NativeMethods.GetWindowRect(new System.Runtime.InteropServices.HandleRef(this, colidedWindowHandle), out rect);
            if (currentWindow.Top != rect.Top ||
            currentWindow.Bottom != rect.Bottom ||
            currentWindow.Left != rect.Left ||
            currentWindow.Right != rect.Right)
            {
                return false;

            }
            return true;
        }
        bool checkTopWindow()
        {
            if ((int)currentWindowHandle == 0)
            {
                return false;
            }
            NativeMethods.RECT rctO;
            NativeMethods.RECT rct;

            NativeMethods.GetWindowRect(new System.Runtime.InteropServices.HandleRef(this, currentWindowHandle), out rctO);


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
                    CollisionDirection myCollision = Collisions.CheckCollision(PetBounds, Collisions.GetRectangleFromNative(rct), CollisionDirection.All);
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

        bool checkWindowCollision(CollisionDirection direction = CollisionDirection.None, bool bounce = false)
        {
            bool colided = false;
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
                if (rect.Top <= petPanel.Height)
                {
                    return true;
                }
                //Console.WriteLine($"{sb} {RectToString(rect)}");
                CollisionDirection collides = Collisions.CheckCollision(PetBounds, Collisions.GetRectangleFromNative(rect), direction);
                //if ()
                if (collides != CollisionDirection.None && Collisions.isInLimits(PetBounds, Collisions.GetRectangleFromNative(rect), collides))
                {
                    currentWindowHandle = hWnd;
                    if (!checkTopWindow())
                    {
                        if (hWnd != colidedWindowHandle)
                        {
                            colidedWindowHandle = hWnd;
                            currentWindowHandle = hWnd;
                            currentWindow = rect;
                            PutMeOnEdge(Collisions.GetRectangleFromNative(currentWindow), collides, bounce, false);
                            //StayOnWindow(currentWindow, collides);
                        }
                        if (direction.HasFlag(CollisionDirection.Left) || direction.HasFlag(CollisionDirection.Right))
                        {
                            hWindow = colidedWindowHandle;
                            climbDirection = collides;
                        }
                        if (direction.HasFlag(CollisionDirection.Top) || direction.HasFlag(CollisionDirection.Bottom))
                        {
                            vWindow = colidedWindowHandle;
                        }
                        colided = true;

                    }
                }


                return true;
            }, IntPtr.Zero);

            return colided;
        }

        Point ScreenToWorld(int x, int y)
        {
            return new Point(x - this.Location.X, y - this.Location.Y);
        }
        bool CanWalk()
        {
            if (walkingOnTaskbar)
            {

                return true;
            }
            if (!isCurrentWindowHere())
            {
                return false;
            }
            if (currentWindow.Left > PetBounds.Right || currentWindow.Right < PetBounds.Left)
            {
                return false;
            }
            return true;
        }
        void WalkAround(int dir)
        {
            Point p = new Point(petPanel.Location.X, petPanel.Location.Y);
            if (State == PetState.Walking)
            {
                if (!CanWalk())
                {
                    ResetState();
                    return;
                }
                p.X += dir * MoveSpeed;
                petPanel.Location = p;
            }
        }
        bool CanClimb()
        {
            if (!isCurrentWindowHere())
            {

                return false;
            }
            if (currentWindow.Bottom < PetBounds.Top ||
                currentWindow.Top > PetBounds.Bottom)
            {
                return false;
            }
            return true;
        }
        bool AmIOnTop()
        {
            if (PetRealPosition.Y + petPanel.Height <= currentWindow.Top)
            {
                return true;
            }
            return false;
        }
        void PutMeOnEdge(Rectangle bounds, CollisionDirection direction, bool bounce = false, bool inside = true)
        {
            int horizontalOffset = 0;
            int verticalOffset = 0;
            if (bounce)
            {
                horizontalOffset = 1;
                verticalOffset = 1;
            }
            if (!inside)
            {
                if (direction.HasFlag(CollisionDirection.Left) || direction.HasFlag(CollisionDirection.Right))
                {
                    horizontalOffset += PetBounds.Width;
                }
                if (direction.HasFlag(CollisionDirection.Top) || direction.HasFlag(CollisionDirection.Bottom))
                {
                    verticalOffset += (PetBounds.Height);
                }
            }
            Point p = new Point(PetRealPosition.X, PetRealPosition.Y);
            if (!Collisions.isInLimits(PetBounds, bounds, direction))
            {
                return;
            }
            if (direction.HasFlag(CollisionDirection.Left))
            {
                p.X = bounds.Left - horizontalOffset;
            }
            if (direction.HasFlag(CollisionDirection.Right))
            {
                p.X = bounds.Right - petPanel.Width + horizontalOffset;
            }

            if (direction.HasFlag(CollisionDirection.Top))
            {
                p.Y = bounds.Top - verticalOffset;
            }

            petPanel.Location = ScreenToWorld(p.X, p.Y);
        }
        void PutMeOnTop()
        {
            Point p = new Point(PetRealPosition.X, PetRealPosition.Y - 2);

            switch (climbDirection)
            {
                case CollisionDirection.Left:
                    {
                        p.X += (Collisions.CollisionOffset + 1);
                        walkingDirection = 1;
                    }
                    break;
                case CollisionDirection.Right:
                    {
                        p.X -= (Collisions.CollisionOffset + 1);
                        walkingDirection = -1;
                    }
                    break;
            }
            p = petPanel.Location = ScreenToWorld(p.X, p.Y);
            petPanel.Location = p;
            UpdateRealPosition();
        }
        void Climb()
        {
            Point p = new Point(petPanel.Location.X, petPanel.Location.Y);
            if (!CanClimb())
            {
                return;
            }
            p.Y -= MoveSpeed;
            petPanel.Location = p;
        }
        int walkingDirection = 1;
        private void animTimer_Tick(object sender, EventArgs e)
        {
            currentScreen = Screen.FromControl(petPanel);
            if (State == PetState.Dragging || State == PetState.Sitting)
            {
                return;
            }
            Point p = new Point(petPanel.Location.X, petPanel.Location.Y);
            if (!isCurrentWindowHere() && !walkingOnTaskbar)
            {
                ResetState();
            }
            switch (State)
            {
                case PetState.Falling:
                    {
                        if (!walkingOnTaskbar)
                        {
                            p.Y += FallSpeed;
                            petPanel.Location = p;
                            if (WalkOnWindows)
                            {
                                if (checkWindowCollision(CollisionDirection.Top))
                                {
                                    if (State != PetState.Neutral)
                                    {
                                        State = PetState.Neutral;
                                    }
                                }
                            }
                            if (p.Y + petPanel.Height > this.Height && !walkingOnTaskbar)
                            {
                                walkingOnTaskbar = true;
                                p.Y = this.Height - petPanel.Height;
                                State = PetState.Neutral;
                                petPanel.Location = p;
                            }
                        }
                    }
                    break;
                case PetState.Neutral:
                    {
                        State = PetState.Walking;
                    }
                    break;
                case PetState.Walking:
                    {
                        if (WalkOnMultipleScreens)
                        {
                            CollisionDirection direction = HitMyBounds();
                            if (direction != CollisionDirection.None)
                            {
                                PutMeOnEdge(this.Bounds, direction);
                                walkingDirection *= -1;

                            }

                        }
                        else
                        {
                            CollisionDirection result = HitScreenBounds();
                            if (result != CollisionDirection.None)
                            {
                                PutMeOnEdge(currentScreen.Bounds, result);
                                walkingDirection *= -1;
                            }
                        }
                        if (WalkOnWindows)
                        {
                            if (!checkWindowCollision(CollisionDirection.Top) && !walkingOnTaskbar)
                            {
                                ResetState();
                            }
                            if (checkWindowCollision(CollisionDirection.Left | CollisionDirection.Right, true))
                            {
                                if (AllowClimbing && hWindow != vWindow)
                                {
                                    State = PetState.Climbing;
                                }
                                else
                                {
                                    if (CanWalk() && hWindow != vWindow)
                                    {
                                        walkingDirection *= -1;
                                    }
                                }
                            }
                        }
                        WalkAround(walkingDirection);
                    }
                    break;
                case PetState.Dragging:
                    break;
                case PetState.Sitting:
                    return;
                case PetState.Climbing:
                    {
                        Climb();
                        if (checkWindowCollision(CollisionDirection.Bottom) && !AmIOnTop())
                        {
                            if (vWindow != hWindow)
                            {
                                ResetState();
                                walkingDirection *= -1;
                            }
                        }
                        if (AmIOnTop())
                        {
                            PutMeOnTop();
                            ResetState();
                        }
                    }
                    break;
            }
            UpdateRealPosition();
            //Console.WriteLine(PetRealPosition);
             DialogFollowMe();
        }
        private void ResetState()
        {
            currentWindowHandle = IntPtr.Zero;
            colidedWindowHandle = IntPtr.Zero;
            hWindow = IntPtr.Zero;
            vWindow = IntPtr.Zero;
            walkingOnTaskbar = false;
            currentWindow = new NativeMethods.RECT();
            State = PetState.Falling;
        }
        int lastX;
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            lastX = petPanel.Location.X;
            State = PetState.Dragging;
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (lastX < petPanel.Location.X)
            {
                walkingDirection = 1;
            }
            else
            {
                walkingDirection = -1;
            }
            if (e.Button == MouseButtons.Left)
            {

                ResetState();
            }
            if (e.Button == MouseButtons.Right)
            {

                Stay();
            }
            UpdateRealPosition();
            DialogFollowMe();
        }
        Point precpos;
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            Size delta = new Size(e.X - precpos.X, e.Y - precpos.Y);
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                petPanel.BringToFront();
                petPanel.Location += delta;
                precpos = e.Location - delta;
            }
            else
            {
                precpos = e.Location;
                if (petPanel.Location.Y < 0)
                {
                    petPanel.Location = new Point(petPanel.Location.X, 0);
                }
            }
        }
        void DialogFollowMe()
        {
            if (dialog.Visible)
            {
                dialog.Location = new Point(petPanel.Location.X - dialog.Width, petPanel.Location.Y - dialog.Height);
            }
        }
        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void panel1_DoubleClick(object sender, EventArgs e)
        {

            dialog.Show();
            dialog.LoadData();

        }
    }
}
