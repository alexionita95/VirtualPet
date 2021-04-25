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
    public partial class MainForm : Form
    {
        PetForm pet = new PetForm();
        public MainForm()
        {
            InitializeComponent();
        }

        private void spawnBtn_Click(object sender, EventArgs e)
        {
            if (!pet.Visible)
            {
                pet.Show();
                pet.Fall();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pet.Visible)
            {
                Toy t = new Toy();
                pet.SpawnToy(t);
            }
        }
    }
}
