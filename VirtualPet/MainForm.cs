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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
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
                if(sb.Length ==0)
                {
                    return true;
                }
                NativeMethods.RECT rect = new NativeMethods.RECT();
                NativeMethods.GetWindowRect(new System.Runtime.InteropServices.HandleRef(this, hWnd),out rect);
                Console.WriteLine($"{sb.ToString()} Maximized:{NativeMethods.IsZoomed(hWnd)}  {rect.Top} {rect.Left} {rect.Right} {rect.Bottom}");


                return true;
            }, IntPtr.Zero);

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PetForm pet = new PetForm();
            pet.Show();
            pet.Fall();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int x = 0;
            int y = 0;
            int widh = 0;
            int height = 0;
            foreach(Screen s in Screen.AllScreens)
            {
                Console.WriteLine($"{s.Bounds} {s.WorkingArea}");
                widh += s.Bounds.Width;
                if (height == 0 || s.WorkingArea.Height < height)
                    height = s.WorkingArea.Height;
                if (s.Bounds.X < x)
                    x = s.Bounds.X;
                if(s.Bounds.Y < y)
                {
                    y = s.Bounds.Y;
                }

            }
            this.Size = new Size(widh, height);
            this.Location = new Point(x, y);
            Console.WriteLine(this.Location);
            this.Refresh();
            this.Update();
        }
    }
}
