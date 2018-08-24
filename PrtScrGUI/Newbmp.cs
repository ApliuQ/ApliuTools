using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PrtScrGUI
{
    public partial class Newbmp : Form
    {
        Parameters par = new Parameters();

        public Newbmp(int style)
        {
            InitializeComponent();
            if (style == 0) label1.Text = "新图像";
            else
            {
                label1.Text = "画布大小";
                label2.Visible = false;
                checkBox1.Visible = false;
            }
        }

        //取消
        private void exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //确定
        private void retu_Click(object sender, EventArgs e)
        {
            par.Wid = int.TryParse(widtext.Text, out int a) ? a : 0;
            par.Heg = int.TryParse(hegtext.Text, out int b) ? b : 0;
            par.Trfs = checkBox1.Checked;
            this.Close();
        }

        public Parameters Get_par()
        {
            return par;
        }

        private void Newbmp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
