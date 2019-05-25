using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;



//using System.Linq;
using System.Text;
using System.Windows.Forms;

// для работы с библиотекой OpenGL 
using Tao.DevIl;
using Tao.OpenGl;
// для работы с библиотекой FreeGLUT 
using Tao.FreeGlut;
// для работы с элементом управления SimpleOpenGLControl 
using Tao.Platform.Windows;

namespace Laba1
{

    
    public partial class mBox : Form
    {
        public class Coord
        {
            public double X;
            public double Y;
            public int D;
            public Coord(double X, double Y, int D)
            {
                this.X = X;
                this.Y = Y;
                this.D = D;
            }
        }
        double ScreenW;
        double ScreenH;
        private float devX;
        private float devY;
        private float[,] GrapValuesArray;
        private int elements_count = 0;
        private bool not_calculate = true;
        private int pointPosition = 0;
        float lineX, lineY;
        float Mcoord_X = 0;
        float Mcoord_Y = 0;

        double wave = 0.0;
        double counter = 0.0;
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
        static double speed = 0.015;
        double dx = 0;
        double dy = 0;
        bool flex = false;
        public string texture_name = "";
        public int imageId = 0;
        public uint mGlTextureApple = 0;
        public uint mGlTextureStick = 0;
        public uint mGlTextureLeave = 0;
        public uint mGlTextureFloor = 0;
        public uint mGlTextureSnake = 0;
        bool swtch = false;

        bool appleEated = false;
        bool nodePassed = false;


        Random rnd = new Random();
        

      

        double applecoordsX = 100;
        double applecoordsY = 100;

       
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
                    if(Crd[0].D != 2)
                        direction = 0;
                    speedBar.Value = 5;
                    break;
                case Keys.A:
                    if (Crd[0].D != 3)
                        direction = 1;
                    speedBar.Value = 5;
                    break;
                case Keys.S:
                    if (Crd[0].D != 0)
                        direction = 2;
                    speedBar.Value = 5;
                    break;
                case Keys.D:
                    if (Crd[0].D != 1)
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
            Il.ilInit();
            Il.ilEnable(Il.IL_ORIGIN_SET);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE);
            // очистка окна          

            Gl.glClearColor(1f, 1f , 1f, 1);

            Gl.glViewport(0, 0, AnT.Width, AnT.Height);
            axisBox.SelectedIndex = 0;
            figureBox.SelectedIndex = 7;
            speedBar.Enabled = false;
            // настройка проекции 
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
           /* if((float)AnT.Width <= (float)AnT.Height)
            {
                ScreenW = 30.0;
                ScreenH = 30.0 * (float)AnT.Height / (float)AnT.Width;
                
            }
            else
            {
                ScreenW = 30.0 * (float)AnT.Width / (float)AnT.Height;
                ScreenH = 30.0;
            }*/
            
            Glu.gluPerspective(45, (float)AnT.Width / (float)AnT.Height, 0.1, 200.0);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);



            WireBox.Checked = false;

            //dd = (int)dNumericUpDown.Value;
            //n = (int)dNumericUpDown.Value;
            //m = (int)dNumericUpDown.Value;
            Coord a = new Coord(xold, yold, -1);
            //Coord b = new Coord(xold, yold+dd, -1);
            //Coord c = new Coord(xold, yold+dd+dd, -1);
            Crd.Add(a);
            //Crd.Add(b);
            //Crd.Add(c);

            AnT.KeyDown += new KeyEventHandler(KeyDown);
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
            if (wave >= 1.0)
                swtch = true;
            if (wave <= -1.0)
                swtch = false;

            if (swtch)
                wave -= 0.02;
            else
                wave += 0.02;
            moved.Checked = not_calculate;
            counter += 1.0;
            counter %= 250.0;
            label21.Text = counter.ToString();
            label21.Refresh();

            label19.Text = Mcoord_X.ToString(); 
            label19.Refresh();
            label20.Text = Mcoord_Y.ToString();
            label20.Refresh();

            label18.Text = wave.ToString();
            label18.Refresh();
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
           // speed = (double)speedBar.Value / 1000000;
            label10.Text = (speedBar.Value).ToString();
        }
        private double DistanceFromHead(double x, double y)
        {
            return Math.Sqrt(Math.Pow(Crd[0].X - x, 2) + Math.Pow(Crd[0].Y - y, 2));
        }

        private void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            if(flex)
                Gl.glClearColor(0.5f+0.8f*(float)Math.Sin(180*wave*wave*wave*0.0175),
                                0.5f+0.8f*(float)Math.Sin((90 + 180 * wave*wave*wave) * 0.0175),
                                0.5f+0.8f*(float)Math.Sin((180 + 180 * wave*wave*wave) * 0.0175), 1);
            else
                Gl.glClearColor((0.2f), (0.1f), (0.4f), 1);

            Gl.glLoadIdentity();

            Gl.glPushMatrix();
           
                Gl.glTranslated(a - dd * n - Crd[0].X,
                    b + Crd[0].Y * Math.Sin((d - 90) * 0.0175),
                    c - Crd[0].Y * Math.Cos((d - 90) * 0.0175));


            Gl.glRotated(d, axisX, axisY, axisZ);
            //Glut.glutSwapBuffers();
            // speed = speedBar.Value;
            // speed = speed / 1000000;
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
                        float[] black;
                        float[] white;
                        if (flex)
                        {
                            black = new float[4] { 0.3f+(float)wave, 0.0f, 0.3f-(float)wave, 0.6f };
                            white = new float[4] { 1.4f-(float)wave, 1.2f*(float)wave, 1.7f+(float)wave, 0.8f };
                        }
                        else
                        {
                            black = new float[4] { 0.3f, 0.0f, 0.3f, 0.6f };
                            white = new float[4] { 1.4f, 1.2f, 1.7f, 0.8f };
                        }
                        float[] shininess = new float[1] { 30f };
                        float[] ambient = {0.5f, 0.02f, 0.6f, 1.0f};
                        Gl.glEnable(Gl.GL_BLEND);
                        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
                        Gl.glShadeModel(Gl.GL_SMOOTH);
                        Gl.glEnable(Gl.GL_LIGHTING);
                        Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, ambient);
                        Gl.glEnable(Gl.GL_COLOR_MATERIAL);
                        Gl.glColorMaterial(Gl.GL_FRONT, Gl.GL_AMBIENT);

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
                            for (i = n; i >= 1; i--)
                            {                                
                                Gl.glTranslated(dd, (-dd * m), 0);
                                for (j = m; j >= 1; j--)
                                {
                                    if (flex)
                                        Gl.glTranslated(0, 0, 0.2*DistanceFromHead(-i,-j));

                                    if (mGlTextureFloor != 0)
                                    {
                                        Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, white);
                                        Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, white);
                                        Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);

                                        Gl.glEnable(Gl.GL_TEXTURE_2D);
                                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureFloor);

                                        Gl.glBegin(Gl.GL_POLYGON);     
                                        Gl.glVertex3d(-dd * 0.45, -dd * 0.45, dd * 0.45);
                                        Gl.glTexCoord2f(0.0f,0.0f);
                                        Gl.glVertex3d(-dd * 0.45, dd * 0.45, dd * 0.45);
                                        Gl.glTexCoord2f(0.0f, 1.0f);
                                        Gl.glVertex3d(-dd * 0.45,dd * 0.45, -dd * 0.45);
                                        Gl.glTexCoord2f(1.0f, 1.0f);
                                        Gl.glVertex3d(-dd * 0.45, -dd * 0.45, -dd * 0.45);
                                        Gl.glTexCoord2f(1.0f, 0.0f);//cfvsq lfkmybq
                                        Gl.glEnd();
                                        Gl.glDisable(Gl.GL_TEXTURE_2D);

                                        Gl.glEnable(Gl.GL_TEXTURE_2D);
                                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureFloor);
                                        Gl.glBegin(Gl.GL_POLYGON);
                                        Gl.glVertex3d(-dd * 0.45, dd * 0.45, dd * 0.45);
                                        Gl.glTexCoord2f(0.0f, 0.0f);
                                        Gl.glVertex3d(dd * 0.45, dd * 0.45, dd * 0.45);
                                        Gl.glTexCoord2f(0.0f, 1.0f);
                                        Gl.glVertex3d(dd * 0.45, dd * 0.45, -dd * 0.45);
                                        Gl.glTexCoord2f(1.0f, 1.0f);
                                        Gl.glVertex3d(-dd * 0.45, dd * 0.45, -dd * 0.45);
                                        Gl.glTexCoord2f(1.0f, 0.0f);
                                        Gl.glEnd();
                                        Gl.glDisable(Gl.GL_TEXTURE_2D);

                                        Gl.glEnable(Gl.GL_TEXTURE_2D);
                                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureFloor);
                                        Gl.glBegin(Gl.GL_POLYGON);
                                        Gl.glVertex3d(dd * 0.45, dd * 0.45, -dd * 0.45);
                                        Gl.glTexCoord2f(0.0f, 0.0f);
                                        Gl.glVertex3d(-dd * 0.45, dd * 0.45, -dd * 0.45);
                                        Gl.glTexCoord2f(0.0f, 1.0f);
                                        Gl.glVertex3d(-dd * 0.45, -dd * 0.45, -dd * 0.45);
                                        Gl.glTexCoord2f(1.0f, 1.0f);
                                        Gl.glVertex3d(dd * 0.45, -dd * 0.45, -dd * 0.45);
                                        Gl.glTexCoord2f(1.0f, 0.0f);
                                        Gl.glEnd();
                                        Gl.glDisable(Gl.GL_TEXTURE_2D);

                                        Gl.glEnable(Gl.GL_TEXTURE_2D);
                                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureFloor);
                                        Gl.glBegin(Gl.GL_POLYGON);
                                        Gl.glVertex3d(dd * 0.45, dd * 0.45, dd * 0.45);
                                        Gl.glTexCoord2f(0.0f, 0.0f);
                                        Gl.glVertex3d(dd * 0.45, -dd * 0.45, dd * 0.45);
                                        Gl.glTexCoord2f(0.0f, 1.0f);
                                        Gl.glVertex3d(dd * 0.45, -dd * 0.45, -dd * 0.45);
                                        Gl.glTexCoord2f(1.0f, 1.0f);
                                        Gl.glVertex3d(dd * 0.45, dd * 0.45, -dd * 0.45);
                                        Gl.glTexCoord2f(1.0f, 0.0f);//cfvsq lfkmybq
                                        Gl.glEnd();
                                        Gl.glDisable(Gl.GL_TEXTURE_2D);
                                    }
                                    else
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
                                        Glut.glutSolidCube(dd * 0.9);
                                    }

                                    Gl.glTranslated(0, dd, 0);
                                    if (flex)
                                        Gl.glTranslated(0, 0, -0.2*DistanceFromHead(-i,-j));


                                }
                            }
                        }

                        if(flex)
                        {
                            
                            Gl.glMatrixMode(Gl.GL_TEXTURE);
                            Gl.glRotatef(1.0f, 0.0f, 0.0f, 1.0f);
                            Gl.glMatrixMode(Gl.GL_MODELVIEW);
                        }
                        
                        DrawSnake();
                        DrawApple();
                        label16.Text = (Crd[Crd.Count-1].X).ToString();
                        label17.Text =(Crd[Crd.Count - 1].Y).ToString();
                        label16.Refresh();
                        label17.Refresh();
                        break;
                    }
            }
            AnT.Invalidate();
        }

        private void ChangeDirection()
        {

            switch (direction)
            {
                case 0:
                    if(Crd[0].D!= 2)
                        Up_Click(new object(), new EventArgs());

                    break;
                case 1:
                    if (Crd[0].D != 3)
                        LeftButton_Click(new object(), new EventArgs());

                    break;
                case 2:
                    if (Crd[0].D != 0)
                        DownButton_Click(new object(), new EventArgs());

                    break;
                case 3:
                    if (Crd[0].D != 1)
                        RightButton_Click(new object(), new EventArgs());

                    break;
            }
        }
        private void UpdateTail()
        {          

            dx = speed*speedBar.Value * dd;
            float[] shininess = new float[1] { 1f };
            float[] color1;
            Gl.glColorMaterial(Gl.GL_FRONT, Gl.GL_AMBIENT);
            Gl.glColorMaterial(Gl.GL_FRONT, Gl.GL_AMBIENT);
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);

           

            for (int i = 1; i < Crd.Count; i++)
            {


                switch (Crd[(int)i].D)
                {
                    case 0:
                        Crd[i].Y -= dx;
                        if (flex)
                            Gl.glTranslated(Math.Sin(i+counter/10)/4,0, 0);
                        break;
                    case 1:
                        Crd[i].X -= dx;
                        if (flex)
                            Gl.glTranslated(0,-Math.Sin( i+counter/10)/4, 0);
                        break;
                    case 2:
                        Crd[i].Y += dx;
                        if (flex)
                            Gl.glTranslated(-Math.Sin(i+counter/10)/4, 0, 0);
                        break;
                    case 3:
                        Crd[i].X += dx;
                        if (flex)
                            Gl.glTranslated(0, Math.Sin(i+counter/10)/4, 0);
                        break;
                }
                if (flex)
                    Gl.glTranslated(0, 0, 0.2 * DistanceFromHead(Crd[i].X, Crd[i].Y));

                if (mGlTextureSnake != 0)
                {
                    Glu.GLUquadric quadr = Glu.gluNewQuadric();
                    Glu.gluQuadricTexture(quadr, Gl.GL_TRUE);

                    Gl.glEnable(Gl.GL_TEXTURE_2D);
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureSnake);
                    Gl.glTranslated(Crd[i].X, Crd[i].Y, -dd);
                    Gl.glRotated(180, 0, 1, 0);
                    Gl.glRotated(180, 0, 0, 1);
                    // Gl.glRotated(180-d, 0, 1, 0);
                    Glu.gluSphere(quadr, (dd / (1.8 + 0.05 * Math.Sqrt(5*i))), 16, 16);
                    //Gl.glRotated(-180+d, 0, 1, 0);
                    Gl.glRotated(180, 0, 1, 0);
                    Gl.glRotated(180, 0, 0, 1);
                    Glu.gluDeleteQuadric(quadr);
                    Gl.glDisable(Gl.GL_TEXTURE_2D);
                }
                else
                {
                    if (flex)
                    {
                        color1 = new float[4]
                       { ((33f*i*(float)wave)%255f/255f),(float)wave, (11f*i*(float)(-wave))%255f/255f, 0.7f };
                    }
                    else
                    {
                        color1 = new float[4]
                      { ((33f*i)%255f/255f),0f, (11f*i)%255f/255f, 0.7f };
                    }
                    Gl.glTranslated(Crd[i].X, Crd[i].Y, -dd);
                    //Gl.glTranslated(left * dd - right * dd,
                    //               up * dd - down * dd, 0);  
                    Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color1);
                    Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color1);


                    Glut.glutSolidSphere((double)(dd / (1.8 + 0.05 * Math.Sqrt(5 * i))), 16, 16);
                }

                    // Gl.glTranslated(-left * dd + right * dd,
                    //                -up * dd + down * dd, 0);
                    Gl.glTranslated(-Crd[i].X, -Crd[i].Y, dd);
                switch (Crd[(int)i].D)
                {
                    case 0:
                        if (flex)
                            Gl.glTranslated(-Math.Sin(i+counter/10)/4, 0, 0);
                        break;
                    case 1:
                        if (flex)
                            Gl.glTranslated(0, Math.Sin(i+counter/10)/4, 0);
                        break;
                    case 2:
                        if (flex)
                            Gl.glTranslated(Math.Sin(i+counter/10)/4, 0, 0);
                        break;
                    case 3:
                        if (flex)
                            Gl.glTranslated(0, -Math.Sin(i+counter/10)/4, 0);
                        break;
                }
                if (flex)
                    Gl.glTranslated(0, 0, -0.2 * DistanceFromHead(Crd[i].X, Crd[i].Y));

            }
           
        }

        private void UpdateHead()
        {
            float[] shininess = new float[1] { 1f };
            float[] color1 = new float[4] { 0.9f, 0.9f, 0.9f, 0.7f };
            Gl.glColorMaterial(Gl.GL_FRONT, Gl.GL_AMBIENT);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color1);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color1);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);


            if (mGlTextureSnake != 0)
            {
                Glu.GLUquadric quadr = Glu.gluNewQuadric();
                Glu.gluQuadricTexture(quadr, Gl.GL_TRUE);

                Gl.glEnable(Gl.GL_TEXTURE_2D);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureSnake);

                Gl.glTranslated(Crd[0].X, Crd[0].Y, -dd);
                Gl.glRotated(180, 0, 1, 0);
                Gl.glRotated(180, 0, 0, 1);
                // Gl.glRotated(180-d, 0, 1, 0);
                Glu.gluSphere(quadr, (dd / 1.8), 16, 16);
                //Gl.glRotated(-180+d, 0, 1, 0);
                Gl.glRotated(180, 0, 1, 0);
                Gl.glRotated(180, 0, 0, 1);
                Glu.gluDeleteQuadric(quadr);
                Gl.glDisable(Gl.GL_TEXTURE_2D);
            }
            else
            {                

                Gl.glTranslated(Crd[0].X, Crd[0].Y, -dd);
                Glut.glutSolidSphere((double)(dd / 1.8), 16, 16); //
            }

            

            // Gl.glTranslated(-left * dd + right * dd,
            //               -up * dd + down * dd, 0);


            dx = left * speedBar.Value * dd - right * speedBar.Value * dd; //движение
            dy = up * speedBar.Value * dd - down * speedBar.Value * dd;

            //dx = 0;
            //dy = 0;
            Crd[0].X = (Crd[0].X + dx); //голова
            Crd[0].Y = (Crd[0].Y + dy);
 
        
            Gl.glTranslated(-Crd[0].X, -Crd[0].Y, dd);

        }
        private void BorderHandler()
        {
            if (Crd[0].X < -n) //корректировка головы относительно границ поля
                 Crd[0].X += (double)n;
             if (Crd[0].X > 0.7)
                 Crd[0].X -= n;
            if (Crd[0].Y < -m-0.7)
                 Crd[0].Y += m;
            if (Crd[0].Y > -0.7)
                 Crd[0].Y -= m;         

            for (int i = 1; i < Crd.Count; i++) //корректировка хвоста относительно стенок и проверка проигрыша
            {
                //съели хвост
                if ((Math.Abs(Crd[i].X - Crd[0].X) < dd / 10) && (Math.Abs(Crd[i].Y - Crd[0].Y) < dd / 10))
                {
                    Crd.Clear(); //сброс коллекции координат змейки и помещение головы в центр экрана
                    Coord c = new Coord(-m / 2, -n / 2, -1);
                    Crd.Add(c);
                }
                else //проверяем границы и корректируем
                {
                    if (Crd[i].X < -n)
                        Crd[i].X += (double)n;
                    if (Crd[i].X > 0.7)
                        Crd[i].X -= (double)n;
                    if (Crd[i].Y < -m-0.7)
                        Crd[i].Y += (double)m;
                    if (Crd[i].Y > -0.7)
                        Crd[i].Y -= m;
                }
            }
        }
        private void AppleEated()
        {
            if ((Math.Abs(Crd[0].X + applecoordsX) < (dd / 4))
                  && (Math.Abs(Crd[0].Y + applecoordsY) < (dd / 4)))
            {               
                applecoordsX = 100;
                applecoordsY = 100;
                appleEated = true;
            }
        }

        private void Syncro()
        {

            //Crd[0].X(Math.Round((Crd[0].Y % 1.0), 1) == 0))
            if (  ( Math.Abs(Crd[0].X - Math.Round(Crd[0].X,0)) <= 0.05)
                && (Math.Abs(Crd[0].Y - Math.Round(Crd[0].Y, 0)) <= 0.05)
                && (!nodePassed))
            {
                nodePassed = true;
                for (int i = Crd.Count - 1; i > 0; i--)
                {
                    Crd[i].X = Math.Round(Crd[i].X, 0);
                    Crd[i].Y = Math.Round(Crd[i].Y, 0);
                    Crd[i].D = (Crd[i - 1].D);
                }
                if (appleEated)
                    GrowSnake();

                ChangeDirection();
                Crd[0].D = direction;

                appleEated = false;
            }   
        }
        private void GrowSnake()
        {
            double yy = Math.Round(Crd[Crd.Count-1].Y, 0);
            double xx = Math.Round(Crd[Crd.Count-1].X, 0);
            Coord s = new Coord( xx,
                                 yy,
                                   Crd[Crd.Count - 1].D);
           
           
            switch (Crd[Crd.Count - 1].D)
            {
                case 0:
                    s.Y += Math.Abs(dd);
                    break;
                case 1:
                    s.X += Math.Abs(dd);
                    break;
                case 2:
                    s.Y -= Math.Abs(dd);
                    break;
                case 3:
                    s.X -= Math.Abs(dd);
                    break;
            }
            Crd.Add(s);
        }

        private void DrawSnake()
        {
            AppleEated();
            Syncro();
            if ((Math.Abs(Crd[0].X - Math.Round(Crd[0].X)) > dd / 4)
                || (Math.Abs(Crd[0].Y - Math.Round(Crd[0].Y)) > dd / 4)
                || (Crd[0].D == -1))
                nodePassed = false;

            UpdateHead();
            UpdateTail();
            BorderHandler();


            label14.Text = (Crd[0].X).ToString(); //координаты выводятся на экран
            label15.Text =(Crd[0].Y).ToString();

            

            label14.Refresh();
            label15.Refresh();
            
        }
        private void DrawApple()
        {
            
            float[] color1 = new float[4] { 1, 0, 0, 1 };
            float[] color2 = new float[4] { 0, 0, 0, 1 };
            float[] color3 = new float[4] { 0, 1, 0, 1 };
            float[] shininess = new float[1] { 30 };
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color1);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color1);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
            if (applecoordsX == 100)
            {
                //bool collision = false;
                //do
                //{
                    applecoordsX = rnd.Next(1, n);
                    applecoordsY = rnd.Next(1, m);
                    //for (int i = 0; i < Crd.Count; i++)                    
                      //  collision |= ((Math.Abs(Crd[i].X + applecoordsX) < (dd / 2))
                        //            && (Math.Abs(Crd[i].Y + applecoordsY) < (dd / 2)));                    

               // }
                //while (collision);
            }
            Glu.GLUquadric quadr1, quadr2;
            Gl.glPushMatrix();
            if (flex)
                Gl.glTranslated(0, 0, 0.2 * DistanceFromHead(-applecoordsX, -applecoordsY));
            Gl.glTranslated(-applecoordsX, -applecoordsY, -dd);
            Gl.glTranslated(left * dd - right * dd,
                            up * dd  - down * dd, 0);
            if (mGlTextureApple != 0)
            {
                Gl.glRotated(180, 0, 1, 0);
                Gl.glRotated(180, 0, 0, 1);
                // Gl.glRotated(180-d, 0, 1, 0);
                //Gl.glRotated(-180+d, 0, 1, 0);
                
                quadr1 = Glu.gluNewQuadric();
                Glu.gluQuadricTexture(quadr1, Gl.GL_TRUE);
                Gl.glEnable(Gl.GL_TEXTURE_2D);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureApple);
                Glu.gluSphere(quadr1, (double)(dd / 2), 16, 16);
                Glu.gluDeleteQuadric(quadr1);
                Gl.glRotated(180, 0, 0, 1);

                Gl.glRotated(180, 0, 1, 0);
                Gl.glDisable(Gl.GL_TEXTURE_2D);
            }
            else
            {
                
                Glut.glutSolidSphere((double)(dd / 2), 16, 16);
            }
            

            Gl.glTranslated(0, 0, -dd);

            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color2);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color2);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
            if (mGlTextureStick != 0)
            {
                quadr2 = Glu.gluNewQuadric();
                Glu.gluQuadricTexture(quadr2, Gl.GL_TRUE);
                Gl.glEnable(Gl.GL_TEXTURE_2D);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureStick);
                Glu.gluCylinder(quadr2, (double)(dd * 0.05), (double)(dd * 0.05),(double)(dd * 0.7), 8, 8);
                Gl.glTranslated(0, -dd * 0.5, dd * 0.3);
                Glu.gluDeleteQuadric(quadr2);
                Gl.glDisable(Gl.GL_TEXTURE_2D);
            }
            else
            {
                
                Glut.glutSolidCylinder((double)(dd * 0.05), (double)(dd * 0.7), 8, 8);
            }

            Gl.glTranslated(0, -dd * 0.5, dd * 0.3);
            Gl.glRotated(30, 0, 1, 0);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color3);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color3);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
            if (mGlTextureLeave != 0)
            {
                Gl.glEnable(Gl.GL_TEXTURE_2D);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureLeave);
                Gl.glBegin(Gl.GL_POLYGON);
                Gl.glVertex3d((double)(dd * 0.5), (double)(dd * 0.9), 0.2);
                Gl.glTexCoord2f(0.0f, 0.0f);

                Gl.glVertex3d((double)(dd * 0.7), (double)(dd * 0.2), 0.2);
                Gl.glTexCoord2f(-0.7f, 0.0f);

                Gl.glVertex3d((double)(dd * -0.5), (double)(dd * -0.9), -0.2);
                Gl.glTexCoord2f(-1.0f, 1.0f);

                Gl.glVertex3d((double)(dd * -0.7), (double)(dd * 0.2), -0.2);
                Gl.glTexCoord2f(0.0f, -1.0f);
                Gl.glEnd();
                Gl.glDisable(Gl.GL_TEXTURE_2D);
                
            }
            else
            {
                
                Glut.glutSolidCylinder((double)(dd * 0.5), (double)(dd * 0.1), 4, 1);
            }
            Gl.glTranslated(0, dd, 0);
            if (flex)
                Gl.glTranslated(0, 0, -0.2 * DistanceFromHead(-applecoordsX, -applecoordsY));
            Gl.glPopMatrix();
            AnT.Invalidate();
        }

        private void LeftButton_Click(object sender, EventArgs e)
        {          
            up = 0;
            down = 0;
            left = 0;
            right = speed;
        }
        private void RightButton_Click(object sender, EventArgs e)
        {
            up = 0;
            down = 0;
            left = speed;
            right = 0;
        }
        
        private void Up_Click(object sender, EventArgs e)
        {
            
            up = 0;
            down = speed;
            left = 0;
            right = 0;
        }

        private void DownButton_Click(object sender, EventArgs e)
        {
            up = speed;
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
            Crd[0].X = -Math.Round((double)( m / 2), 3);
            Crd[0].Y = -Math.Round((double)( n / 2), 3);
        }


        private void nTextBox_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(nTextBox.Text, out n);
        }

        private void mTextBox_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(mTextBox.Text, out m);
        }
        private void DrawDiagram()
        {
            if(not_calculate)
                functionCalculation();
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex2d(GrapValuesArray[0, 0], GrapValuesArray[0, 1]);
            for (int ax = 1; ax < elements_count; ax += 2)
                Gl.glVertex2d(GrapValuesArray[ax, 0], GrapValuesArray[ax, 1]);
            Gl.glEnd();
            Gl.glPointSize(5);
            Gl.glColor3f(1f, 0, 0);
            Gl.glBegin(Gl.GL_POINTS);
            Gl.glVertex2d(GrapValuesArray[pointPosition, 0],
                GrapValuesArray[pointPosition, 1]);
            Gl.glEnd();
            Gl.glPointSize(1);
        }

        private void dTextBox_TextChanged(object sender, EventArgs e)
        {
            Double.TryParse(dTextBox.Text, out dd);
        }
        private void functionCalculation()
        {
            float x = 0;
            float y = 0;
            GrapValuesArray = new float[300, 2];
            elements_count = 0;
            for(x = -15; x<15; x+=0.1f)
            {
                y = (float)Math.Sin(x) * 3 - 1;
                GrapValuesArray[elements_count, 0] = x;
                GrapValuesArray[elements_count, 1] = y;
                elements_count++;
            }
            not_calculate = false;
        }
        private void PrintText2D(float x, float y, string e)
        {
            Gl.glRasterPos2f(x, y);
            foreach(char char_for_draw in Text)
            {
                Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_9_BY_15, char_for_draw);

            }
        }

        private void AnT_Load(object sender, EventArgs e)
        {
            
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            DialogResult res = openFileDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                Il.ilGenImages(1, out imageId);
                Il.ilBindImage(imageId);
                string ur1 = openFileDialog.FileName;
                if (Il.ilLoadImage(ur1))
                {
                    int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                    int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                    int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
                    switch (textComboBox.SelectedIndex)
                    {
                        case 0:
                            {
                                switch (bitspp)
                                {
                                    case 24:
                                        mGlTextureApple = MakeGLTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                                        break;
                                    case 32:
                                        mGlTextureApple = MakeGLTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                                        break;
                                }
                                break;
                            }
                        case 1:
                            {
                                switch (bitspp)
                                {
                                    case 24:
                                        mGlTextureStick = MakeGLTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                                        break;
                                    case 32:
                                        mGlTextureStick = MakeGLTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                                        break;
                                }
                                break;
                            }
                        case 2:
                            {
                                switch (bitspp)
                                {
                                    case 24:
                                        mGlTextureLeave = MakeGLTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                                        break;
                                    case 32:
                                        mGlTextureLeave = MakeGLTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                                        break;
                                }
                                break;
                            }
                        case 3://floor
                            {
                                switch (bitspp)
                                {
                                    case 24:
                                        mGlTextureFloor = MakeGLTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                                        break;
                                    case 32:
                                        mGlTextureFloor = MakeGLTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                                        break;
                                }
                                break;
                            }

                        case 4://Snek
                            {
                                switch (bitspp)
                                {
                                    case 24:

                                        mGlTextureSnake= MakeGLTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                                        break;
                                    case 32:
                                        mGlTextureSnake = MakeGLTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                                        break;
                                }
                                break;

                            }
                    }
                    Il.ilDeleteImages(1, ref imageId);


                }

            }
        }

        private void AnT_Load_1(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void Error_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {
            trackBar4.Value = 180;
            d = 180;
        }


        private uint MakeGLTexture(int Format,IntPtr pixels, int w, int h)
        {
            uint texObject;
            Gl.glGenTextures(1, out texObject);
            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texObject);

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE);
            switch(Format)
            {
                case Gl.GL_RGB:
                    {
                        Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, w ,h,
                            0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, pixels);
                        break;
                    }
                case Gl.GL_RGBA:
                    {
                        Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, w, h,
                            0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);
                        break;
                    }

            }
            return texObject;
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            DialogResult res = openFileDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                Il.ilGenImages(1, out imageId);
                Il.ilBindImage(imageId);
                string ur1 = openFileDialog.FileName;
                if (Il.ilLoadImage(ur1))
                {
                    int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                    int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                    int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
                    switch (textComboBox.SelectedIndex)
                    {
                        case 0:
                            {
                                switch (bitspp)
                                {
                                    case 24:
                                        mGlTextureApple = MakeGLTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                                        break;
                                    case 32:
                                        mGlTextureApple = MakeGLTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                                        break;
                                }
                                break;
                            }
                        case 1:
                            {
                                switch (bitspp)
                                {
                                    case 24:
                                        mGlTextureStick = MakeGLTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                                        break;
                                    case 32:
                                        mGlTextureStick = MakeGLTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                                        break;
                                }
                                break;
                            }
                        case 2:
                            {
                                switch (bitspp)
                                {
                                    case 24:
                                        mGlTextureLeave = MakeGLTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                                        break;
                                    case 32:
                                        mGlTextureLeave = MakeGLTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                                        break;
                                }
                                break;
                            }
                        case 3:
                            {
                                switch (bitspp)
                                {
                                    case 24:
                                        mGlTextureFloor = MakeGLTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                                        break;
                                    case 32:
                                        mGlTextureFloor = MakeGLTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                                        break;
                                }
                                break;
                            }
                        case 4:
                            {
                                switch (bitspp)
                                {
                                    case 24:
                                        mGlTextureSnake = MakeGLTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                                        break;
                                    case 32:
                                        mGlTextureSnake = MakeGLTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                                        break;
                                }
                                break;
                            }

                    }
                    Il.ilDeleteImages(1, ref imageId);


                }

            }
        }

        private void menuStrip1_ItemClicked_1(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void loadImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                Il.ilGenImages(1, out imageId);
                Il.ilBindImage(imageId);
                string ur1 = openFileDialog.FileName;
                if (Il.ilLoadImage(ur1))
                {
                    int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                    int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                    int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
                    switch (textComboBox.SelectedIndex)
                    {
                        case 0:
                            {
                                switch (bitspp)
                                {
                                    case 24:
                                        mGlTextureApple = MakeGLTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                                        break;
                                    case 32:
                                        mGlTextureApple = MakeGLTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                                        break;
                                }
                                break;
                            }
                        case 1:
                            {
                                switch (bitspp)
                                {
                                    case 24:
                                        mGlTextureStick = MakeGLTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                                        break;
                                    case 32:
                                        mGlTextureStick = MakeGLTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                                        break;
                                }
                                break;
                            }
                        case 2:
                            {
                                switch (bitspp)
                                {
                                    case 24:
                                        mGlTextureLeave = MakeGLTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                                        break;
                                    case 32:
                                        mGlTextureLeave = MakeGLTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                                        break;
                                }
                                break;
                            }
                        case 3:
                            {
                                switch (bitspp)
                                {
                                    case 24:
                                        mGlTextureFloor = MakeGLTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                                        break;
                                    case 32:
                                        mGlTextureFloor = MakeGLTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                                        break;
                                }
                                break;
                            }
                        case 4:
                            {
                                switch (bitspp)
                                {
                                    case 24:
                                        mGlTextureSnake = MakeGLTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                                        break;
                                    case 32:
                                        mGlTextureSnake = MakeGLTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                                        break;
                                }
                                break;
                            }

                    }
                    Il.ilDeleteImages(1, ref imageId);


                }

            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            trackBar1.Value = 0;
            a = (double)trackBar1.Value / 1000.0;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            trackBar2.Value = 0;
            b = (double)trackBar2.Value / 1000.0;
        }

        private void label3_Click(object sender, EventArgs e)
        {
            trackBar3.Value = -20000;
            c = (double)trackBar3.Value / 1000.0;
        }

        private void AnT_MouseMove(object sender, MouseEventArgs e)
        {
            Mcoord_X = e.X;
            Mcoord_Y = e.Y;
            lineX = devX * e.X;
            lineY = (float)(ScreenH - devY * e.Y);
        }

        private void flexx_CheckedChanged(object sender, EventArgs e)
        {
            flex = flexx.Checked;
        }
    }
}
