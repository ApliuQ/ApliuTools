using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace PrtScrGUI
{
    public partial class dynamic : Form
    {
        public Bitmap Bmp;//保存截图过来的图片

        public dynamic(string ofd_FileName)
        {
            InitializeComponent();

            if (ofd_FileName != "")
            {
                Bitmap Bmp_ofd = new Bitmap(ofd_FileName);
                Bmp = (Bitmap)Bmp_ofd.Clone();
                set_size();
                gif_open.Image = Bmp;
                set_name_1(str_name(ofd_FileName));
                Clipboard.SetImage(Bmp);//保存到剪切板中
            }
        }

        private void 返回ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Start sta = new Start();
            Thread newThread = new Thread(() => Application.Run(sta));
            newThread.SetApartmentState(System.Threading.ApartmentState.STA);
            newThread.Start();
        }

        private void set_size()
        {//400 280
            gif_open.Size = Bmp.Size;
            
            int width = gif_open.Width + 24; int height = gif_open.Height + 68;
            if (width < 400) width = 400;
            if (height < 280) height = 280;
            this.ClientSize = new Size(width, height);

            gif_open.Location = new Point((width - Bmp.Width) / 2, (height - Bmp.Height) / 2 + 20);
            Point loca = new Point((int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - this.Size.Width) / 2, (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - this.Size.Height) / 2);
            this.Location = loca;//设置窗体显示位置
        }

        private string str_name(string file)
        {
            string name = file.Substring(file.LastIndexOf("\\") + 1);
            return name;
        }

        public void set_name_1(string str)
        {
            name_1.Visible = true;
            name_1.Text = str;
            name_1.BackColor = Color.Yellow;
            issava.Location = new Point(issava.Location.X + name_1.Size.Width - 63, name_1.Location.Y - 5);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
