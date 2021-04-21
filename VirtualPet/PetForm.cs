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
        public PetForm()
        {
            InitializeComponent();
        }
    }
}
