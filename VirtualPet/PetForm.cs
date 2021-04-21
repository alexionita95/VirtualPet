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
        public PetForm()
        {
            InitializeComponent();
            this.Visible = false;
        }

        public void Fall()
        {
            currentScreen = Screen.FromControl(this);
            int x = currentScreen.Bounds.X + currentScreen.Bounds.Width / 2 - this.Width / 2;
            int y = currentScreen.Bounds.Y;
            this.Location = new Point(x, y);
            this.Visible = true;
            animTimer.Start();
        }
        bool checkTopWindow()
        {
            if((int)currentWindowHandle == 0)
            {
                return false;
            }
            NativeMethods.RECT rctO;
            NativeMethods.RECT rct;

            NativeMethods.GetWindowRect(new System.Runtime.InteropServices.HandleRef(this, currentWindowHandle), out rctO);

            //TODO: check if window is still there

            IntPtr nextWindow = NativeMethods.GetTopWindow(currentWindowHandle);

            while(nextWindow != IntPtr.Zero)
            {
                if (NativeMethods.IsWindowVisible(nextWindow))
                {
                    NativeMethods.GetWindowRect(new System.Runtime.InteropServices.HandleRef(this, nextWindow), out rct);

                    if(ColidesWith(rct))
                    {
                        return true;
                    }
                }
                nextWindow = NativeMethods.GetTopWindow(nextWindow);
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
                if(NativeMethods.IsZoomed(hWnd))
                {
                    return true;
                }
                NativeMethods.RECT rect = new NativeMethods.RECT();
                NativeMethods.GetWindowRect(new System.Runtime.InteropServices.HandleRef(this, hWnd), out rect);
               // Console.WriteLine($"{sb.ToString()} Maximized:{NativeMethods.IsZoomed(hWnd)}  {rect.Top} {rect.Left} {rect.Right} {rect.Bottom}");
               
                if(ColidesWith(rect))
                {
                    currentWindow = rect;
                    currentWindowHandle = hWnd;
                    if (!checkTopWindow())
                    {
                        StayOnWindow(currentWindow);
                        colided = true;
                        Console.WriteLine($"Colides with: {sb.ToString()} Maximized:{NativeMethods.IsZoomed(hWnd)}");
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
                return false;
            }
            if (this.Location.Y > rect.Top && this.Location.X > rect.Left && this.Location.X < rect.Right)
            {
                Console.WriteLine($"Top:{rect.Top} Left:{rect.Left} Right:{rect.Right} Bottom:{rect.Bottom} X:{Location.X} Y:{Location.Y}");
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

        private void animTimer_Tick(object sender, EventArgs e)
        {
            Point p = this.Location;
            p.Y += 1;
            if(checkWindowCollision())
            {
                animTimer.Stop();
                return;
            }
            if(p.Y+this.Height > currentScreen.WorkingArea.Height)
            {
                p.Y = currentScreen.WorkingArea.Height - this.Height;
                animTimer.Stop();
            }
            this.Location = p;
        }
    }
}
