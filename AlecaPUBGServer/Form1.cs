using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlecaPUBGServer
{
    public partial class Form1 : Form
    {
        AlecaPUBGServer apiServer;

        public Form1()
        {
            InitializeComponent();
        }

        private void startAPI_Click(object sender, EventArgs e)
        {
            apiServer = new AlecaPUBGServer();
            apiServer.ApplyEndpoints();
            apiServer.Start(6968);
            startAPI.Text = apiServer.test();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StaticServerData.debugTextBox = textBox1;
            startAPI.PerformClick();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 1;
            if (apiServer != null) label1.Text = apiServer.getNumberOfClients() + "";
        }
    }
}
