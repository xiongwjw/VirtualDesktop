using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Diagnostics;
namespace VirtualDesktop
{
    public class ButtonPanel:Panel
    {
        public string path = string.Empty;
        private Image ImageFile = null;
        private string ShowText = string.Empty;
        private ToolPanel pControl = null;
        public ButtonPanel(int width,int height,string path,string name,Image img,ToolPanel pControl)
        {
            this.path = path;
            this.pControl = pControl;
            this.Tag = name;
            this.BorderStyle = BorderStyle.None;
            this.BackColor = Color.Transparent;
            this.Width = width;
            this.Height = height;
            this.ImageFile = img;
            this.ShowText = name;
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.Cursor = Cursors.Hand;
            this.BackColor = Color.FromArgb(166, 222, 255);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.Cursor = Cursors.Default;
            this.BackColor = SystemColors.Control;
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            this.AllowDrop = true;
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if(pControl.IsAdjust)
                this.DoDragDrop(this, DragDropEffects.Move);
            else if(e.Button==MouseButtons.Right)
            {
                if (MessageBox.Show("You want to del this element?", "VirtualDestop", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DeleElement(this.ShowText);
                }
            }
        }
        public delegate void ClickDelegate();
        public event ClickDelegate OnClickEvent;
        private void ClickEvent()
        {
            if (OnClickEvent != null)
                OnClickEvent();
        }

   
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    if (File.Exists(path) || Directory.Exists(path))
                    {
                        Process.Start(path);
                        ClickEvent();
                    }
                    else
                        MessageBox.Show("can't find the path:" + path);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }
      
        public delegate void DelDelegate(string name);
        public event DelDelegate OnDelEvent;
        private void DeleElement(string name)
        {
            if (OnDelEvent != null)
                OnDelEvent(name);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (ImageFile != null && !string.IsNullOrEmpty(ShowText))
            {
                g.DrawImage(ImageFile, new Rectangle(0, 0, Width, Height - 20));
                Font textFont = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                g.DrawString(ShowText, textFont, new SolidBrush(Color.Black), new Point(0, Height - 15));
            }
            base.OnPaint(e);
        }
     
    }
}
