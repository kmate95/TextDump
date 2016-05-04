using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextDump
{
    public partial class frmOptions : Form
    {
        public frmOptions()
        {
            InitializeComponent();
            _syncval = Properties.Settings.Default.DoSync;
            cbDoSync.Checked  = Properties.Settings.Default.DoSync ;
        }

        private bool _syncval ;

        private void udMins_ValueChanged(object sender, EventArgs e)
        {
            var val = udMins.Value*60000 + udSec.Value*1000;
            if (val <2000)
            {
                val = 2000;
                udSec.Value = 2;
            }
            TextDump.Properties.Settings.Default.SyncInterval =(int) val;
        }

        private void frmOptions_Load(object sender, EventArgs e)
        {
            var interval = TextDump.Properties.Settings.Default.SyncInterval;
            udSec.Value= (interval%60000 )/1000;
            udMins.Value = interval/60000;
            tbPassword.Text = TextDump.Properties.Settings.Default.Password;
            panel1.Enabled = _syncval;
            cbDoSync.Checked = _syncval;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ((Form1)Owner).SetSync(_syncval);
            Properties.Settings.Default.Save();

            this.Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            TextDump.Properties.Settings.Default.Password = 
                Encryption.Encrypt(tbPassword.Text, TextDump.Properties.Settings.Default.FileName);
        }

        private void cbDoSync_CheckedChanged(object sender, EventArgs e)
        {
           panel1.Enabled= cbDoSync.Checked;
            _syncval = cbDoSync.Checked;
        }
    }
}
