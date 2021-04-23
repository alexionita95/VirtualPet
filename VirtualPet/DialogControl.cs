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
    public partial class DialogControl : UserControl
    {
        public DialogControl()
        {
            InitializeComponent();
            DoubleBuffered = true;

           
        }
        public void LoadData()
        {
            chromiumWebBrowser1.Load("https://www.youtube.com/embed/i1gvPva38Nk?autoplay=1");
        }
    }
}
