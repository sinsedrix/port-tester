using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.IO;

namespace PortTester
{

    public partial class SerialPortTester : UserControl
    {
        static List<int> baudRates = new List<int> { 75, 110, 134, 150, 300, 600, 1200, 1800, 2400, 4800, 7200, 9600, 14400, 19200, 38400, 57600, 115200, 128000 };
        static List<int> datasBits = new List<int> { 5, 6, 7, 8 };
        static List<Parity> parities = Enum.GetValues(typeof(Parity)).Cast<Parity>().ToList();
        static List<StopBits> stopBits = new List<StopBits> { StopBits.One, StopBits.OnePointFive, StopBits.Two };
        static List<Handshake> handshakes = Enum.GetValues(typeof(Handshake)).Cast<Handshake>().ToList();

        const int bufferSize = 1024;

        byte[] receiveBuffer = new byte[bufferSize];
        Stream stream;
        Thread th;

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
            //th = new Thread(new ThreadStart(new ThreadPort(this).Process));
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

        private void portOpen()
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
                serialPort1.RtsEnable = checkBoxRts.Checked;
                serialPort1.DtrEnable = checkBoxDtr.Checked;
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
                serialPort1.ErrorReceived += new SerialErrorReceivedEventHandler(serialPort1_ErrorReceived);
                serialPort1.Open();
                //th.Start();

                serialPort1.BaseStream.ReadTimeout = 0;
                //var t = serialPort1.BaseStream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);
                //t.ContinueWith(ReadSerialStream);

                LogInfo("Ouverture port : OK");
            }
            catch (Exception ex)
            {
                LogInfo("Ouverture port : KO : " + ex.Message);
            }
        }

        private void portClose()
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

        internal void Read()
        {
            for(bool ok = true; ok;)
            {
                try
                {
                    LogReceive(serialPort1.ReadLine());
                }
                catch (TimeoutException ex) 
                {
                    LogInfo("Echec lecture : " + ex.Message);
                    ok = false; 
                }
            }
        }

        private void comboBoxPortNames_SelectedIndexChanged(object sender, EventArgs e)
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

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            LogInfo("Données reçues...");
            LogReceive(serialPort1.ReadExisting());
        }

        private void serialPort1_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            LogInfo("Erreur reçue...");
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            textBoxStatut.BackColor = textBoxStatut.BackColor;
            textBoxStatut.ForeColor = serialPort1.IsOpen ? Color.Green : Color.Red;
            textBoxStatut.Text = serialPort1.IsOpen ? "Ouvert" : "Fermé";
            if (serialPort1.IsOpen)
            {
                checkBoxCts.Checked = serialPort1.CtsHolding;
                checkBoxDsr.Checked = serialPort1.DsrHolding;
            }
            //
        }

        private void ReadSerialStream(Task<int> t)
        {
            var bytesReceived = new byte[t.Result];
            Array.Copy(receiveBuffer, bytesReceived, t.Result);

            LogInfo("Données reçues...");
            LogReceive(Encoding.Default.GetString(bytesReceived));
        }

        private void ReadSerialBytes()
        {
            if (!serialPort1.IsOpen)
                return;

            if (serialPort1.BytesToRead > 0)
            {
                var receiveBuffer = new byte[serialPort1.ReadBufferSize];
                var numBytesRead = serialPort1.Read(receiveBuffer, 0, serialPort1.ReadBufferSize);
                var bytesReceived = new byte[numBytesRead];
                Array.Copy(receiveBuffer, bytesReceived, numBytesRead);

                LogInfo("Données reçues...");
                LogReceive(Encoding.Default.GetString(bytesReceived));
            }
        }

        private void buttonClear1_Click(object sender, EventArgs e)
        {
            textBoxLog.Text = "";
        }

        private void buttonClear2_Click(object sender, EventArgs e)
        {
            textBoxReceived.Text = "";
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            portOpen();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            portClose();
        }

    }

    class ThreadPort
    {
        SerialPortTester tester;
        internal ThreadPort(SerialPortTester pTester)
        {
            tester = pTester;
        }

        internal void Process()
        {
            tester.Read();
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
