using AlecaPUBGClientLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlecaPUBGClientTester
{
    public partial class Form1 : Form
    {
        AlecaPUBGClient client;

        public Form1()
        {
            InitializeComponent();
        }

        private void clientStart_Click(object sender, EventArgs e)
        {
            client = new AlecaPUBGClient();
          
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text += (await client.echoRequest("yellowmellow")).ToString() + Environment.NewLine;
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text += (await client.encryptedEchoRequest("yellowmellow")).ToString() + Environment.NewLine;

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text += (await client.roasterInfoRequest("squad", "Savage_Main","[\"4buser\",\"anonoma\",\"alecamar\",\"Jitter_I3uG\"]")).ToString() + Environment.NewLine;
        }
    }
}
