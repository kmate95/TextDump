using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.IO;

namespace TextDump
{
    public partial class Form1 : Form
    {
        private string savfile = "txtdump.sav";
        public Form1()
        {
            InitializeComponent();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TopMost = !TopMost ;
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
           
           
        }

        private void toolStripStatusLabel1_DoubleClick(object sender, EventArgs e)
        {
 TopMost = !TopMost;
            toolStripStatusLabel1.Text = TopMost ? "Always on top" : "Not always on top";
        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(savfile)){
                richTextBox1.LoadFile(savfile);
                File.Delete(savfile);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (richTextBox1.Lines.Count() != 0)
            {
                richTextBox1.SaveFile(savfile);
            }
        }
    }
}
