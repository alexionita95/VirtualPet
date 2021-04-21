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
            Dragging,
            Sitting
        }

        public PetState State { get; set; }
        public bool WalkOnWindows { get; set; }
        public bool WalkOnMultipleScreens { get; set; }

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

        /// <summary>
        /// With this overridden function, it is possible to prevent the form to get the focus once created.
        /// </summary>
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }
        Screen currentScreen;
        NativeMethods.RECT currentWindow;
        IntPtr currentWindowHandle;
        IntPtr colidedWindowHandle;
        bool walkingOnTaskbar;
        Random rnd = new Random();
        public PetForm()
        {
            InitializeComponent();
            this.Visible = false;
            WalkOnWindows = true;
            WalkOnMultipleScreens = false;
            walkingOnTaskbar = false;
            currentScreen = Screen.FromControl(this);
            //panel1.BackColor = Color.FromArgb(rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256));
        }

        public void Fall()
        {

            int x = currentScreen.Bounds.X + 10;//currentScreen.Bounds.Width / 2 - this.Width / 2;
            int y = currentScreen.Bounds.Y;
            this.Location = new Point(x, y);
            this.Visible = true;
            State = PetState.Falling;
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
        LeaveDirection LeavingScreen()
        {
            if (currentScreen.WorkingArea.Right < this.Location.X)
            {
                return LeaveDirection.Right;
            }
            if (currentScreen.WorkingArea.Left > this.Location.X + this.Width)
            {
                return LeaveDirection.Left;
            }
            return LeaveDirection.None;
        }
        bool HitScreenBounds()
        {
            if(Location.X < currentScreen.Bounds.Left || currentScreen.Bounds.Right < Location.X + Width)
            {
                return true;
            }
            return false;
        }
        void ChangeScreen(LeaveDirection dir)
        {
            var screens = Screen.AllScreens;
            int currentIndex = 0;
            for (int i = 0; i < screens.Length; ++i)
            {
                if (screens[i] == currentScreen)
                {
                    currentIndex = i;
                }
            }
            currentIndex++;
            if (currentIndex >= screens.Length)
            {
                currentIndex = 0;
            }
            currentScreen = screens[currentIndex];
            Console.WriteLine(currentIndex);
            if (dir == LeaveDirection.Left)
            {
                this.Location = new Point(currentScreen.WorkingArea.Right - this.Width + 1, this.Location.Y);
            }
            if (dir == LeaveDirection.Right)
            {
                this.Location = new Point(currentScreen.WorkingArea.Left - this.Width + 1, this.Location.Y);
            }
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
            // Console.WriteLine($"Current: {RectToString(currentWindow)} New: {RectToString(rect)}");
            if (currentWindow.Top != rect.Top ||
                currentWindow.Bottom != rect.Bottom ||
                currentWindow.Left != rect.Left ||
                currentWindow.Right != rect.Right)
            {
                return false;
            }
            if (currentWindow.Left > Location.X || currentWindow.Right < Location.X)
            {
                return false;
            }
            if (!checkTopWindow())
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
                if (NativeMethods.IsWindowVisible(nextWindow))
                {
                    NativeMethods.GetWindowText(nextWindow, sb, 128);
                    NativeMethods.GetWindowRect(new System.Runtime.InteropServices.HandleRef(this, nextWindow), out rct);
                    if ((rct.Top < rctO.Top && rct.Bottom > rctO.Top))
                    {
                        if (ColidesWith(rct))
                        {
                            return true;
                        }
                    }
                    if (NativeMethods.IsZoomed(nextWindow))
                    {
                        return true;
                    }
                }
                nextWindow = NativeMethods.GetWindow(nextWindow, 2);
            }

            return false;
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
                if(sb.ToString() == Text)
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

                if (ColidesWith(rect))
                {
                    currentWindowHandle = hWnd;
                    if (!checkTopWindow())
                    {
                        colidedWindowHandle = hWnd;
                        currentWindowHandle = hWnd;
                        currentWindow = rect;
                        StayOnWindow(currentWindow);
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
            if (rectHeight >= currentScreen.WorkingArea.Height && rectWidth >= currentScreen.WorkingArea.Width)
            {
                //return false;
            }
            if ((this.Location.Y + this.Height > rect.Top) && this.Location.X > rect.Left && this.Location.X + this.Width < rect.Right)
            {

                return true;
            }
            return false;
        }
        void StayOnWindow(NativeMethods.RECT rect)
        {
            Point p = this.Location;
            p.Y = rect.Top - this.Height;
            this.Location = p;
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
            if (currentWindow.Left >= Location.X + this.Width || 
                currentWindow.Right <= Location.X || 
                currentWindow.Top < Location.Y)
            {
                return false;
            }
            return true;
        }
        void WalkAround(int dir)
        {
            Point p = new Point(Location.X, Location.Y);
            if (State == PetState.Walking)
            {
                if (!CanWalk())
                {
                    ResetState();
                    return;
                }
                p.X += dir * 2;
                Location = p;
            }
        }
        int walkingDirection = 1;
        private void animTimer_Tick(object sender, EventArgs e)
        {
            currentScreen = Screen.FromControl(this);
            if (State == PetState.Dragging || State == PetState.Sitting)
            {
                return;
            }
            Point p = new Point(Location.X, Location.Y);
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
                            this.Location = p;
                            if (WalkOnWindows)
                            {
                                if (checkWindowCollision())
                                {
                                    if (State != PetState.Neutral)
                                    {
                                        State = PetState.Neutral;
                                        this.Location = p;
                                    }
                                }
                            }
                            if (p.Y + this.Height > currentScreen.WorkingArea.Height && !walkingOnTaskbar)
                            {
                                walkingOnTaskbar = true;
                                p.Y = currentScreen.WorkingArea.Height - this.Height;
                                State = PetState.Neutral;
                                this.Location = p;
                            }
                        }
                    }
                    break;
                case PetState.Neutral:
                    {
                        State = PetState.Walking;
                        
                        int val = rnd.Next(1, 10);
                        walkingDirection = val < 5 ? -1 : 1;
                    }
                    break;
                case PetState.Walking:
                    {
                        WalkAround(walkingDirection);
                        if (WalkOnMultipleScreens)
                        {
                            LeaveDirection direction = LeavingScreen();
                            if (direction != LeaveDirection.None)
                            {
                                ChangeScreen(direction);
                                ResetState();
                            }
                        }
                        else
                        {
                            if(HitScreenBounds())
                            {
                                walkingDirection *= -1;
                            }
                        }
                    }
                    break;
                case PetState.Dragging:
                    break;
                case PetState.Sitting:
                    return;
            }
        }
        private void ResetState()
        {
            currentWindowHandle = IntPtr.Zero;
            colidedWindowHandle = IntPtr.Zero;
            walkingOnTaskbar = false;
            currentWindow = new NativeMethods.RECT();
            State = PetState.Falling;
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
                State = PetState.Dragging;
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ResetState();
            }
            if(e.Button == MouseButtons.Right)
            {
                Stay();
            }
        }
        Point precpos;
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            Size delta = new Size(e.X - precpos.X, e.Y - precpos.Y);
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                this.BringToFront();
                this.Location += delta;
                precpos = e.Location - delta;
            }
            else
            {
                precpos = e.Location;
                if (this.Location.Y < 0)
                {
                    this.Location = new Point(this.Location.X, 0);
                }
            }
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
        }
    }
}
