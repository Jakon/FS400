using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace FS400.Utils
{
    public class ConsoleHelper
    {
        private static ConsoleHelper _instance;

        private RichTextBox _console;

        private ConsoleHelper()
        {

        }

        public static ConsoleHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ConsoleHelper();

                return _instance;
            }
        }

        public void SetConsole(RichTextBox console)
        {
            _console = console;
        }

        public void Write(string message)
        {
            Write(message, Colors.Transparent);
        }

        public void Write(string message, Color color)
        {
            if (_console != null)
            {
                _console.AppendText($"{message}{Environment.NewLine}");
            }
        }
    }

}
