using System;
using System.Windows.Forms;

namespace ImmGetCandidateListDemo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form f = new ImmForm();
            Application.Run(f);
        }
    }
}
