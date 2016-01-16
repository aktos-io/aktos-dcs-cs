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
        // WARNING: You MUST initialize Actor object in constructor! Unlike below: 
        //ExamplePinger dcs = new ExamplePinger();  // DO NOT DO THIS
        ExamplePinger dcs;

        public Form1()
        {
            InitializeComponent();
            dcs = new ExamplePinger();  // You MUST initialize Actor objects in constructor
            dcs.event_PingMessage += handle_PingMessage;
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
        public void send_PongMessage(string text_msg)
        {
            string json_str = string.Format(@"
                {{""PongMessage"": 
                    {{""text"": ""{0}""}}
                }}
            ", text_msg);
            send(json_str);
        }
    }

}
