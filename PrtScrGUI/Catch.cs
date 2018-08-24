using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace PrtScrGUI
{
    public partial class Catch : Form
    {
        #region 用户变量
        private Point DownPoint = Point.Empty;//记录鼠标按下坐标，用来确定绘图起点
        private bool CatchStart = false;//表示截图开始
        private Bitmap originBmp;//用来保存全屏图像
        private Bitmap Bmp;//用来保存截图图像
        private Rectangle CatchRect;//用来保存截图的矩形
        private int Circle;
        #endregion

        public Catch(int shape)
        {
            Circle = shape;
            InitializeComponent();
            originBmp = GetFullScreen();
            
        }

        public Bitmap Get_Bmp
        {
            get { return Bmp; }
        }

        private void Catch_Load(object sender, EventArgs e)
        {
            //this.ClientSize = new System.Drawing.Size(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height);
            this.Opacity = 0.3D;
        }

        private void Catch_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!CatchStart)
                {//如果捕捉没有开始
                    CatchStart = true;
                    DownPoint = new Point(e.X, e.Y);//保存鼠标按下坐标
                }
            }
        }

        //鼠标点击放开时发生的事件
        private void Catch_MouseUp(object sender, MouseEventArgs e)
        {
            if (CatchStart && CatchRect.Width != 0 && CatchRect.Height!=0)
            {
                Bmp = new Bitmap(CatchRect.Width, CatchRect.Height);//新建一个于矩形等大的空白图片
                Graphics g = Graphics.FromImage(Bmp);
                g.DrawImage(originBmp, new Rectangle(0, 0, CatchRect.Width, CatchRect.Height), CatchRect, GraphicsUnit.Pixel);
                this.Close();
            }
            CatchStart = false;
        }


        //Esc退出
        private void Catch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Escape)
            {
                this.Close();
            }
        }

        private void Catch_MouseMove(object sender, MouseEventArgs e)
        {
            if (CatchStart)
            {//如果捕捉开始
                Point newPoint = new Point(DownPoint.X, DownPoint.Y);//获取鼠标的坐标
                int width = Math.Abs(e.X - DownPoint.X);
                int height = Math.Abs(e.Y - DownPoint.Y);//获取矩形的长和宽

                Graphics g = Graphics.FromHwnd(Handle);
                Pen p = new Pen(Color.Green, 2);
                g.Clear(System.Drawing.SystemColors.Control);

                if (e.X < DownPoint.X)
                {
                    newPoint.X = e.X;
                }
                if (e.Y < DownPoint.Y)
                {
                    newPoint.Y = e.Y;
                }

                if (Circle==1) g.DrawEllipse(p, newPoint.X, newPoint.Y, width, height);//在画板上画椭圆,起始坐标为(10,10),外接圆的宽为,高为
                else g.DrawRectangle(p, newPoint.X, newPoint.Y, width, height);//在画板上画矩形,起始坐标为鼠标点击处,宽为,高为

                CatchRect = new Rectangle(newPoint, new Size(width, height));//保存矩形

                g.Dispose();
            }
        }

        /// <summary> 
        /// 截取全屏幕图像 
        /// </summary> 
        /// <returns>屏幕位图</returns>         
        public Bitmap GetFullScreen()
        {
            Bitmap mimage = new Bitmap(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            Graphics gp = Graphics.FromImage(mimage);
            gp.CopyFromScreen(new Point(System.Windows.Forms.Screen.PrimaryScreen.Bounds.X, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Y), new Point(0, 0), mimage.Size, CopyPixelOperation.SourceCopy);
            gp.Dispose();
            return mimage;
        }

        //窗体关闭时发生
        private void Catch_FormClosing(object sender, FormClosingEventArgs e)
        {
            Clipboard.SetImage(Bmp);//保存到剪切板中
            Start sta = new Start();
            sta.Bmp = Bmp;
            sta.set_name_1("新建截图");
            sta.issava.Visible = true;
            Thread newThread = new Thread(() => Application.Run(sta));
            newThread.SetApartmentState(System.Threading.ApartmentState.STA);
            newThread.Start();
        }
    }
}
