using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;

namespace VirtualDesktop
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            this.Hide();
            RegistHotKey();
            LoadAppConfig();
            CreateTool();

        }
        private HotkeyHelper hotkeyHelper;
        private int favKey;

        private void RegistHotKey()
        {
            hotkeyHelper = new HotkeyHelper(this.Handle);
            favKey = hotkeyHelper.RegisterHotkey(Keys.G, HotkeyHelper.KeyFlags.MOD_ALT);//alt+s 弹出发送消息
            hotkeyHelper.OnHotkey += new HotkeyEventHandler(OnHotkey);
        }
        int horNum = 0;
        int verNum = 0;
        int sizeWidth = 0;
        int sizeHight = 0;
        private void LoadAppConfig()
        {
            string numString = System.Configuration.ConfigurationManager.AppSettings["grid"];
            string[] tmpArray = numString.Split(',');
            if (tmpArray.Length != 2) ApplicationExit();
            bool ret =  int.TryParse(tmpArray[1], out horNum);
            if (!ret) ApplicationExit();
            ret = int.TryParse(tmpArray[0], out verNum);
            if (!ret) ApplicationExit();

          
            string sizeString = System.Configuration.ConfigurationManager.AppSettings["size"];
            tmpArray = sizeString.Split(',');
            if (tmpArray.Length != 2) ApplicationExit();
            ret = int.TryParse(tmpArray[0], out sizeWidth);
            if (!ret) ApplicationExit();
            ret = int.TryParse(tmpArray[1], out sizeHight);
            if (!ret) ApplicationExit();

        }
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            public char szDisplayName;
            public char szTypeName;
        }
        SHFILEINFO shfi = new SHFILEINFO();
        [DllImport("Shell32.dll")]
        public static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                tp.IsAdjust = false;
                this.Hide();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        public const uint SHGFI_ICON = 0x000000100;     // get icon
        public const uint SHGFI_LARGEICON = 0x000000000;     // get large icon
        private void AddButton(string name, string path, string position)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppConfigSection Section = config.GetSection("applications") as AppConfigSection;
            AppKeyValueSetting keyvalue = new AppKeyValueSetting();
            keyvalue.Name = name; keyvalue.Path = path; keyvalue.Position = position;
            Section.KeyValues.Add(keyvalue);
            config.Save();
            ConfigurationManager.RefreshSection("applications"); 
        }
        private void MoveButton(string name, string path, string position)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppConfigSection Section = config.GetSection("applications") as AppConfigSection;
            Section.KeyValues.Remove(name);
            AppKeyValueSetting keyvalue = new AppKeyValueSetting();
            keyvalue.Name = name; keyvalue.Path = path; keyvalue.Position = position;
            Section.KeyValues.Add(keyvalue);
            config.Save();
            ConfigurationManager.RefreshSection("applications"); 
        }
        private void tp_OnDrag(string name, string path, string position)
        {
            AddButton(name, path, position);
        }
        private ToolPanel tp = null;
        private void CreateTool()
        {
            tp = new ToolPanel(sizeWidth, sizeHight, horNum, verNum);
            tp.Location = new Point(0, 0);
            this.Width = tp.Width; this.Height = tp.Height;
            this.Controls.Add(tp);
            AppConfigSection appSection = (AppConfigSection)ConfigurationManager.GetSection("applications");
            foreach (AppKeyValueSetting app in appSection.KeyValues)
            {
                
                string path = app.Path;
                string name = app.Name;
                string position = app.Position;
                if (!File.Exists(path)&&!Directory.Exists(path))
                    continue;
                Image img = GetFileImage(path);
                int x = 0; int y = 0;
                string numString = position;
                string[] tmpArray = numString.Split(',');
                if (tmpArray.Length != 2) continue;
                bool ret = int.TryParse(tmpArray[1], out x);
                if (!ret) continue;
                ret = int.TryParse(tmpArray[0], out y);
                if (!ret) continue;
                tp.AddButton(x, y, name, path, img);
            }
            
           
            int SH = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;
            int SW = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
            this.Location = new Point(SW, SH);
            tp.OnDropFileEvent += new ToolPanel.DropFileDelegate(tp_OnDrag);
            tp.OnDelEvent+=new ToolPanel.DelDelegate(tp_OnDelEvent);
            tp.OnClickEvent+=new ToolPanel.ClickDelegate(tp_OnClickEvent);
        }

        private void tp_OnClickEvent()
        {
            this.Hide();
        }
        private void tp_OnDelEvent(string name)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppConfigSection Section = config.GetSection("applications") as AppConfigSection;
            Section.KeyValues.Remove(name);
            config.Save();
            ConfigurationManager.RefreshSection("applications"); 
        }
        private Image GetFileImage(string path)
        {
            int iTotal = (int)SHGetFileInfo(path, 0, ref shfi, 100, SHGFI_ICON | SHGFI_LARGEICON);//SHGFI_ICON|SHGFI_SMALLICON
            Icon ic = Icon.FromHandle(shfi.hIcon);
            Image img = Image.FromHbitmap(ic.ToBitmap().GetHbitmap());
            return img;
        }
        private void ApplicationExit()
        {
            MessageBox.Show("config file wrong!exit!");
            Application.Exit();
        }
        private void OnHotkey(int hotkeyID)
        {
            if (hotkeyID == favKey)
            {
                this.TopMost = true;
                this.Show();
                this.BringToFront();
                this.Select();
            }
        }      
        private bool windowCreate = true;
        protected override void OnActivated(EventArgs e)
        {
            if (windowCreate)
            {
                base.Visible = false;
                windowCreate = false;
            }

            base.OnActivated(e);
        }
        private void FormCliboard_Deactivate(object sender, EventArgs e)
        {
            //if (!tp.IsAdjust)
            //{
            //    this.TopMost = false;
            //    this.Hide();
            //}

        }


    }
}
