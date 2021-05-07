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
using VirtualPet.Physics;
using System.Diagnostics;
using VirtualPet.Math.Vectors;
using VirtualPet.Math.Geometry;
namespace VirtualPet
{
    public partial class MainForm : Form, IPetInteractor
    {
        PetForm pet = new PetForm();
        PluginLoader loader = new PluginLoader();
        System2 physics = new System2(new Vec2(0, 10));
        public MainForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
            renderPanel.Paint += RenderPanel_Paint;
            Circle c1 = new Circle(10);
            Circle c2 = new Circle(100);
            Rectangle2 r = new Rectangle2(renderPanel.Width/2, 20);
            Rectangle2 r2 = new Rectangle2(renderPanel.Width / 4, 20);

            //Body2 b1 = new Body2(new Transform2(new Vec2(120,150)),0.6f, c1);
            Body2 b1 = new Body2(new Transform2(new Vec2(renderPanel.Width / 2, 10), 0), 0.6f, r2);
            b1.CoR = 0.95f;
           Body2 b2 = new Body2(new Transform2(new Vec2(renderPanel.Width/2, renderPanel.Height-100), 0), 0.0f,r);
            //Body2 b2 = new Body2(new Transform2(new Vec2(150,400), 0), 0.0f, c2);
            b2.CoR = 0.4f;
            b1.Rotation = 95;
            b2.Rotation = 30;
            b2.EnableGravity = false;
            b1.EnableGravity = false;
            b1.DynamicFriction = 0.2f;
            b1.StaticFriction = 0.4f;
            b2.DynamicFriction = 0.2f;
            b2.StaticFriction = 0.4f;
            //b1.Collider = c1;
            //b2.Collider = c2;
            physics.AddBody(b1,true);
           // physics.ApplyForce(b1, new WindTest());
            physics.AddBody(b2,false);
            timer1.Start();
            
        }

        private void RenderPanel_Paint(object sender, PaintEventArgs e)
        {
            foreach(Body2 b in physics.Bodies)
            {
                RenderBody(e.Graphics, b);
            }
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
            return null;
        }

        public Point GetPetLocation()
        {
            return new Point();
        }

        public Rectangle GetPetBounds()
        {
            return new Rectangle(0, 0, 10, 10);
        }

        public void SetPetLocation(Point p)
        {
            
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
            return new Point();
        }
        private void RenderBody(Graphics gfx, Body2 body)
        {
            if(body.Shape is Circle)
            {
                Circle c = body.Shape as Circle;
                Vec2 beginning = body.Position.Sub(c.Radius);
                gfx.DrawEllipse(Pens.Red, new Rectangle((int)beginning.X, (int)beginning.Y, (int)c.Radius * 2, (int)c.Radius * 2));
            }
            if(body.Shape is Poly2)
            {
                Poly2 p = body.Shape as Poly2;
                for(int i=0;i<p.Vertices.Count;++i)
                {
                    Math.Matrix.Mat2 rot = Math.Matrix.Mat2.Rotation(body.Rotation);
                    int j = i + 1 < p.Vertices.Count ? i + 1 : 0;
                    Vec2 v1 = body.Position.Add(rot.Mul(p.Vertices[i]));
                    Vec2 v2 = body.Position.Add(rot.Mul(p.Vertices[j]));
                    Vec2 mid = v1.Add(v2).Div(2);
                    Vec2 dir =mid.Add(rot.Mul(p.Normals[i]).Mul(10));
                    gfx.DrawLine(Pens.Red, new Point((int)v1.X, (int)v1.Y), new Point((int)v2.X, (int)v2.Y));
                    gfx.DrawLine(Pens.Green, new Point((int)mid.X, (int)mid.Y), new Point((int)dir.X, (int)dir.Y));
                    gfx.FillEllipse(Brushes.Blue, new Rectangle((int)v1.X - 1, (int)v1.Y - 2, 4, 4));
                    gfx.FillEllipse(Brushes.Blue, new Rectangle((int)v2.X - 1, (int)v2.Y - 2, 4, 4));

                }
            }
        }
        public void SpawnDialog(UserControl dialog)
        {
            pet.SpawnDialog(dialog);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            physics.Update(0.016f);
            renderPanel.Invalidate();
        }
    }
}
