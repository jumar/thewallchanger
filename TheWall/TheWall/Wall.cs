using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace TheWall
{
    public class Wall
    {
        private static System.Windows.Forms.NotifyIcon aTrayIcon;
        private static List<String> aFileList;
        private static IEnumerator aFileIterator;
        private System.Windows.Forms.ContextMenuStrip aTrayIconMenu;

        private System.Windows.Forms.ToolStripItem aMenuItemBrowse;
        private System.Windows.Forms.ToolStripItem aMenuItemNext;
        private System.Windows.Forms.ToolStripItem aMenuItemQuit;
        private System.Windows.Forms.FolderBrowserDialog aFolderBrowserDialog;
        public static readonly Random random = new Random();
        private static Timer aTimer = new Timer();
        private FileSystemWatcher aWatcher;
        private String aWallDir;
        public Wall()
        {

            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Wall));

            aTrayIcon = new System.Windows.Forms.NotifyIcon();
            this.aTrayIconMenu = new System.Windows.Forms.ContextMenuStrip();
            this.aMenuItemBrowse = new System.Windows.Forms.ToolStripMenuItem();
            this.aMenuItemNext = new System.Windows.Forms.ToolStripMenuItem();
            this.aMenuItemQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.aFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.aTrayIconMenu.SuspendLayout();
            // 
            // aTrayIcon
            // 
            aTrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("aTrayIcon.Icon")));
            aTrayIcon.Text = "The Wall";
            aTrayIcon.Visible = true;
            aTrayIcon.MouseClick += new MouseEventHandler(this.aTrayIcon_Click);

            // 
            // aTrayIconMenu
            // 
            this.aTrayIconMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aMenuItemBrowse, this.aMenuItemNext, new ToolStripSeparator(), this.aMenuItemQuit});
            this.aTrayIconMenu.Name = "aTrayIconMenu";
            this.aTrayIconMenu.Size = new System.Drawing.Size(174, 26);
            this.aTrayIconMenu.DoubleClick += new System.EventHandler(this.aMenuItemBrowse_Click);
            // 
            // aMenuItemBrowse
            // 
            this.aMenuItemBrowse.Name = "aMenuItemBrowse";
            this.aMenuItemBrowse.Size = new System.Drawing.Size(173, 22);
            this.aMenuItemBrowse.Text = "Set The Wall folder";
            this.aMenuItemBrowse.Click += new System.EventHandler(this.aMenuItemBrowse_Click);
            this.aMenuItemBrowse.Image = ((System.Drawing.Image)(resources.GetObject("fileopen")));
            this.aMenuItemBrowse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.aMenuItemBrowse.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // aMenuItemNext
            // 
            this.aMenuItemNext.Name = "aMenuItemNext";
            this.aMenuItemNext.Size = new System.Drawing.Size(173, 22);
            this.aMenuItemNext.Text = "Next";
            this.aMenuItemNext.Click += new System.EventHandler(this.aMenuItemNext_Click);
            this.aMenuItemNext.Image = ((System.Drawing.Image)(resources.GetObject("1rightarrow")));
            this.aMenuItemNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.aMenuItemNext.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // aMenuItemQuit
            // 
            this.aMenuItemQuit.Name = "aMenuItemQuit";
            this.aMenuItemQuit.Size = new System.Drawing.Size(173, 22);
            this.aMenuItemQuit.Text = "Quit";
            this.aMenuItemQuit.Click += new System.EventHandler(this.aMenuQuit_Click);
            this.aMenuItemQuit.Image = ((System.Drawing.Image)(resources.GetObject("exit")));
            this.aMenuItemQuit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.aMenuItemQuit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;

            aTrayIcon.ContextMenuStrip = aTrayIconMenu;
            this.aTrayIconMenu.ResumeLayout();

            aTimer.Tick += new EventHandler(this.mTick);
            aTimer.Interval = 10;
        }

        public void mStartTimer(int pIntervalMin)
        {
            aTimer.Interval = pIntervalMin * 1000 * 60;
            aTimer.Start();
        }

        public void mTick(object sender, EventArgs eArgs)
        {
            aTimer.Stop();
            mSetNextWallpaper();
            aTimer.Start();
        }

        private void aMenuItemBrowse_Click(object sender, EventArgs e)
        {
            aFolderBrowserDialog.ShowDialog();
            mSetWallDir(aFolderBrowserDialog.SelectedPath);
        }

        public void mSetWallDir(String pWallDir)
        {
            aWallDir = pWallDir;
            if (aWallDir != "")
            {
                mPopulateFiles();
                mSetNextWallpaper();
                mStartDirWatcher();
            }
            
        }

        private void mPopulateFiles()
        {
            aFileList = new List<string>();
            foreach (string lFileName in Directory.GetFiles(aWallDir))
            {
                if ((lFileName.EndsWith(".jpg") || lFileName.EndsWith(".png")))
                {
                    aFileList.Add(lFileName);
                    Console.Out.WriteLine("Adding " + lFileName);
                }
            }
            aFileList = Shuffle(aFileList);
            aFileIterator = aFileList.GetEnumerator();
            aFileIterator.Reset();
        }

        private void mStartDirWatcher()
        {
            if (aWallDir == null || aWallDir.Length == 0)
            {
                Console.WriteLine("Wall dir not set");
                return;
            }

            if (aWatcher != null)
            {
                aWatcher.EnableRaisingEvents = false;
                aWatcher = null;
            }
            Console.WriteLine("Starting watcher on " + aWallDir);
            aWatcher = new System.IO.FileSystemWatcher();
            aWatcher.Path = aWallDir;
            aWatcher.Filter = "*.*";
            aWatcher.NotifyFilter = NotifyFilters.LastAccess |
                         NotifyFilters.LastWrite |
                         NotifyFilters.FileName |
                         NotifyFilters.DirectoryName;

            aWatcher.Changed += new FileSystemEventHandler(mOnChanged);
            aWatcher.Created += new FileSystemEventHandler(mOnChanged);
            aWatcher.Deleted += new FileSystemEventHandler(mOnChanged);
            aWatcher.Renamed += new RenamedEventHandler(mOnRenamed);
            aWatcher.EnableRaisingEvents = true;
        }

        void mOnChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine(e.ChangeType);
            mPopulateFiles();
            mSetNextWallpaper();
            mStartDirWatcher();
        }
        void mOnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine(e.ChangeType);
            mPopulateFiles();
            mSetNextWallpaper();
            mStartDirWatcher();
        }

        private void aMenuItemNext_Click(object sender, EventArgs e)
        {
            aTimer.Stop();
            mSetNextWallpaper();
            aTimer.Start();
        }

        private void mSetNextWallpaper()
        {

            aFileIterator.MoveNext();
            String lNext = (String)aFileIterator.Current;
            if (File.Exists(lNext))
            {
                Console.WriteLine("Setting new wallpaper: " + lNext);
                int nResult = WinAPI.SystemParametersInfo(WinAPI.SPI_SETDESKWALLPAPER, 1, mCreateBMPFromImage(lNext), WinAPI.SPIF_SENDCHANGE);
            }
            else
            {
                Console.WriteLine("File does nort exist: " + lNext);
            }

        }

        private String mCreateBMPFromImage(String pFileName)
        {
            String lLocalpath = System.IO.Directory.GetCurrentDirectory();
            Bitmap lImage = new Bitmap(pFileName);
            lImage.Save(lLocalpath + Path.DirectorySeparatorChar + "TheWall.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            lImage = null;
            return lLocalpath + Path.DirectorySeparatorChar + "TheWall.bmp";
        }

        private void aTrayIcon_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mSetNextWallpaper();
            }
            else if (e.Button == MouseButtons.Right)
            {
                aMenuItemNext.Enabled = (aFileList != null && aFileList.Count > 0);
            }
        }

        private void aMenuQuit_Click(object sender, EventArgs e)
        {
            aTrayIcon.Visible = false;
            Application.Exit();
        }

        public static List<T> Shuffle<T>(List<T> pToShuffle)  
        {  
            List<T> lList = new List<T> ( pToShuffle );
            int N = lList.Count;  

            for (int i = 0; i < N; ++i )  
            {  
               int lRandIndex = i + (int)(random.Next(N - i));
               T lTmp = lList[lRandIndex];
               lList[lRandIndex] = lList[i];
               lList[i] = lTmp;  
            }  
            return lList;  
        }  
    }
}