using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TheWall
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
            
            Wall lWall = new Wall();
            lWall.mSetWallDir("C:\\Users\\jmarbach\\Pictures\\Wallpapers");
            lWall.mStartTimer(5);
            Application.Run();
        }
    }
}