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
    public partial class FormSerial : Form
    {
        public FormSerial()
        {
            InitializeComponent();

            SerialPortTester sp = new SerialPortTester();
            sp.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            Controls.Add(sp);
        }
    }
}
