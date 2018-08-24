using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace PrtScrGUI
{
    public partial class Start : Form
    {
        #region 用户变量
        public int shape_j=0;//表示截图形状 0长方形 1椭圆形
        public Bitmap Bmp;//保存截图过来的图片
        private Point DownPoint = Point.Empty;//记录鼠标按下坐标，用来确定绘图起点

        private int shape_h = 3;//表示画图形状 0长方形 1椭圆形  2直线  3自由线
        private bool CatchStart = false;//表示画图未开始
        private bool CatchStart_only = false;//表示未选择画笔
        public Bitmap end_Bmp;//保存画图
        private Color clr=Color.Black;//保存画笔颜色
        private int Ing = 2;//保存画笔粗细
        

        private bool cut_sta = false;//表示剪切未开始
        private bool cut_sta_only = false;//表示未选择剪切
        private Rectangle cut_reg ;//表示剪切形状 0长方形 1椭圆形

        private bool move_sta = false;//表示移动未开始
        private bool move_sta_only = false;//表示未选择移动功能
        private Point pic_Point = Point.Empty;//记录鼠标按下坐标，用来确定图片起始位置

        private bool eraser_sta = false;//表示橡皮擦未开始
        private bool eraser_sta_only = false;//表示未选择橡皮擦功能
        //private int eraser_syl = 0;//橡皮擦类型

        private Point sta_h = Point.Empty;//保存鼠标移动的上一次坐标
        //private Point end_h = Point.Empty;

        Parameters par;//用于窗体之间传参数
        string ofd_FileName;//用于覆盖打开的图片的路径
        //ImageFormat ofd_format;//用于保存打开图片的格式

        private bool word_sta = false;//表示未选择写字功能
        private bool word_sta_only = false;//表示未选择写字功能
        private Font word_font;//表示选定的字体
        private Point word_loca = Point.Empty;//保存写字的坐标

        private bool Reduce_sta_only = false;//表示未选择缩小放大功能

        private Bitmap[] save_bmp = new Bitmap[20];//保存历史图片，提供后退功能
        private int save_int=-1;//保存后退步骤，以供是否设定前进为空或者有值

        private bool not_edit = false;//是否进入不可编辑模式
        #endregion

        public Start()
        {
            InitializeComponent();
            toolStrip1.Visible = false;
        }
        public Start(string[] args)
        {
            InitializeComponent();
            toolStrip1.Visible = false;
            if (args.Length != 0)
            {
                ofd_FileName = args[args.Length-1];
                Open();
            }
        }

        private void 长方形ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            shape_j = 0;
            printscreen(shape_j);
        }

        private void 圆形ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            shape_j = 1;
            printscreen(shape_j);
        }

        //新建空白图片
        private void 新建ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Newbmp newform = new Newbmp(0);
            this.Visible = false;
            newform.ShowDialog();
            par = newform.Get_par();
            newform.Dispose();
            this.Visible = true;

            if (!par.IsEmpty())
            {
                Bmp = new Bitmap(par.Wid, par.Heg);//会出现透明的
                if (par.Trfs)
                {
                    Graphics g = Graphics.FromImage(Bmp); // 根据新建的 Bitmap 位图，创建画布
                    g.Clear(Color.White);
                    g.Dispose();
                }
                end_Bmp = (Bitmap)Bmp.Clone();
                set_name_1("新建图片");
                issava.Visible = true;
                frist_run();
            }
        }

        //打开图片文件
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Png文件 (*.png)|*.png|Gif文件 (*.gif)|*.gif|Ico文件 (*.ico)|*.ico|Bmp文件 (*.bmp)|*.bmp|Jpg文件 (*.jpg)|*.jpg|All files (*.*)|*.*"; //过滤文件类型
            ofd.InitialDirectory = Application.StartupPath + "C:\\Users\\Administrator\\Desktop";//设定初始目录
            ofd.ShowReadOnly = true; //设定文件是否只读
            DialogResult data = ofd.ShowDialog();
            if (data == DialogResult.OK)
            {
                ofd_FileName = ofd.FileName;
                ofd.Dispose();
                Open();
            }
        }

        private void Open()
        {
            if (ofd_FileName.Substring(ofd_FileName.LastIndexOf(".") + 1).ToLower().Equals("gif")) run_gif(ofd_FileName);
            Bitmap Bmp_ofd = new Bitmap(ofd_FileName);//不直接使用pbImg.Image = Image.FormFile(ofd.FileName)是因为这样会让图片一直处于打开状态，也就无法保存修改后的图片

            set_name_1(str_name(ofd_FileName));
            issava.Visible = false;
            Bmp = new Bitmap(Bmp_ofd.Width, Bmp_ofd.Height);
            Graphics g = Graphics.FromImage(Bmp);

            g.DrawImage(Bmp_ofd, 0, 0, Bmp_ofd.Width, Bmp_ofd.Height);//将图片画到画板上
            g.Dispose();//释放画板所占资源

            end_Bmp = (Bitmap)Bmp.Clone();

            Bmp_ofd.Dispose();
            frist_run();
        }

        //开始截图
        private void printscreen(int c_shape)
        {
            this.Close();
            Catch cth = new Catch(c_shape);
            Thread newThread = new Thread(() => Application.Run(cth));
            newThread.SetApartmentState(System.Threading.ApartmentState.STA);
            newThread.Start();
        }

        private void Start_Shown(object sender, EventArgs e)
        {
            frist_run();
            this.picBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.picBox_MouseWheel);
        }

        private void frist_run()//第一次运行窗体，进行初始化设置
        {
            if (Bmp == null)
            {
                picBox.Size = new Size(1, 1);
                this.ClientSize = new Size(260, 31);
                toolStrip1.Visible = false;
                name_1.Visible = false;
                issava.Visible = false;
            }
            else
            {
                save_bmp_set(save_bmp, Bmp);
                toolStrip1.Visible = true;
                清除图片ToolStripMenuItem.Visible = true ;
                set_size();
            }
            Point loca = new Point((int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - this.Size.Width) / 2, (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - this.Size.Height) / 2);
            this.Location = loca;//设置窗体显示位置

            picBox.Image = Bmp;
        }

        //鼠标滚轮事件
        private void picBox_MouseWheel(object sender, MouseEventArgs e)
        {
            ////判断是否已安装带滚轮的鼠标
            //SystemInformation.MouseWheelPresent.ToString();
            ////获取鼠标滚轮在滚动时所获得的行数
            //SystemInformation.MouseWheelScrollLines.ToString();
            ////判断该操作系统是否支持滚轮鼠标
            //SystemInformation.NativeMouseWheelSupport.ToString();
            if (!Reduce_sta_only) return;
            if (!not_edit)
            {
                MessageBox.Show("放大缩小，将进入不可编辑模式！");
                not_edit = true;
                edit(not_edit);
            }
            if (e.Delta > 0)
            {
                picBox.Size = new Size((int)(picBox.Size.Width * 1.1), (int)(picBox.Size.Height * 1.1));
            }
            else
            {
                picBox.Size = new Size((int)(picBox.Size.Width * 0.9), (int)(picBox.Size.Height * 0.9));
            }

            int wid = picBox.Width; int heg = picBox.Height;
            if (wid < 400) wid = 400;
            if (heg < 280) heg = 280;
            this.ClientSize = new Size(wid + 100, heg + 120);
            picBox.Location = new Point((wid + 100 - picBox.Width) / 2, (heg + 120 - picBox.Height) / 2 + 20);
        }

        private void 退出不可编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            not_edit = false;
            edit(not_edit);
            set_size();
        }

        void edit(bool edit)
        {
            if (edit)
            {
                toolStrip1.Visible = false;
                退出不可编辑ToolStripMenuItem.Visible = true;
            }
            else
            {
                toolStrip1.Visible = true;
                退出不可编辑ToolStripMenuItem.Visible = false;
            }
        }

        //鼠标按下左键---picBox
        private void picBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                //name_1.Text = e.Delta.ToString() + "滚轮键";
            }

            if (e.Button == MouseButtons.Left)
            {
                //Graphics g = Graphics.FromHwnd(this.picBox.Handle);
                if (!CatchStart && CatchStart_only)
                {//如果捕捉没有开始
                    CatchStart = true;
                    DownPoint = new Point(e.X, e.Y);//保存鼠标按下坐标
                }

                if (!cut_sta && cut_sta_only)
                {//如果捕捉没有开始
                    cut_sta = true;
                    DownPoint = new Point(e.X, e.Y);//保存鼠标按下坐标
                }

                if (!move_sta && move_sta_only)
                {//如果捕捉没有开始
                    move_sta = true;
                    DownPoint = new Point(e.X, e.Y);//保存鼠标按下坐标
                    pic_Point = picBox.Location;//保存鼠标按下坐标,图片起始坐标
                }

                if (!eraser_sta && eraser_sta_only)
                {//如果捕捉没有开始
                    eraser_sta = true;
                    DownPoint = new Point(e.X, e.Y);//保存鼠标按下坐标
                }

                //必须放在前面，不然会出现字体坐标不在上次编写位置
                if (text_word.Text != "" && text_word.Visible == true)
                {
                    text_word.Visible = false;
                    Graphics g = Graphics.FromImage(Bmp);
                    SolidBrush brush = new SolidBrush(Color.Black);
                    g.DrawString(text_word.Text, word_font, brush, word_loca);	//在指定位置并且用指定的 Brush 和 Font 对象绘制指定的文本字符串。
                    //DrawString(String, Font, Brush, RectangleF)	//在指定矩形并且用指定的 Brush 和 Font 对象绘制指定的文本字符串。
                    g.Dispose();
                    picBox.Image = Bmp;
                    save_bmp_set(save_bmp, Bmp);
                    text_word.Text = "";
                }

                if (!word_sta && word_sta_only)
                {
                    word_sta = true;
                    text_word.Visible = true;
                    text_word.Location = new Point(e.X+50, e.Y+80);
                    word_loca = new Point(e.X, e.Y);//保存鼠标按下坐标
                }

            }
        }

        //鼠标移动---picBox
        private void picBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (CatchStart||cut_sta||eraser_sta)
            {//如果捕捉开始           
                Point newPoint = new Point(DownPoint.X, DownPoint.Y);//获取鼠标的坐标
                Point e_pnt = e.Location;
                

                if (shape_h == 3 || eraser_sta_only)
                {
                    if (end_Bmp == null) end_Bmp = (Bitmap)Bmp.Clone();
                    Graphics g_bmp_end = Graphics.FromImage(end_Bmp);

                    //SolidBrush myBrush = new SolidBrush(Color.White);//画刷
                    //g_bmp_end.FillEllipse(myBrush, new Rectangle(e.X - Ing / 2, e.Y - Ing / 2, Ing, Ing));

                    if (!sta_h.IsEmpty) 
                    {
                        Pen p=new Pen(clr, Ing);;
                        if (eraser_sta_only) p.Color=Color.White;
                        p.StartCap = LineCap.Round;
                        p.EndCap = LineCap.Round;//将线帽样式设为圆线帽，否则笔宽变宽时会出现明显的缺口 
                        g_bmp_end.DrawLine(p, sta_h, e_pnt);
                        p.Dispose();
                    }

                    sta_h = e.Location;//保存此次画图结束点

                    g_bmp_end.Dispose();
                }

                else end_Bmp = StripMenuItem(newPoint, e_pnt, shape_h);

                //Graphics g_pic = Graphics.FromHwnd(this.picBox.Handle);

                //g_pic.DrawImage(end_Bmp, new Point(0, 0));

                //g_pic.Dispose();
                this.picBox.Image = end_Bmp;
                
            }
            if (move_sta)//进行控件移动
            {//如果捕捉开始
                picBox.Location = new Point(picBox.Location.X + e.X - DownPoint.X, picBox.Location.Y + e.Y - DownPoint.Y);
            }
        }

        //鼠标释放左键---picBox
        private void picBox_MouseUp(object sender, MouseEventArgs e)
        {
            if ((CatchStart || eraser_sta || word_sta) && end_Bmp != null)
            {//如果捕捉结束 
                Graphics g_bmp = Graphics.FromImage(Bmp);
                g_bmp.DrawImage(end_Bmp, new Point(0, 0));
                g_bmp.Dispose();
                save_bmp_set(save_bmp, Bmp);
                issava.Visible = true;
            }
            end_Bmp = null;
            sta_h = Point.Empty;
            CatchStart = false;
            cut_sta = false;
            move_sta = false;
            eraser_sta = false;
            word_sta = false;
        }

        //复制到剪切板
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(Bmp);//保存到剪切板中
        }

        //将剪切板的内容粘贴过来
        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bmp=(Bitmap)Clipboard.GetImage();
            end_Bmp = (Bitmap)Bmp.Clone();
            set_name_1("粘贴图片");
            issava.Visible = true;
            frist_run();
        }

        //剪切到剪切板
        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(Bmp);//保存到剪切板中
            Bmp = null;
            清除图片ToolStripMenuItem.Visible = false;
            frist_run();
        }

        //清除pic图片内容
        private void 清除图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bmp = null;
            清除图片ToolStripMenuItem.Visible = false;
            frist_run();
        }

        //选择画图样式
        private Bitmap StripMenuItem(Point newPoint, Point e, int shape_se)
        {
            Bitmap newbmp = (Bitmap)Bmp.Clone();
            Graphics g = Graphics.FromImage(newbmp);
            Pen p = new Pen(clr, Ing);

            if (cut_sta) p.DashStyle = DashStyle.Dot;//如果是剪切，则画图用虚线，定义虚线的样式为点

            
            p.StartCap = LineCap.Round;
            p.EndCap = LineCap.Round;//将线帽样式设为圆线帽，否则笔宽变宽时会出现明显的缺口  
  
            Point change = newPoint;

            int width = Math.Abs(e.X - newPoint.X);
            int height = Math.Abs(e.Y - newPoint.Y);

            if (e.X < newPoint.X)
            {
                change.X = e.X;
            }
            if (e.Y < newPoint.Y)
            {
                change.Y = e.Y;
            }

            switch (shape_se)
            {
                case 2: g.DrawLine(p, newPoint.X, newPoint.Y, e.X, e.Y); //在画板上画直线
                    break;
                case 0: 
                    g.DrawRectangle(p, change.X, change.Y, width, height); //在画板上画长方形
                    cut_reg = new Rectangle(change, new Size(width, height));//保存剪切矩形
                    break;
                case 1: 
                    g.DrawEllipse(p, change.X, change.Y, width, height); //在画板上画椭圆形
                    cut_reg = new Rectangle(change, new Size(width, height));//保存剪切矩形
                    break;
                default: break;
            }
            g.Dispose();
            p.Dispose();
            return newbmp;
        }

        //放大缩小
        private void toolStripButton5_ButtonClick(object sender, EventArgs e)
        {
            this.picBox.Focus();//获取焦点
            set_only(6);//当鼠标的一种功能存在时（移动 画图 剪切 橡皮擦），则关闭其他功能
            set_cursor(global::PrtScrGUI.Properties.Resources.zoom);
        }

        private void 放大ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Size size = Bmp.Size;
            size.Height = (int)(((double)size.Height) * 1.1);
            size.Width = (int)(((double)size.Width) * 1.1);
            Bmp = KiResizeImage(Bmp, size);//直接设置尺寸

            //float xDpi, yDpi;
            //xDpi = Bmp.HorizontalResolution * (float)1.1;
            //yDpi = Bmp.VerticalResolution * (float)1.1;
            //Bmp.SetResolution(xDpi, yDpi);//从像素设置大小

            set_size();
            picBox.Image = Bmp;
            save_bmp_set(save_bmp, Bmp);
            issava.Visible = true;
        }
        
        private void 缩小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Size size = Bmp.Size;
            size.Height = (int)(((double)size.Height) * 0.9);
            size.Width = (int)(((double)size.Width) * 0.9);
            Bmp = KiResizeImage(Bmp, size);//直接设置尺寸

            //float xDpi, yDpi;
            //xDpi = Bmp.HorizontalResolution * (float)0.9;
            //yDpi = Bmp.VerticalResolution * (float)0.9;
            //Bmp.SetResolution(xDpi, yDpi);//从像素设置大小

            set_size();
            picBox.Image = Bmp;
            save_bmp_set(save_bmp, Bmp);
            issava.Visible = true;
        }

        public static Bitmap KiResizeImage(Bitmap bmp, Size size)
        {
            try
            {
                Bitmap newmap = new Bitmap(size.Width, size.Height);
                Graphics g = Graphics.FromImage(newmap);

                // 插值算法的质量
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(bmp, new Rectangle(0, 0, size.Width, size.Height), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();

                return newmap;
            }
            catch
            {
                return null;
            }
        }

        //设置框架尺寸
        private void set_size()
        {
            int wid = 400, heg = 280;
            if (Bmp.Width > 400) wid = Bmp.Width;
            if (Bmp.Height > 280) heg = Bmp.Height;
            picBox.Size = Bmp.Size;
            this.ClientSize = new Size(wid + 100, heg + 120);
            //picBox.Location = new Point(50, 90);
            picBox.Location = new Point((wid + 100 - Bmp.Width) / 2, (heg + 120 - Bmp.Height) / 2 + 20);
        }

        //画笔粗细
        private void dToolStripMenuItem2px_Click(object sender, EventArgs e)
        {
            Ing = 2;//画笔粗细
            toolStripButton10.Image = global::PrtScrGUI.Properties.Resources.glow020;//调整对应显示图标

            set_cursor_size(picBox, Ing);//调整橡皮擦图标大小
        }

        private void toolStripMenuItem4px_Click(object sender, EventArgs e)
        {
            Ing = 4;
            toolStripButton10.Image = global::PrtScrGUI.Properties.Resources.glow040;

            set_cursor_size(picBox, Ing);
        }

        private void toolStripMenuItem8px_Click(object sender, EventArgs e)
        {
            Ing = 8;
            toolStripButton10.Image = global::PrtScrGUI.Properties.Resources.glow060;

            set_cursor_size(picBox, Ing);
        }

        private void toolStripMenuItem16px_Click(object sender, EventArgs e)
        {
            Ing = 16;
            toolStripButton10.Image = global::PrtScrGUI.Properties.Resources.glow080;

            set_cursor_size(picBox, Ing);
        }

        private void toolStripMenuItem32px_Click(object sender, EventArgs e)
        {
            Ing = 32;
            toolStripButton10.Image = global::PrtScrGUI.Properties.Resources.glow100;

            set_cursor_size(picBox,Ing);
        }


        //选择框样式
        private void 长方形选择ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shape_h = 0;
            toolStripButton11.Image = global::PrtScrGUI.Properties.Resources.select_2;
        }

        private void 圆形选择ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shape_h = 1;
            toolStripButton11.Image = global::PrtScrGUI.Properties.Resources.select_1;
        }

        private void 直线ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (cut_sta_only)
            {
                MessageBox.Show("剪切形状不包括直线，请重新选择！");
                return;
            }

            shape_h = 2;
            toolStripButton11.Image = global::PrtScrGUI.Properties.Resources.select_3;
        }


        //表示剪切开始
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (shape_h == 2 || shape_h == 3)
            {
                MessageBox.Show("剪切形状不包括直线/自由线，请重新选择！");
                return;
            }

            if (cut_sta_only && cut_reg.Width != 0 && cut_reg.Height != 0)
            {
                Bitmap bim = new Bitmap(cut_reg.Width, cut_reg.Height);
                Graphics g = Graphics.FromImage(bim);
                g.DrawImage(Bmp, new Rectangle(0, 0, cut_reg.Width, cut_reg.Height), cut_reg, GraphicsUnit.Pixel);
                Bmp = (Bitmap)bim.Clone();
                set_size();
                picBox.Image = Bmp;
                save_bmp_set(save_bmp, Bmp);
                issava.Visible = true;
                bim.Dispose();
            }
            else
            {
                set_only(1);//当鼠标的一种功能存在时（移动 画图 剪切），则关闭其他功能
                picBox.Cursor = Cursors.Default;
                text_word.Text = "";
                text_word.Visible = false;
            }
        }

        //选择画图工具以及画图开始
        private void 铅笔ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            set_only(2);//当鼠标的一种功能存在时（移动 画图 剪切 橡皮擦），则关闭其他功能
            set_cursor(global::PrtScrGUI.Properties.Resources.pencil_cur);
        }

        //鼠标移动控件
        private void toolStripSplitButton2_Click(object sender, EventArgs e)
        {
            set_only(3);//当鼠标的一种功能存在时（移动 画图 剪切 橡皮擦），则关闭其他功能
            set_cursor(global::PrtScrGUI.Properties.Resources.gesture_options);
        }

        //表示选择鼠标指针
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            set_only(0);//当鼠标的一种功能存在时（移动 画图 剪切 橡皮擦），则关闭其他功能
            this.toolStrip1.Focus();
            picBox.Cursor = Cursors.Default;
            text_word.Text = "";
            text_word.Visible = false;
        }


        private void 全色橡皮擦ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            set_only(4);//当鼠标的一种功能存在时（移动 画图 剪切 橡皮擦），则关闭其他功能
            set_cursor(global::PrtScrGUI.Properties.Resources.eraser_16_16px);
            set_cursor_size(picBox,Ing);
        }

        private void 自由线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cut_sta_only)
            {
                MessageBox.Show("剪切形状不包括自由线线，请重新选择！");
                return;
            }

            shape_h = 3;
            toolStripButton11.Image = global::PrtScrGUI.Properties.Resources.select_4;
        }

        private void set_only(int _only)
        {
            CatchStart_only = false;//当鼠标的一种功能存在时（移动 画图 剪切），则关闭其他功能
            eraser_sta_only = false;
            cut_sta_only = false;
            move_sta_only = false;
            word_sta_only = false;
            Reduce_sta_only = false;

            switch (_only)//只打开一种功能
            {
                case 0: break;
                case 1: cut_sta_only = true;
                    break;
                case 2: CatchStart_only = true;
                    break;
                case 3: move_sta_only = true;
                    break;
                case 4: eraser_sta_only = true;
                    break;
                case 5: word_sta_only = true;
                    break;
                case 6: Reduce_sta_only = true;
                    break;
                default:break;
            }

            if (_only != 1) picBox.Image = Bmp;//如果没进行剪切，则取消剪切图案
        }

        private void set_cursor(Bitmap _cursor)//设置光标图案
        {
            //if(1==0) return;//是否启用光标
            Cursor cursor = new Cursor(_cursor.GetHicon());
            picBox.Cursor = cursor;

            _cursor.Dispose();
            text_word.Text = "";
            text_word.Visible=false;
        }

        private void set_cursor_size(PictureBox _cursor, int size)//设置光标图案
        {
            if (!eraser_sta_only) return;
            Bitmap newbit = new Bitmap(global::PrtScrGUI.Properties.Resources.eraser_16_16px, Ing, Ing);
            if (2 < Ing && Ing < 32)
            {
                Graphics g = Graphics.FromImage(newbit);
                Pen p = new Pen(Color.Black, 1);
                g.DrawLine(p, 0, newbit.Height - 1, newbit.Width - 1, newbit.Height - 1);
                g.DrawLine(p, newbit.Width - 1, 0, newbit.Width - 1, newbit.Height - 1);
                g.Dispose();
            }
            set_cursor(newbit);
        }

        //左旋转图片
        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            Bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);//旋转图片
            set_size();
            picBox.Image = Bmp;
        }

        //右旋转图片
        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            Bmp.RotateFlip(RotateFlipType.Rotate90FlipXY);
            set_size();
            picBox.Image = Bmp;
        }

        private void 一键去白底ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Bmp==null) return;
            Bmp.MakeTransparent(Color.White);//去除白色颜色
            set_size();
            picBox.Image = Bmp;
            save_bmp_set(save_bmp, Bmp);
            issava.Visible = true;
        }

        private void 一键减去其他颜色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Bmp == null) return;
            Bmp.MakeTransparent(clr);
            set_size();
            picBox.Image = Bmp;
            save_bmp_set(save_bmp, Bmp);
            issava.Visible = true;
        }

        private void 画布ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Bmp == null) return;

            Newbmp newform = new Newbmp(1);
            this.Visible = false;
            newform.ShowDialog();
            par = newform.Get_par();
            newform.Dispose();
            this.Visible = true;
            if (par.IsEmpty()) return;

            Bitmap bim = new Bitmap(par.Wid, par.Heg);
            Graphics g = Graphics.FromImage(bim);
            g.Clear(Color.White);
            g.DrawImage(Bmp, new Rectangle(0, 0, par.Wid, par.Heg), new Rectangle(0, 0, par.Wid, par.Heg), GraphicsUnit.Pixel);
            Bmp = (Bitmap)bim.Clone();
            g.Dispose();
            bim.Dispose();

            set_size();
            picBox.Image = Bmp;
            save_bmp_set(save_bmp, Bmp);
            issava.Visible = true;
            
        }

        //保存修改图片
        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd_FileName == null)
            {
                save();
                return;
            }
            FileInfo file = new FileInfo(ofd_FileName);
            if (file.Exists)
            {
                file.Delete();
                Bmp.Save(ofd_FileName); 
                issava.Visible = false;
            }
            else
            {
                Bmp.Save(ofd_FileName);
                issava.Visible = false;
            }
        }

        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save();
        }

        private void save()
        {
            if (Bmp != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = @"Bitmap文件(*.bmp)|*.bmp|PNG文件(*.png)|*.png|GIF文件(*.gif)|*.gif|Jpeg文件(*.jpg)|*.jpg|所有合适文件(*.bmp,*.jpg)|*.bmp;*.jpg";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.FileName = "默认新建GDI图片";
                saveFileDialog.RestoreDirectory = true;
                if (DialogResult.OK == saveFileDialog.ShowDialog())
                {
                    ImageFormat format = ImageFormat.Png;
                    switch (Path.GetExtension(saveFileDialog.FileName).ToLower())
                    {
                        case ".jpg":
                            format = ImageFormat.Jpeg;
                            break;
                        case ".bmp":
                            format = ImageFormat.Bmp;
                            break;
                        case ".gif":
                            format = ImageFormat.Gif;
                            break;
                        case ".png":
                            format = ImageFormat.Png;
                            break;
                        default:
                            MessageBox.Show(this, "Unsupported image format was specified", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                    }
                    try
                    {
                        Bmp.Save(saveFileDialog.FileName, format);
                        issava.Visible = false;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(this, "Failed writing image file", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please start！");
            }
        }

        //选择字体
        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            fontDialog1.ShowHelp = true;
            fontDialog1.ShowDialog();
            word_font = fontDialog1.Font;
            set_only(5);
            Bitmap _cursor=global::PrtScrGUI.Properties.Resources.word_1;
            set_cursor(_cursor);
        }

        //选择颜色！！
        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowHelp = true;
            colorDialog1.ShowDialog();
            clr = colorDialog1.Color;
            toolStripButton9.BackColor = colorDialog1.Color;
        }

        //保存图片到数组，以供后退使用
        private void save_bmp_set(Bitmap[] save_bmp_, Bitmap map)
        {
            if (save_int < 19) save_int++;
            for (int m = save_int; m < 19; m++)//判定当前图片在数组中的位置，将其后面的位置置于空
            {
                save_bmp_[m] = null;
            }
            
            int i = 0;
            for (i = 0; i < 20; i++)//判定数组是否堆满
            {
                if (save_bmp_[i] == null) break;
            }
            if (i == 20)//堆满了，就加在19，其余往前移动
            {
                for (int j = 0; j < 20; j++)
                {
                    if (j == 19)
                    {
                        save_bmp_[j] = (Bitmap)map.Clone();
                        break;
                    }
                    save_bmp_[j] = (Bitmap)save_bmp_[j + 1].Clone();
                }
            }
            else
            {
                save_bmp_[i] = (Bitmap)map.Clone();
            }
        }

        //读取数组图片
        private Bitmap save_bmp_get(Bitmap[] save_bmp_,int Int_save)
        {
            Bitmap newmap = null;
            newmap = (Bitmap)save_bmp_[Int_save].Clone();
            return newmap;
        }

        //后退一步
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try//因为函数可能返回值为空，在此抛出异常
            {
                Bmp = (Bitmap)save_bmp_get(save_bmp, save_int - 1).Clone();
                set_size();
                picBox.Image = Bmp;
                save_int--;
                issava.Visible = true;
            }
            catch
            {
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try//因为函数可能返回值为空，在此抛出异常
            {
                Bmp = (Bitmap)save_bmp_get(save_bmp, save_int + 1).Clone();
                set_size();
                picBox.Image = Bmp;
                save_int++;
                issava.Visible = true;
            }
            catch
            {
            }
        }

        public void set_name_1(string str)
        {
            name_1.Visible = true;
            name_1.Text = str;
            name_1.BackColor = Color.Yellow;
            issava.Location = new Point(issava.Location.X+name_1.Size.Width-63, name_1.Location.Y - 5);
        }

        private void 加密ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileInfo file = new FileInfo(ofd_FileName);
            file.Encrypt();
        }

        private void 解密ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileInfo file = new FileInfo(ofd_FileName);
            file.Decrypt();
        }

        private string str_name(string file)
        {
            string name=file.Substring(file.LastIndexOf("\\")+1);
            return name;
        } 

        private void run_gif(string str)
        {
            this.Close();
            dynamic dyn = new dynamic(str);
            Thread newThread = new Thread(() => Application.Run(dyn));
            newThread.SetApartmentState(System.Threading.ApartmentState.STA);
            newThread.Start();
        }

        private void 灰度化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Bmp == null) return;
            Bitmap bit = (Bitmap)RGB2Gray(Bmp).Clone();//下面都以RGB2Gray为例
            Bmp = new Bitmap(bit.Width, bit.Height);
            Graphics g = Graphics.FromImage(Bmp);

            g.DrawImage(bit, 0, 0, bit.Width, bit.Height);//将图片画到画板上
            g.Dispose();//释放画板所占资源
            bit.Dispose();

            picBox.Image = Bmp;
            save_bmp_set(save_bmp, Bmp);
            issava.Visible = true;

        }

        public static Bitmap RGB2Gray(Bitmap srcBitmap)
        {
            int wide = srcBitmap.Width;
            int height = srcBitmap.Height;
            Rectangle rect = new Rectangle(0, 0, wide, height);
            // 将Bitmap锁定到系统内存中, 获得BitmapData
            BitmapData srcBmData = srcBitmap.LockBits(rect,
                      ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //创建Bitmap
            Bitmap dstBitmap = CreateGrayscaleImage(wide, height);//这个函数在后面有定义
            BitmapData dstBmData = dstBitmap.LockBits(rect,
                      ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            // 位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行
            System.IntPtr srcPtr = srcBmData.Scan0;
            System.IntPtr dstPtr = dstBmData.Scan0;
            // 将Bitmap对象的信息存放到byte数组中
            int src_bytes = srcBmData.Stride * height;
            byte[] srcValues = new byte[src_bytes];
            int dst_bytes = dstBmData.Stride * height;
            byte[] dstValues = new byte[dst_bytes];
            //复制GRB信息到byte数组
            System.Runtime.InteropServices.Marshal.Copy(srcPtr, srcValues, 0, src_bytes);
            System.Runtime.InteropServices.Marshal.Copy(dstPtr, dstValues, 0, dst_bytes);
            // 根据Y=0.299*R+0.114*G+0.587B,Y为亮度
            for (int i = 0; i < height; i++)
                for (int j = 0; j < wide; j++)
                {
                    //只处理每行中图像像素数据,舍弃未用空间
                    //注意位图结构中RGB按BGR的顺序存储
                    int k = 3 * j;
                    byte temp = (byte)(srcValues[i * srcBmData.Stride + k + 2] * .299
                         + srcValues[i * srcBmData.Stride + k + 1] * .587 + srcValues[i * srcBmData.Stride + k] * .114);
                    dstValues[i * dstBmData.Stride + j] = temp;
                }
            //将更改过的byte[]拷贝到原位图
            System.Runtime.InteropServices.Marshal.Copy(dstValues, 0, dstPtr, dst_bytes);

            // 解锁位图
            srcBitmap.UnlockBits(srcBmData);
            dstBitmap.UnlockBits(dstBmData);
            return dstBitmap;

        }//#

        /// <summary>
        /// Create and initialize grayscale image
        /// </summary>

        public static Bitmap CreateGrayscaleImage(int width, int height)
        {

            // create new image

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            // set palette to grayscale

            SetGrayscalePalette(bmp);

            // return new image

            return bmp;

        }//#

        /// <summary>

        /// Set pallete of the image to grayscale

        /// </summary>

        public static void SetGrayscalePalette(Bitmap srcImg)
        {

            // check pixel format

            if (srcImg.PixelFormat != PixelFormat.Format8bppIndexed)

                throw new ArgumentException();

            // get palette

            ColorPalette cp = srcImg.Palette;

            // init palette

            for (int i = 0; i < 256; i++)
            {

                cp.Entries[i] = Color.FromArgb(i, i, i);

            }

            // set palette back

            srcImg.Palette = cp;

        }
    }
}
