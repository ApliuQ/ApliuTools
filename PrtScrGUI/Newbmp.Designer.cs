namespace PrtScrGUI
{
    partial class Newbmp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.exit = new System.Windows.Forms.Button();
            this.retu = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.widtext = new System.Windows.Forms.TextBox();
            this.hegtext = new System.Windows.Forms.TextBox();
            this.heg = new System.Windows.Forms.Label();
            this.wid = new System.Windows.Forms.Label();
            this.name = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // exit
            // 
            this.exit.Location = new System.Drawing.Point(69, 255);
            this.exit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(100, 29);
            this.exit.TabIndex = 19;
            this.exit.Text = "取消";
            this.exit.UseVisualStyleBackColor = true;
            this.exit.Click += new System.EventHandler(this.exit_Click);
            // 
            // retu
            // 
            this.retu.Location = new System.Drawing.Point(245, 255);
            this.retu.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.retu.Name = "retu";
            this.retu.Size = new System.Drawing.Size(100, 29);
            this.retu.TabIndex = 18;
            this.retu.Text = "确定";
            this.retu.UseVisualStyleBackColor = true;
            this.retu.Click += new System.EventHandler(this.retu_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(31, 212);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 18);
            this.label2.TabIndex = 17;
            this.label2.Text = "白底（透明）：";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(179, 214);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(18, 17);
            this.checkBox1.TabIndex = 16;
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // widtext
            // 
            this.widtext.Location = new System.Drawing.Point(176, 118);
            this.widtext.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.widtext.Name = "widtext";
            this.widtext.Size = new System.Drawing.Size(153, 25);
            this.widtext.TabIndex = 15;
            // 
            // hegtext
            // 
            this.hegtext.Location = new System.Drawing.Point(176, 160);
            this.hegtext.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.hegtext.Name = "hegtext";
            this.hegtext.Size = new System.Drawing.Size(153, 25);
            this.hegtext.TabIndex = 14;
            // 
            // heg
            // 
            this.heg.AutoSize = true;
            this.heg.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.heg.Location = new System.Drawing.Point(65, 164);
            this.heg.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.heg.Name = "heg";
            this.heg.Size = new System.Drawing.Size(44, 18);
            this.heg.TabIndex = 13;
            this.heg.Text = "高度";
            // 
            // wid
            // 
            this.wid.AutoSize = true;
            this.wid.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.wid.Location = new System.Drawing.Point(65, 118);
            this.wid.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.wid.Name = "wid";
            this.wid.Size = new System.Drawing.Size(44, 18);
            this.wid.TabIndex = 12;
            this.wid.Text = "宽度";
            // 
            // name
            // 
            this.name.AutoSize = true;
            this.name.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.name.Location = new System.Drawing.Point(65, 71);
            this.name.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(44, 18);
            this.name.TabIndex = 11;
            this.name.Text = "名称";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(136, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "新图片/画布";
            // 
            // Newbmp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 310);
            this.Controls.Add(this.exit);
            this.Controls.Add(this.retu);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.widtext);
            this.Controls.Add(this.hegtext);
            this.Controls.Add(this.heg);
            this.Controls.Add(this.wid);
            this.Controls.Add(this.name);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Newbmp";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Newbmp";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Newbmp_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button exit;
        private System.Windows.Forms.Button retu;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox widtext;
        private System.Windows.Forms.TextBox hegtext;
        private System.Windows.Forms.Label heg;
        private System.Windows.Forms.Label wid;
        private System.Windows.Forms.Label name;
        private System.Windows.Forms.Label label1;
    }
}