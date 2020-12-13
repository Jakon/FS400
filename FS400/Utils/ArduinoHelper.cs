using System;
using System.ComponentModel;
using System.IO.Ports;

namespace FS400.Utils
{
    public class ArduinoHelper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsConnected { get; set; }

        public SerialPort Port { get; set; }

        public int BaudRate { get; set; }

        public string PortName { get; set; }

        public int ReadTimeout { get; set; }

        public int WriteTimeout { get; set; }

        public int EventsSent { get; set; }

        public void Connect()
        {
            try
            {
                InitPort();
                Port.Open();
                IsConnected = true;
                Port.WriteLine("S911"); // On allume la Led de l'Arduino pour lui indiquer qu'on est là ! 
                //Port.WriteLine("V201"); // On active toutes les entrées dispo sur l'arduino
                Port.WriteLine("S102"); // On active toutes les entrées dispo sur l'arduino
            }
            catch (Exception e)
            {
                ConsoleHelper.Instance.Write(e.Message);
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
                    ConsoleHelper.Instance.Write(e.Message);
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
