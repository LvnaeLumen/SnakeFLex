using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;



//using System.Linq;
using System.Text;
using System.Windows.Forms;

// для работы с библиотекой OpenGL 
using Tao.OpenGl;
// для работы с библиотекой FreeGLUT 
using Tao.FreeGlut;
// для работы с элементом управления SimpleOpenGLControl 
using Tao.Platform.Windows;

namespace Laba1
{
    

    public partial class mBox : Form
    {

        double a = 0;
        double b = 0;
        double c = -20;
        double d = 120;
        double zoom = 1;
        int axisX = 1;
        int axisY = 0;
        int axisZ = 0;
        static int n = 15;
        static int m = 15;
        double dd = 1;
        double up = 0.0;
        double down = 0.0;
        double left = 0.0;
        double right = 0.0;
        bool Wire = false;
        static double xold = -n / 2;
        static double yold = -m / 2;
        static double speed = 0.02;
        Random rnd = new Random();
        int rn;

        double applecoordsX = 100;
        double applecoordsY = 100;

        public class Coord
        {
            public double X;
            public double Y;
            public Coord(double X, double Y)
            {
                this.X = X;
                this.Y = Y;
            }
        }
        static List<Coord> Crd = new List<Coord>();



        short direction = -1; // 0 north 1 west 2 south 3 east

        

        public mBox()
        {
            InitializeComponent();
            AnT.InitializeContexts();
        }
        private void KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    if(direction!=2)
                        direction = 0;
                    speedBar.Value = 5;
                    break;
                case Keys.A:
                    if (direction != 3)
                        direction = 1;
                    speedBar.Value = 5;
                    break;
                case Keys.S:
                    if (direction != 0)
                        direction = 2;
                    speedBar.Value = 5;
                    break;
                case Keys.D:
                    if (direction != 1)
                        direction = 3;
                    speedBar.Value = 5;
                    break;
                case Keys.NumPad8:
                    if(speedBar.Value < speedBar.Maximum)
                    speedBar.Value++;
                    speedBar.Update();
                    break;
                case Keys.Space:
                    speedBar.Value = 0;
                    break;

                default:
                    break;
            };
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Glut.glutInit();
            //Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE);
            // отчитка окна 

            Gl.glClearColor(255, 255, 255, 1);

            Gl.glViewport(0, 0, AnT.Width, AnT.Height);

            
            // настройка проекции 
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(45, (float)AnT.Width / (float)AnT.Height, 0.1, 200.0);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);

            axisBox.SelectedIndex = 0;
            figureBox.SelectedIndex = 7;
            WireBox.Checked = false;

            //dd = (int)dNumericUpDown.Value;
            //n = (int)dNumericUpDown.Value;
            //m = (int)dNumericUpDown.Value;
            Coord c = new Coord(xold, yold);
            Crd.Add(c);
            timer1.Start();
        }

        private void axisBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (axisBox.SelectedIndex)
            {
                case 0:
                    { axisX = 1; axisY = 0; axisZ = 0; break; }
                case 1:
                    { axisX = 0; axisY = 1; axisZ = 0; break; }
                case 2:
                    { axisX = 0; axisY = 0; axisZ = 1; break; }
            }
        }

        private void WireBox_CheckedChanged(object sender, EventArgs e)
        {
            Wire = (WireBox.Checked);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            

            Draw();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            a = (double)trackBar1.Value / 1000.0;
            label1.Text = a.ToString();

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            b = (double)trackBar2.Value / 1000.0;
            label2.Text = b.ToString();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            c = (double)trackBar3.Value / 1000.0;
            label3.Text = c.ToString();
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            d = (double)trackBar4.Value;
            label4.Text = d.ToString();
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            zoom = (double)trackBar5.Value / 1000.0;
            label5.Text = zoom.ToString();
        }
        private void speedBar_Scroll(object sender, EventArgs e)
        {
            speed = (double)speedBar.Value / 1000000;
            label10.Text = (speedBar.Value).ToString();
        }

        private void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glClearColor(255, 255, 255, 1);
            Gl.glLoadIdentity();

            Gl.glPushMatrix();
            Gl.glTranslated(a-dd*n - Crd[0].X,
                b+ Crd[0].Y * Math.Sin((d-90)*0.0175),
                c- Crd[0].Y * Math.Cos((d-90)*0.0175));
            Gl.glRotated(d, axisX, axisY, axisZ);

            speed = speedBar.Value;
            speed = speed / 1000000;
            Gl.glScaled(zoom, zoom, zoom);

            switch (figureBox.SelectedIndex)
            {
                case 0:
                    {
                        if (Wire)
                            Glut.glutWireCylinder(1, 2, 32, 32);
                        else
                        {
                            float[] color = new float[4] { 0.02f, 0.02f, 0.3f, 1 };
                            float[] shininess = new float[1] { 10 };
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
                            Glut.glutSolidCylinder(1, 2, 32, 32);
                        }
                        break;
                    }
                case 1:
                    {
                        if (Wire)
                            Glut.glutWireSphere(2, 16, 16);
                        else
                        {
                            float[] color = new float[4] { 0.02f, 0.02f, 0.3f, 1 };
                            float[] shininess = new float[1] { 10 };
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
                            Glut.glutSolidSphere(2, 16, 16);
                        }
                        break;
                    }
                case 2:
                    {
                        if (Wire)
                            Glut.glutWireCube(2);
                        else
                        {
                            float[] color = new float[4] { 0.02f, 0.02f, 0.3f, 1 };
                            float[] shininess = new float[1] { 30 };
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
                            Glut.glutSolidCube(2);
                        }
                        break;
                    }
                case 3:
                    {
                        if (Wire)
                            Glut.glutWireCone(2, 3, 32, 32);
                        else
                        {
                            float[] color = new float[4] { 0.02f, 0.02f, 0.3f, 1 };
                            float[] shininess = new float[1] { 30 };
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
                            Glut.glutSolidCone(2, 3, 32, 32);
                        }
                        break;
                    }
                case 4:
                    {
                        if (Wire)
                            Glut.glutWireTorus(2, 3, 32, 32);
                        else
                        {
                            float[] color = new float[4] { 0.02f, 0.02f, 0.3f, 1 };
                            float[] shininess = new float[1] { 30 };
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
                            Glut.glutSolidTorus(2, 3, 32, 32);
                        }
                        break;
                    }
                case 5:
                    {
                        if (Wire)
                        {
                            Glut.glutWireCylinder(0.25, 3, 32, 32);
                            Gl.glTranslated(0, 0, 2.5);

                            Glut.glutWireCone(1, 2, 50, 32);
                            Gl.glTranslated(0, 0, 0.8);

                            Glut.glutWireCone(0.8, 2, 50, 32);
                            Gl.glTranslated(0, 0, 0.7);

                            Glut.glutWireCone(0.6, 1.5, 50, 32);
                        }
                        else
                        {
                            float[] color = new float[4] { 0.3f, 0.05f, 0.05f, 1 };
                            float[] shininess = new float[1] { 0.0f };
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
                            Glut.glutSolidCylinder(0.25, 2, 32, 32);
                            Gl.glTranslated(0, 0, 2);

                            color = new float[4] { 0.4f, 0.5f, 0.2f, 1 };
                            shininess = new float[1] { 10.0f };
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);

                            Glut.glutSolidCone(1, 2, 50, 32);
                            Gl.glTranslated(0, 0, 0.8);

                            Glut.glutSolidCone(0.8, 2, 50, 32);
                            Gl.glTranslated(0, 0, 0.7);

                            Glut.glutSolidCone(0.6, 1.5, 50, 32);
                        }
                        break;
                    }
                case 6:
                    {
                        if (Wire)
                        {
                            Glut.glutWireSphere(2, 16, 16);
                            Gl.glTranslated(0, 0, 3.5);
                            Glut.glutWireSphere(1.5, 16, 16);
                            Gl.glTranslated(0, 0, 2.5);
                            Glut.glutWireSphere(1, 16, 16);
                            Gl.glTranslated(00, -0.7, 0);
                            Gl.glRotated(90, 1, 0, 0);
                            Glut.glutWireCone(0.2, 1, 50, 32);
                            break;
                        }
                        else
                        {
                            float[] color = new float[4] { 1f, 1f, 1f, 1 };
                            float[] shininess = new float[1] { 30 };
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);

                            Glut.glutSolidSphere(2, 16, 16);
                            Gl.glTranslated(0, 0, 3);
                            Glut.glutSolidSphere(1.5, 16, 16);
                            Gl.glTranslated(0, 0, 2);
                            Glut.glutSolidSphere(1, 16, 16);
                            Gl.glTranslated(00, -0.7, 0);
                            Gl.glRotated(90, 1, 0, 0);

                            color = new float[4] { 0.3f, 0.05f, 0.05f, 1 };
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color);
                            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
                            Glut.glutSolidCone(0.2, 1, 50, 32);
                            break;
                        }
                    }
                case 7:
                    {
                        float[] black = new float[4] { 0, 0, 0, 1 };
                        float[] white = new float[4] { 1, 1, 1, 1 };
                        float[] shininess = new float[1] { 30 };


                        int i, j;
                        
                      
                        if (Wire)
                        {
                            for (i = 1; i <= n; i++)
                            {

                                Gl.glTranslated(0, (dd * m), 0);
                                for (j = m; j >= 1; j--)
                                {
                                    if ((i + j) % 2 == 0)
                                    {
                                        Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, white);
                                        Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, white);
                                    }
                                    else
                                    {
                                        Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, black);
                                        Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, black);
                                    }

                                    Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
                                    Glut.glutWireCube(dd);
                                    Gl.glTranslated(0, -dd, 0);
                                }
                            }
                        }
                        else
                        {
                            for (i = 1; i <= n; i++)
                            {

                                Gl.glTranslated(dd, (-dd * m), 0);
                                for (j = m; j >= 1; j--)
                                {
                                    if ((i + j) % 2 == 0)
                                    {
                                        Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, white);
                                        Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, white);
                                        Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
                                    }
                                    else
                                    {
                                        Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, black);
                                        Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, black);
                                        Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
                                    }
                                    Glut.glutSolidCube(dd);
                                    Gl.glTranslated(0, dd, 0);
                                }
                            }
                        }

                        label14.Text = applecoordsX.ToString();
                        label14.Refresh();
                        label15.Text = applecoordsY.ToString();
                        label15.Refresh();
                        DrawSnake();
                        DrawApple();
                        break;
                    }
            }
            AnT.Invalidate();
        }
        private void DrawSnake()
        {
            label12.Text = (Math.Round(Crd[0].X, 1)).ToString();
            label12.Update();

            label13.Text = (Math.Round(Crd[0].Y, 1)).ToString();
            label13.Update();

            //label14.Text = direction.ToString();
            //label14.Update();            
                       
            AnT.KeyDown += new KeyEventHandler(KeyDown);
            if ((Math.Round((Crd[0].X % 1), 1) == 0) 
                && (Math.Round((Crd[0].Y % 1), 1) == 0))
            {
                switch (direction)
                {                    
                    case 0:
                        Up_Click(new object(), new EventArgs());
                       
                        break;
                    case 1:
                        LeftButton_Click(new object(), new EventArgs());
                        
                        break;
                    case 2:
                        DownButton_Click(new object(), new EventArgs());
                        
                        break;
                    case 3:
                        RightButton_Click(new object(), new EventArgs());
                        
                        break;
                }

               

               // direction = -1;
            }

            if ((Math.Round(Crd[0].X, 1) == Math.Round(-applecoordsX, 1)
                   && (Math.Round(Crd[0].Y, 1) == Math.Round(-applecoordsY, 1))))
            {
                Coord s = new Coord(-5.5, -5.5);
                Crd.Add(s);
                applecoordsX = 100;
                applecoordsY = 100;
            }

            List<Coord> buff = new List<Coord>();
            Coord b;
            bool f = true;


            float[] color1 = new float[4] { 0, 0, 1, 1 };
            float[] shininess = new float[1] { 30 };


            double dx = Crd[0].X;
            double dy = Crd[0].Y;
            Crd[0].X = (Crd[0].X + left * speedBar.Value * dd - right * speedBar.Value * dd);
            Crd[0].Y = (Crd[0].Y + up * speedBar.Value * dd - down * speedBar.Value * dd);
            dx = Crd[0].X - dx;
            dy = Crd[0].Y - dy;

            Gl.glTranslated(Crd[0].X, Crd[0].Y, -dd);
            Gl.glTranslated(left * dd - right * dd,
                            up * dd - down * dd, 0);

            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color1);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color1);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
            Glut.glutSolidSphere((double)(dd / 2), 16, 16);

            Gl.glTranslated(-left * dd + right * dd,
                            -up * dd + down * dd, 0);
            Gl.glTranslated(-Crd[0].X, -Crd[0].Y, dd);



            double ddx = 0;
            double ddy = 0;
            for (int i = 1; i< Crd.Count; i++)
            {
                color1 = new float[4] { 1, 0, 1, 1 };

                ddx = Crd[i - 1].X - Crd[i].X;
                ddy = Crd[i - 1].Y - Crd[i].Y;
                if ((ddx - dd) > (dd / 10))
                    Crd[i].Y -= dy;

                if ((ddx - dd) < (-dd / 10))
                    Crd[i].Y += dy;

                if ((ddy - dd) > (dd / 10))
                    Crd[i].X -= dx;

                if ((ddy - dd) < (-dd / 10))
                    Crd[i].X += dx;

                Gl.glTranslated(Crd[i].X, Crd[i].Y, -dd);
                Gl.glTranslated(left * dd - right * dd,
                                up * dd - down * dd, 0);

                Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color1);
                Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color1);
                Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
                Glut.glutSolidSphere((double)(dd / 2), 16, 16);                

                Gl.glTranslated(-left * dd + right * dd,
                                -up * dd + down * dd, 0);
                Gl.glTranslated(-Crd[i].X, -Crd[i].Y, dd);
                
                //b = new Coord((c.X + left * speedBar.Value * dd - right * speedBar.Value * dd),
                //   (c.Y + up * speedBar.Value * dd - down * speedBar.Value * dd));
          
            }

            /*
            if (Crd[0].X <= -n)
                Crd[0].X += n + 1;
            if (Crd[0].X >= 1)
                Crd[0].X -= n + 1;

            if (Crd[0].Y <= -m - 1)
                Crd[0].Y += m;
            if (Crd[0].Y > -1)
                Crd[0].Y -= m;*/

        }
        private void DrawApple()
        {           

            float[] color1 = new float[4] { 1, 0, 0, 1 };
            float[] color2 = new float[4] { 0, 0, 0, 1 };
            float[] color3 = new float[4] { 0, 1, 0, 1 };
            float[] shininess = new float[1] { 30 };
            if (applecoordsX == 100)
            {
                applecoordsX = rnd.Next(1, n);
                applecoordsY = rnd.Next(1, m);
            }
            Gl.glTranslated(-applecoordsX, -applecoordsY, -dd);
            Gl.glTranslated(left * dd - right * dd,
                            up * dd  - down * dd, 0);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color1);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color1);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
            Glut.glutSolidSphere((double)(dd / 2), 16, 16);
            Gl.glTranslated(0, 0, -dd);            
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color2);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color2);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
            Glut.glutSolidCylinder((double)(dd * 0.05), (double)(dd * 0.7), 8, 8);
            Gl.glTranslated(0, -dd * 0.5, dd * 0.3);
            Gl.glRotated(30, 0, 1, 0);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color3);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color3);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
            Glut.glutSolidCylinder((double)(dd * 0.5), (double)(dd * 0.1), 4, 1);


            

        }

        private void DrawSnakeSegment(Coord _c)
        {

            
            // Gl.glTranslated(0, 0, -dd);
            /*
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color2);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color2);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
            Glut.glutSolidCylinder((double)(dd * 0.05), (double)(dd * 0.7), 8, 8);
            Gl.glTranslated(0, -dd * 0.5, dd * 0.3);
            Gl.glRotated(30, 0, 1, 0);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color3);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color3);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
            Glut.glutSolidCylinder((double)(dd * 0.5), (double)(dd * 0.1), 4, 1);*/
        }

        private void LeftButton_Click(object sender, EventArgs e)
        {          
            up = 0;
            down = 0;
            left = 0;
            right = 0.01;
        }
        private void RightButton_Click(object sender, EventArgs e)
        {
            up = 0;
            down = 0;
            left = 0.01;
            right = 0;
        }
        
        private void Up_Click(object sender, EventArgs e)
        {
            
            up = 0;
            down = 0.01;
            left = 0;
            right = 0;
        }

        private void DownButton_Click(object sender, EventArgs e)
        {
            up = 0.01;
            down = 0;
            left = 0;
            right = 0;
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            speedBar.Value = 0;
            speedBar.Update();
        }

        private void Append_Click(object sender, EventArgs e)
        {
            //dd = (int)dNumericUpDown.Value;
            //n = (int)nNumericUpDown.Value;
           // m = (int)mNumericUpDown.Value;

        }

        private void ToCenter_Click(object sender, EventArgs e)
        {
            Crd[0].X = Math.Round((double)( m / 2),1);
            Crd[0].Y = Math.Round((double)( n / 2), 1);
        }


        private void nTextBox_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(nTextBox.Text, out n);
        }

        private void mTextBox_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(mTextBox.Text, out m);
        }

        private void dTextBox_TextChanged(object sender, EventArgs e)
        {
            Double.TryParse(dTextBox.Text, out dd);
        }

        private void AnT_Load(object sender, EventArgs e)
        {
            
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            DialogResult res = openFileDialog1.ShowDialog();
        }

        private void AnT_Load_1(object sender, EventArgs e)
        {

        }
    }
}