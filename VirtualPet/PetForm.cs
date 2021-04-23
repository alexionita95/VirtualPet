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
        bool walkingOnTaskbar;
        DialogForm dialog;
        Random rnd = new Random();
        Point PetRealPosition = new Point();
        LeaveDirection climbDirection;
        public PetForm()
        {
            InitializeComponent();
            WalkOnWindows = true;
            WalkOnMultipleScreens = true;
            AllowClimbing = false;
            walkingOnTaskbar = false;
            //panel1.BackColor = Color.FromArgb(rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256));
            dialog = new DialogForm();
            ResizeForm();
            UpdateRealPosition();

        }

        private void UpdateRealPosition()
        {
            PetRealPosition.X = Location.X + petPanel.Location.X;
            PetRealPosition.Y = Location.Y + petPanel.Location.Y;
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
        enum LeaveDirection
        {
            None,
            Left,
            Right
        };
        LeaveDirection HitMyBounds()
        {
            if (petPanel.Location.X < 0)
            {
                return LeaveDirection.Left;
            }
            if (petPanel.Location.X + petPanel.Width > this.Width)
            {
                return LeaveDirection.Right;
            }
            return LeaveDirection.None;
        }
        LeaveDirection HitScreenBounds()
        {
            if (PetRealPosition.X < currentScreen.Bounds.Left)
            {
                return LeaveDirection.Left;
            }
            if (currentScreen.Bounds.Right < PetRealPosition.X + petPanel.Width)
            {
                return LeaveDirection.Right;
            }
            return LeaveDirection.None;
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
            if (State == PetState.Walking)
            {

                if (currentWindow.Left > PetRealPosition.X + petPanel.Width || currentWindow.Right < PetRealPosition.X)
                {
                    return false;
                }
            }
            if (State == PetState.Climbing)
            {
                if (currentWindow.Bottom < PetRealPosition.Y || currentWindow.Top > PetRealPosition.Y + petPanel.Height)
                {
                    return false;
                }
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

                    if (NativeMethods.IsZoomed(nextWindow) && ColidesWith(rct))
                    {
                        return true;
                    }
                    if (ColidesWith(rct))
                    {
                        return true;
                    }

                }
                nextWindow = NativeMethods.GetWindow(nextWindow, 2);
            }

            return false;
        }

        bool checkTopWindowWalls()
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

                    if (NativeMethods.IsZoomed(nextWindow) && ColidesWith(rct))
                    {
                        return true;
                    }
                    if (ColidesWithWall(rct) != LeaveDirection.None)
                    {
                        return true;
                    }

                }
                nextWindow = NativeMethods.GetWindow(nextWindow, 2);
            }

            return false;
        }

        LeaveDirection ColidesWithWall(NativeMethods.RECT rect)
        {
            if (rect.Top < PetRealPosition.Y + petPanel.Height && rect.Bottom > PetRealPosition.Y + petPanel.Height)
            {
                if (rect.Left <= PetRealPosition.X + petPanel.Width && rect.Right > PetRealPosition.X)
                {
                    return LeaveDirection.Left;
                }
                if ((rect.Right >= PetRealPosition.X && rect.Left < PetRealPosition.X))
                {
                    return LeaveDirection.Right;
                }
            }
            return LeaveDirection.None;
        }
        bool checkWindowWallCollisions()
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
                LeaveDirection result = ColidesWithWall(rect);
                if (result != LeaveDirection.None)
                {
                    currentWindowHandle = hWnd;
                    if (!checkTopWindowWalls() && isInLimits(rect))
                    {
                        if (AllowClimbing)
                        {
                            if (hWnd != colidedWindowHandle)
                            {
                                colidedWindowHandle = hWnd;
                                currentWindowHandle = hWnd;
                                currentWindow = rect;
                                StayOnWindow(currentWindow, result);
                                climbDirection = result;

                            }
                        }
                        else
                        {
                            StayOnWindow(rect, result);
                        }
                        colided = true;

                    }
                }


                return true;
            }, IntPtr.Zero);

            return colided;
        }

        bool checkWindowCollision()
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

                if (ColidesWith(rect) && isOnTop(rect))
                {

                    currentWindowHandle = hWnd;
                    if (!checkTopWindow())
                    {
                        if (hWnd != colidedWindowHandle)
                        {
                            colidedWindowHandle = hWnd;
                            currentWindowHandle = hWnd;
                            currentWindow = rect;
                            StayOnWindow(currentWindow);

                        }
                        colided = true;

                    }
                }


                return true;
            }, IntPtr.Zero);

            return colided;
        }

        bool ColidesWith(NativeMethods.RECT rect)
        {
            int rectWidth = rect.Right - rect.Left;
            int rectHeight = rect.Bottom - rect.Top;

            /*   if (this.Location.Y + offset > rect.Top && this.Location.Y < rect.Bottom || this.Location.Y > rect.Bottom) 
               {

                   return false;
               }   */
            if ((PetRealPosition.Y + petPanel.Height >= rect.Top) && PetRealPosition.X + petPanel.Width > rect.Left && PetRealPosition.X < rect.Right && rect.Bottom > PetRealPosition.Y)
            {

                return true;
            }
            return false;
        }
        bool isInLimits(NativeMethods.RECT rect)
        {
            if ((PetRealPosition.X + petPanel.Width < rect.Left + 20 && PetRealPosition.X + petPanel.Width > rect.Left)
                || (PetRealPosition.X > rect.Right - 20 && PetRealPosition.X < rect.Right))
            {
                return true;
            }
            return false;
        }
        bool isOnTop(NativeMethods.RECT rect)
        {
            if (PetRealPosition.Y + petPanel.Height > rect.Top + 20)
            {
                return false;
            }
            return true;
        }
        Point ScreenToWorld(int x, int y)
        {
            return new Point(x - this.Location.X, y - this.Location.Y);
        }
        void StayOnWindow(NativeMethods.RECT rect, LeaveDirection direction = LeaveDirection.None)
        {
            Point p = new Point();
            switch (direction)
            {
                case LeaveDirection.None:
                    p = ScreenToWorld(PetRealPosition.X, rect.Top - petPanel.Height);
                    break;
                case LeaveDirection.Left:
                    p = ScreenToWorld(rect.Left - petPanel.Width, PetRealPosition.Y);
                    break;
                case LeaveDirection.Right:
                    p = ScreenToWorld(rect.Right, PetRealPosition.Y);
                    break;
            }
            petPanel.Location = p;
            UpdateRealPosition();
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
            if (currentWindow.Left >= PetRealPosition.X + petPanel.Width ||
                currentWindow.Right <= PetRealPosition.X ||
                currentWindow.Top < PetRealPosition.Y)
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
                p.X += dir * 2;
                petPanel.Location = p;
            }
        }
        bool CanClimb()
        {
            if (!isCurrentWindowHere())
            {

                return false;
            }
            if (currentWindow.Bottom < PetRealPosition.Y ||
                currentWindow.Top > PetRealPosition.Y + petPanel.Height)
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
        void PutMeOnEdge(Rectangle bounds, LeaveDirection direction)
        {
            Point p = new Point(PetRealPosition.X, PetRealPosition.Y);
            switch (direction)
            {
                case LeaveDirection.Left:
                    p.X = bounds.Left + 1;
                    break;
                case LeaveDirection.Right:
                    p.X = bounds.Right - petPanel.Width - 1;
                    break;
            }
            petPanel.Location = ScreenToWorld(p.X, p.Y);
        }
        void PutMeOnTop()
        {
            Point p = new Point(PetRealPosition.X, PetRealPosition.Y - 2);

            switch (climbDirection)
            {
                case LeaveDirection.Left:
                    {
                        p.X += 1;
                        walkingDirection = 1;
                    }
                    break;
                case LeaveDirection.Right:
                    {
                        p.X -= 1;
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
                ResetState();
                return;
            }
            p.Y -= 1;
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
                State = PetState.Falling;
            }
            switch (State)
            {
                case PetState.Falling:
                    {
                        if (!walkingOnTaskbar)
                        {
                            p.Y += 5;
                            petPanel.Location = p;
                            if (WalkOnWindows)
                            {
                                if (checkWindowCollision())
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
                        WalkAround(walkingDirection);
                        if (WalkOnMultipleScreens)
                        {
                            LeaveDirection direction = HitMyBounds();
                            if (direction != LeaveDirection.None)
                            {
                                PutMeOnEdge(this.Bounds, direction);
                                walkingDirection *= -1;

                            }

                        }
                        else
                        {
                            LeaveDirection result = HitScreenBounds();
                            if (result != LeaveDirection.None)
                            {
                                PutMeOnEdge(currentScreen.Bounds, result);
                                walkingDirection *= -1;
                            }
                        }
                        if (WalkOnWindows)
                        {
                            if (!checkWindowCollision() && !walkingOnTaskbar)
                            {
                                ResetState();
                            }
                            if (checkWindowWallCollisions())
                            {
                                if (AllowClimbing)
                                {
                                    State = PetState.Climbing;
                                }
                                else
                                {
                                    walkingDirection *= -1;
                                }
                            }
                        }
                    }
                    break;
                case PetState.Dragging:
                    break;
                case PetState.Sitting:
                    return;
                case PetState.Climbing:
                    {
                        Climb();
                        if (!checkWindowWallCollisions() && !AmIOnTop())
                        {
                            ResetState();
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
            // DialogFollowMe();
        }
        private void ResetState()
        {
            currentWindowHandle = IntPtr.Zero;
            colidedWindowHandle = IntPtr.Zero;
            walkingOnTaskbar = false;
            currentWindow = new NativeMethods.RECT();
            State = PetState.Falling;
            Console.WriteLine("Reset");
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
                dialog.Location = new Point(this.Location.X - dialog.Width, this.Location.Y - dialog.Height);
            }
        }
        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void panel1_DoubleClick(object sender, EventArgs e)
        {

            dialog.Show();

        }
    }
}
