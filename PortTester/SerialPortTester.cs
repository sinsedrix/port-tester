using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;

namespace PortTester
{

    public partial class SerialPortTester : UserControl
    {
        static readonly List<int> baudRates = new List<int> { 75, 110, 134, 150, 300, 600, 1200, 1800, 2400, 4800, 7200, 9600, 14400, 19200, 38400, 57600, 115200, 128000 };
        static readonly List<int> datasBits = new List<int> { 5, 6, 7, 8 };
        static readonly List<Parity> parities = Enum.GetValues(typeof(Parity)).Cast<Parity>().ToList();
        static readonly List<StopBits> stopBits = new List<StopBits> { StopBits.One, StopBits.OnePointFive, StopBits.Two };
        static readonly List<Handshake> handshakes = Enum.GetValues(typeof(Handshake)).Cast<Handshake>().ToList();

        public SerialPortTester()
        {
            InitializeComponent();

            List<string> names = SerialPort.GetPortNames().ToList();
            names.Insert(0, "-- Sélectionner --");
            comboBoxPortNames.DataSource = names;
            comboBox1.DataSource = baudRates;
            comboBox2.DataSource = datasBits;
            comboBox3.DataSource = parities;
            comboBox4.DataSource = stopBits;
            comboBox5.DataSource = handshakes;

            timer1.Interval = 500;
            timer1.Start();


        }

        private void LogInfo(string msg)
        {
            OutilsControl.InvokeIfRequired(textBoxLog, c =>
            {
                TextBox tb = c as TextBox;
                tb.Text += string.Format("\r\n{0:s} - {1}", DateTime.Now, msg);
                tb.SelectionStart = tb.Text.Length;
                tb.ScrollToCaret();
            });
        }

        private void LogReceive(string msg)
        {
            OutilsControl.InvokeIfRequired(textBoxReceived, c =>
            {
                TextBox tb = c as TextBox;
                tb.Text += string.Format("\r\n{0:s} [RX] - {1}", DateTime.Now, msg);
                tb.SelectionStart = tb.Text.Length;
                tb.ScrollToCaret();
            });
        }

        public void PortOpen()
        {
            try
            {
                LogInfo("Ouverture port... ");
                serialPort1.BaudRate = (int)comboBox1.SelectedValue;
                serialPort1.DataBits = (int)comboBox2.SelectedValue;
                serialPort1.Parity = (Parity)comboBox3.SelectedValue;
                serialPort1.StopBits = (StopBits)comboBox4.SelectedValue;
                serialPort1.Handshake = (Handshake)comboBox5.SelectedValue;
                serialPort1.ParityReplace = 63; //?
                serialPort1.NewLine = "\n";
                serialPort1.RtsEnable = checkBoxRts.Checked;
                serialPort1.DtrEnable = checkBoxDtr.Checked;
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(SerialPort1_DataReceived);
                serialPort1.ErrorReceived += new SerialErrorReceivedEventHandler(SerialPort1_ErrorReceived);
                serialPort1.Open();

                serialPort1.BaseStream.ReadTimeout = 0;

                LogInfo("Ouverture port : OK");
            }
            catch (Exception ex)
            {
                LogInfo("Ouverture port : KO : " + ex.Message);
            }
        }

        public void PortClose()
        {
            try 
            {
                LogInfo("Fermeture port... ");
                //th.Abort();
                serialPort1.Close();
                LogInfo("Fermeture port OK ");
            }
            catch(Exception ex)
            {
                LogInfo("Fermeture port KO : " + ex.Message);
            }
        }

        private void ComboBoxPortNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1 = new SerialPort((string)comboBoxPortNames.SelectedValue);
            serialPort1.ReadTimeout = 0;
            
            comboBox1.SelectedItem = serialPort1.BaudRate;
            comboBox2.SelectedItem = serialPort1.DataBits;
            comboBox3.SelectedItem = serialPort1.Parity;
            comboBox4.SelectedItem = serialPort1.StopBits;
            comboBox5.SelectedItem = serialPort1.Handshake;
            checkBoxRts.Checked = serialPort1.RtsEnable;
            checkBoxDtr.Checked = serialPort1.DtrEnable;
        }

        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            LogInfo("Données reçues...");
            LogReceive(serialPort1.ReadExisting());
        }

        private void SerialPort1_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            LogInfo("Erreur reçue...");
        }


        private void Timer1_Tick(object sender, EventArgs e)
        {
            textBoxStatut.BackColor = textBoxStatut.BackColor;
            textBoxStatut.ForeColor = serialPort1.IsOpen ? Color.Green : Color.Red;
            textBoxStatut.Text = serialPort1.IsOpen ? "Ouvert" : "Fermé";
            if (serialPort1.IsOpen)
            {
                checkBoxCts.Checked = serialPort1.CtsHolding;
                checkBoxDsr.Checked = serialPort1.DsrHolding;
            }
        }

        private void ButtonClear1_Click(object sender, EventArgs e)
        {
            textBoxLog.Text = "";
        }

        private void ButtonClear2_Click(object sender, EventArgs e)
        {
            textBoxReceived.Text = "";
        }

        private void ButtonOpen_Click(object sender, EventArgs e)
        {
            PortOpen();
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            PortClose();
        }

        private void ButtonSend_Click(object sender, EventArgs e)
        {
            serialPort1.Write(textBoxSend.Text);
        }
    }

    public static class OutilsControl
    {
        internal static void InvokeIfRequired(Control c, Action<Control> action)
        {
            if (c.InvokeRequired)
            {
                c.Invoke(new Action(() => action(c)));
            }
            else
            {
                action(c);
            }
        }
    }
}
