using System;
using System.Windows.Forms;

namespace PT_Sim
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Enable visual styles and set rendering defaults (No ApplicationConfiguration in .NET Framework 4.8)
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start the application with the main form
            Application.Run(new Form1());
        }
    }
}
