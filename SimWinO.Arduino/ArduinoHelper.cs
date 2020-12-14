using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace SimWinO.Arduino
{
    public class ArduinoHelper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsConnected { get; set; }

        public SerialPort Port { get; set; }

        public int BaudRate { get; set; } = 115200;

        public string PortName { get; set; }

        public int ReadTimeout { get; set; } = 250;

        public int WriteTimeout { get; set; } = 250;

        public void Connect()
        {
            try
            {
                InitPort();
                Port.Open();
                IsConnected = Port.IsOpen;
                Port.WriteLine("S911"); // On allume la Led de l'Arduino pour lui indiquer qu'on est là ! 
                Port.WriteLine("S201"); // On active toutes les entrées dispo sur l'arduino
            }
            catch (Exception e)
            {
                IsConnected = false;
                Debug.Write(e.Message);
            }
        }

        public void Disconnect()
        {
            if (Port != null && Port.IsOpen)
            {
                try
                {
                    Port.WriteLine("S200");
                    Port.WriteLine("S910");
                    Port.Close();
                    Port.Dispose();
                    IsConnected = false;
                }
                catch (Exception e)
                {
                    Debug.Write(e.Message);
                }
            }
        }

        private void InitPort()
        {
            // TODO Check param

            Port = new SerialPort
            {
                PortName = PortName,
                BaudRate = BaudRate,
                ReadTimeout = ReadTimeout,
                WriteTimeout = WriteTimeout
            };
        }
    }
}
