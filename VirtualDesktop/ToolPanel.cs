using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace VirtualDesktop
{
    public class ToolPanel : Panel
    {
        private int sizeWidth = 0;
        private int sizeHeight = 0;
        private int horNum = 0;
        private int verNum = 0;
        int blankSeperator = 20;
        int elementSeperator = 20;
        public bool isAdjust = false;

        public ToolPanel(int sizeWidth, int sizeHeight, int horNum, int verNum)
        {
            this.SetStyle(
              ControlStyles.AllPaintingInWmPaint |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.UserPaint |
              ControlStyles.SupportsTransparentBackColor, true);
            this.UpdateStyles();
            this.DoubleBuffered = true;
            this.BackColor = SystemColors.Control;
            this.ContextMenuStrip = contextMenu;
            InitContextMenu();
            this.AllowDrop = true;
            this.BorderStyle = BorderStyle.None;
            this.sizeWidth = sizeWidth;
            this.sizeHeight = sizeHeight;
            this.horNum = horNum; this.verNum = verNum;
            this.Width = 2 * blankSeperator + horNum * sizeWidth + elementSeperator * (horNum - 1);
            this.Height = 2 * blankSeperator + verNum * sizeHeight + elementSeperator * (verNum - 1);

        }
        private ContextMenuStrip contextMenu = new ContextMenuStrip();
        private void InitContextMenu()
        {
            this.contextMenu.Items.Clear();
            this.contextMenu.Items.Add("Close", Properties.Resources.close);
            this.contextMenu.Items.Add("Adjust", Properties.Resources.Adjust);
            this.contextMenu.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            foreach (ToolStripItem items in this.contextMenu.Items)
                items.Click += new EventHandler(items_Click);
        }
        private void items_Click(object sender, EventArgs e)
        {
            ToolStripItem ts = sender as ToolStripItem;
            if (ts != null)
            {
                if (ts.Text == "Close")
                {
                    Application.Exit();
                }
                else if (ts.Text == "Adjust")
                {
                    ts.Text = "No Adjust";
                    this.isAdjust = true;
                }
                else if (ts.Text == "No Adjust")
                {
                    ts.Text = "Adjust";
                    this.isAdjust = false;
                }
            }

        }
        public void AddButtonByLocation(int x, int y, string name, string path, Image img)
        {
            int positionX = x / (blankSeperator + sizeWidth) + 1;
            int positionY = y / (blankSeperator + sizeHeight) + 1;
            AddButton(positionX, positionY, name, path, img);
            DragFileDrop(name, path, positionY.ToString() + "," + positionX.ToString());
        }
        public void AddButton(int horPosition, int verPosition, string name, string path, Image img)
        {
            ButtonPanel bp = new ButtonPanel(sizeWidth, sizeHeight, path, name, img, this);
            bp.Tag = name;
            int x = blankSeperator + (horPosition - 1) * (bp.Width + elementSeperator);
            int y = blankSeperator + (verPosition - 1) * (bp.Height + elementSeperator);
            bp.Location = new Point(x, y);
            this.Controls.Add(bp);
            bp.OnDelEvent += new ButtonPanel.DelDelegate(bp_OnDelEvent);
            bp.OnClickEvent+=new ButtonPanel.ClickDelegate(bp_OnClickEvent);
        }
        private void bp_OnClickEvent()
        {
            ClickEvent();
        }
        private void bp_OnDelEvent(string name)
        {
            ButtonPanel con = null;
            foreach (Control control in this.Controls)
            {
                if (control is ButtonPanel && (control as ButtonPanel).Tag.ToString() == name)
                {
                    con = control as ButtonPanel;
                    break;
                }
            }
            if (con != null)
            {
                con.OnDelEvent -= new ButtonPanel.DelDelegate(bp_OnDelEvent);
                this.Controls.Remove(con);
                DeleElement(con.Tag.ToString());

            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            int pix = 3;
            Graphics g = e.Graphics;
            Brush b = new SolidBrush(Color.Blue);
            g.FillRectangle(b, 0, 0, this.Width, pix);
            g.FillRectangle(b, 0, 0, pix, this.Height);
            g.FillRectangle(b, 0, this.Height - pix, this.Width, pix);
            g.FillRectangle(b, this.Width - pix, 0, pix, this.Height);

        }
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);
            drgevent.Effect = DragDropEffects.Move;
        }
        public delegate void DelDelegate(string name);
        public event DelDelegate OnDelEvent;
        private void DeleElement(string name)
        {
            if (OnDelEvent != null)
                OnDelEvent(name);
        }

        public delegate void ClickDelegate();
        public event ClickDelegate OnClickEvent;
        private void ClickEvent()
        {
            if (OnClickEvent != null)
                OnClickEvent();
        }

        public delegate void DropFileDelegate(string name, string path, string position);
        public event DropFileDelegate OnDropFileEvent;
        private void DragFileDrop(string name, string path, string position)
        {
            if (OnDropFileEvent != null)
                OnDropFileEvent(name, path, position);
        }
        protected override void OnDragDrop(DragEventArgs drgevent)
        {

            string fileName = string.Empty;
            string name = string.Empty;
            bool isMove = false;
            if (drgevent.Data.GetFormats()[0] == "VirtualDesktop.ButtonPanel")
            {
                ButtonPanel bp = (ButtonPanel)drgevent.Data.GetData(typeof(ButtonPanel));
                fileName = bp.path;
                name = bp.Tag.ToString();
                isMove = true;
            }
            else
                fileName = ((System.Array)drgevent.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();

            if (string.IsNullOrEmpty(fileName) || (!File.Exists(fileName)&& !Directory.Exists(fileName)))
            {
                return;
            }

            if (!isMove)
            {
                name = Path.GetFileNameWithoutExtension(fileName);
                FormRename fr = new FormRename(fileName, name);
                if (DialogResult.OK == fr.ShowDialog())
                {
                    Image img = GetFileImage(fileName);
                    Point p = this.PointToClient(new Point(drgevent.X, drgevent.Y));
                    name = fr.name;
                    AddButtonByLocation(p.X, p.Y, name, fileName, img);
                }
                else
                    return;
            }
            else
            {
                Image img = GetFileImage(fileName);
                Point p = this.PointToClient(new Point(drgevent.X, drgevent.Y));
                bp_OnDelEvent(name);
                AddButtonByLocation(p.X, p.Y, name, fileName, img);
            }

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
        public const uint SHGFI_ICON = 0x000000100;     // get icon
        public const uint SHGFI_LARGEICON = 0x000000000;     // get large icon
        private Image GetFileImage(string path)
        {
            int iTotal = (int)SHGetFileInfo(path, 0, ref shfi, 100, SHGFI_ICON | SHGFI_LARGEICON);//SHGFI_ICON|SHGFI_SMALLICON
            Icon ic = Icon.FromHandle(shfi.hIcon);
            Image img = Image.FromHbitmap(ic.ToBitmap().GetHbitmap());
            return img;
        }


    }
}
