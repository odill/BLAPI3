using System;
using System.Windows.Forms;

namespace BLAPI3
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            textBox1.Text = Properties.Settings.Default.ConsumerKey;
            textBox2.Text = Properties.Settings.Default.ConsumerSecret;
            textBox3.Text = Properties.Settings.Default.TokenValue;
            textBox4.Text = Properties.Settings.Default.TokenSecret;
            textBox5.Text = Properties.Settings.Default.FilesPath;
            textBox6.Text = Properties.Settings.Default.LogoPath;
            checkBoxToPacked.Checked = Properties.Settings.Default.ToPacked;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ConsumerKey = textBox1.Text;
            Properties.Settings.Default.ConsumerSecret = textBox2.Text;
            Properties.Settings.Default.TokenValue = textBox3.Text;
            Properties.Settings.Default.TokenSecret = textBox4.Text;
            Properties.Settings.Default.FilesPath = textBox5.Text;
            Properties.Settings.Default.LogoPath = textBox6.Text;
            Properties.Settings.Default.ToPacked = checkBoxToPacked.Checked;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox5.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox6.Text = openFileDialog1.FileName;
            }
        }

    }
}
