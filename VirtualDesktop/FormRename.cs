using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VirtualDesktop
{
    public partial class FormRename : Form
    {
        public string name = string.Empty;
        public FormRename(string path, string name)
        {
            InitializeComponent();
            this.name = name;
            this.txtName.Text = name;
            this.txtFilePath.Text = path;
            this.txtName.Focus();
            this.txtName.Select();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || txtName.Text.Length > 10)
            {
                MessageBox.Show("Name length >0 && <10");
                txtName.Focus();
                return;
            }
            this.name = txtName.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
