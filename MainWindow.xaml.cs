using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO.Ports;
using System.Windows.Threading;

namespace SerialCommunication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort serialPort = new SerialPort();
        string recievedData;

        FlowDocument mcFlowDoc = new FlowDocument();
        Paragraph para = new Paragraph();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            string[] ports = SerialPort.GetPortNames();
            //Console.WriteLine("The following serial ports were found:");            
            foreach (string port in ports)
            {
                //Console.WriteLine(port); // Display each port name to the console.
                cBoxComPort.Items.Add(port);
            }
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                serialPort.PortName = cBoxComPort.Text;
                serialPort.BaudRate = Convert.ToInt32(cBoxBaudRate.Text);
                serialPort.DataBits = Convert.ToInt32(cBoxDataBits.Text);
                serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cBoxStopBits.Text);
                serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), cBoxParityBits.Text);
                serialPort.Open(); // Open port.
                serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(serialPort_DataRecieved);

                pBar.Value = 100;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close(); // Close port.

                pBar.Value = 0;
            }
        }

        private void BtnSendData_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write(tBoxOutData.Text);
            }
        }

        private delegate void UpdateUiTextDelegate(string text);
        private void serialPort_DataRecieved(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // Collecting the characters received to our 'buffer' (string).
            recievedData = serialPort.ReadExisting();

            // Delegate a function to display the received data.
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(DataWrited), recievedData);
        }

        private void DataWrited(string text)
        {
            tBoxInData.Text += text;
        }
    }
}
