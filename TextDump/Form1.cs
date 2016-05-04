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
using System.Net.Http;


namespace TextDump
{
    public partial class Form1 : Form
    {
        private bool bModified;
        private DateTime dtLastMod;
        public bool bWasError;
        public string sErrorString;
        //private string savfile = "txtdump.sav";
        public Form1()
        {
            this.bModified = false;
            bWasError = false;
            InitializeComponent();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TopMost = !TopMost;
        }

        public void SetSync(bool value)
        {
            TextDump.Properties.Settings.Default.DoSync = value;
            SyncTimer.Interval = Properties.Settings.Default.SyncInterval;
            SyncTimer.Enabled = value;
            toolStripStatusLabel5.Enabled = value;
            if (!value)
            {
                toolStripStatusLabel4.Text = "Sync status: Turned off";
                toolStripStatusLabel4.ForeColor = Color.Gray;
            }
            else
            {
                toolStripStatusLabel4.Text = "Sync status: ...";
                toolStripStatusLabel4.ForeColor = Color.Black;
            }

        }
        public bool GetSync()
        {
            return TextDump.Properties.Settings.Default.DoSync;
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var a = GetCurrentText();

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
            Properties.Settings.Default.Reload();
            if (Properties.Settings.Default.DoSync)
            {
                Sync();
            }
            else
            {
                bWasError = true;
            }
            if (bWasError)
            {
                var savfile = TextDump.Properties.Settings.Default.FileName + ".txt";
                if (File.Exists(savfile))
                {
                    richTextBox1.LoadFile(savfile);
                    File.Delete(savfile);


                }

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var savfile = TextDump.Properties.Settings.Default.FileName + ".txt";
            if (richTextBox1.Lines.Count() != 0)
            {
                richTextBox1.SaveFile(savfile);
            }

        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void toolStripStatusLabel3_Click(object sender, EventArgs e)
        {


            frmOptions frm = new frmOptions();
            SetSync(false);
            frm.Show(this);

        }

        public string GetCurrentText()
        {
            return richTextBox1.Rtf;

        }
        public void SetCurrentText(string txt)
        {
            richTextBox1.Rtf = txt;

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            bModified = true;
            dtLastMod = DateTime.Now;
        }

        public async void Sync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                        {"filename", TextDump.Properties.Settings.Default.FileName}

                    };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync(TextDump.Properties.Settings.Default.Server, content);

                    var responseString = await response.Content.ReadAsStringAsync();
                    var unix = double.Parse(responseString);
                    DateTime servermodTime = UnixTimeStampToDateTime(unix);

                    if (servermodTime > dtLastMod)
                    {
                        RefreshFromServer();
                    }
                    else
                    {
                        UploadToServer();
                    }
                    bWasError = false;
                }

            }
            catch (Exception ex)
            {
                bWasError = true;
                sErrorString = ex.Message;
            }
            finally
            {
                if (bWasError)
                {
                    toolStripStatusLabel4.Text = "Sync status: Error!";
                    toolStripStatusLabel4.ForeColor = Color.Red;
                    toolStripStatusLabel4.ToolTipText = sErrorString;
                }
                else
                {
                    toolStripStatusLabel4.Text = "Sync status: Ok!";
                    toolStripStatusLabel4.ForeColor = Color.Green;
                    toolStripStatusLabel4.ToolTipText = "";
                }
            }
        }

        private async void RefreshFromServer()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                        {"filename", Properties.Settings.Default.FileName},
                        {"computer",System.Environment.MachineName }
                    };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync(TextDump.Properties.Settings.Default.Server, content);

                    var responseString = await response.Content.ReadAsStringAsync();

                    // string[] lines = responseString.Split(Environment.NewLine.ToCharArray()).Skip(1).ToArray();
                    // string output = string.Join(Environment.NewLine, lines);
                    var data = Encryption.Decrypt(responseString, Properties.Settings.Default.Password);
                    SetCurrentText(data);
                }

            }
            catch (Exception ex)
            {
                bWasError = true;
                sErrorString = ex.Message;
                throw;
            }
        }

        private async void UploadToServer()
        {
            try
            {
                var message = Encryption.Encrypt(GetCurrentText(), Properties.Settings.Default.Password);

                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                        {"filename", TextDump.Properties.Settings.Default.FileName},
                        {"computer",System.Environment.MachineName },
                        {"data",message }
                    };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync(TextDump.Properties.Settings.Default.Server, content);

                    var responseString = await response.Content.ReadAsStringAsync();

                    bModified = false;
                }

            }
            catch (Exception ex)
            {
                bWasError = true;
                sErrorString = ex.Message;
                throw;
            }
        }

        private void SyncTimer_Tick(object sender, EventArgs e)
        {
            Sync();

        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        private void toolStripStatusLabel5_Click(object sender, EventArgs e)
        {
            Sync();
            
        }
    }
}
