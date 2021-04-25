using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualPet.Core
{
    public partial class Toy : UserControl
    {
        public bool OnBottom { get; set; }
        Action toyAction;
        public Toy()
        {
            InitializeComponent();
            OnBottom = false;
        }
        public Toy(Action action = null)
        {
            InitializeComponent();
            OnBottom = false;
            toyAction = action;
        }
        public void UpdateState()
        {
            if (!OnBottom)
            {
                Location = new Point(Location.X, Location.Y + 1);
            }
        }
        Point precpos;
        private void Toy_MouseMove(object sender, MouseEventArgs e)
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
        public void ExecuteAction()
        {
            if (toyAction != null)
            {
                toyAction();
            }
        }
    }
}
