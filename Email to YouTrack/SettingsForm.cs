using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Email_to_YouTrack
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.baseUrl.Text = Properties.Settings.Default.baseUrl;
            this.perm.Text = Properties.Settings.Default.perm;
            this.project.Text = Properties.Settings.Default.project;
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void BaseUrl_TextChanged(object sender, EventArgs e)
        {

        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.baseUrl = this.baseUrl.Text;
            Properties.Settings.Default.perm = this.perm.Text;
            Properties.Settings.Default.project = this.project.Text;
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void Label2_Click(object sender, EventArgs e)
        {

        }

        private void TableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
