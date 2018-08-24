using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PrtScrGUI
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]

        static void Main(string[] args)
        {
            bool is_gif = false;
            string ofd_FileName = "";
            if (args.Length != 0)
            {
                ofd_FileName = args[args.Length - 1];
                if (ofd_FileName.Substring(ofd_FileName.LastIndexOf(".") + 1).ToLower().Equals("gif")) is_gif=true;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (is_gif) {  Application.Run(new dynamic(ofd_FileName));}
            else Application.Run(new Start(args));
        }
    }
}
