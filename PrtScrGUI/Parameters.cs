
namespace PrtScrGUI
{
    public class Parameters
    {
        public int widtext;
        public int hegtext;
        public bool trfs;

        public Parameters() { }
        public Parameters(int a, int b, bool c)
        {
            widtext = a;
            hegtext = b;
            trfs = c;
        }

        public int Wid
        {
            get { return widtext; }
            set { widtext = value; }
        }
        public int Heg
        {
            get { return hegtext; }
            set { hegtext = value; }
        }
        public bool Trfs
        {
            get { return trfs; }
            set { trfs = value; }
        }

        public bool IsEmpty()
        {
            if (widtext == 0 || hegtext == 0) return true;
            else return false;
        }
    }
}
