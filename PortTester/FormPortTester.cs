using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PortTester
{
    public partial class FormPortTester : Form
    {
        readonly SerialPortTester sp;

        public FormPortTester()
        {
            InitializeComponent();

            sp = new SerialPortTester
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right
            };
            Controls.Add(sp);

            Closed += FormSerial_Closed;
        }

        protected void FormSerial_Closed(object sender, EventArgs e)
        {
            sp.PortClose();
        }
    }
}
