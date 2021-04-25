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
    public partial class MainForm : Form, IPetInteractor
    {
        PetForm pet = new PetForm();
        PluginLoader loader = new PluginLoader();
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

        public PetInfo GetPetInfo()
        {
            return pet.Info;
        }

        public Point GetPetLocation()
        {
            return pet.GetPetLocation();
        }

        public Rectangle GetPetBounds()
        {
            return new Rectangle(0, 0, 10, 10);
        }

        public void SetPetLocation(Point p)
        {
            pet.SetPetLocation(p);
        }

        public void SpawnToy(Toy t)
        {
            pet.SpawnToy(t);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                IPlugin result = loader.LoadPlugin(ofd.FileName, this);
                if (result != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Loaded Plugin: {ofd.FileName}");
                    result.Initialize();
                }
            }    
        }

        public Point GetScreenPetLocation()
        {
            return pet.PetRealPosition;
        }

        public void SpawnDialog(UserControl dialog)
        {
            pet.SpawnDialog(dialog);
        }
    }
}
