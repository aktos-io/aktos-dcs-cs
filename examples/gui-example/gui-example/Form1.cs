using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using aktos_dcs_cs;
using System.Threading;

namespace gui_example
{
    /**
    run aktos-dcs/examples/ponger.py in order to communicate with another actor
    **/
    public partial class Form1 : Form
    {
        ExamplePinger dcs = new ExamplePinger(); 

        public Form1()
        {
            InitializeComponent();
            dcs.event_PingMessage += handle_PingMessage;
            dcs.syncContext = SynchronizationContext.Current;
        }

        private void handle_PingMessage(Dictionary<string, object> msg)
        {
            label1.Text = (string)msg["text"];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dcs.send_PongMessage(textBox1.Text); 
        }

    }
    class ExamplePinger : Actor
    {
        public event msg_callback event_PingMessage;
        public void send_PongMessage(string msg_json)
        {
            string msg_ser = string.Format(@"
                {{""PongMessage"": 
                    {{""text"": ""{0}""}}
                }}
            ", msg_json);
            send(msg_ser);
        }
    }

}
