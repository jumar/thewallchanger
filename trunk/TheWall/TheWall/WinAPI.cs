using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TheWall
{
    public class WinAPI
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        public const int SPI_SETDESKWALLPAPER = 20;
        public const int SPIF_SENDCHANGE = 0x2;

    }  


}
