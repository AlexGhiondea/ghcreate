using OutputColorizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Creator.Helpers
{
    public class ColorizerFileLog : IOutputWriter, IDisposable
    {
        private string _fileName;
        StreamWriter _writer;
        public ColorizerFileLog(string fileName)
        {
            _fileName = fileName;
            _writer = new StreamWriter(fileName, true);
        }
        public ConsoleColor ForegroundColor
        {
            get
            {
                return Console.ForegroundColor;
            }

            set
            {
                Console.ForegroundColor = value;
            }
        }

        public void Write(string text)
        {
            _writer.Write(text);
            Console.Write(text);
        }

        public void WriteLine(string text)
        {
            _writer.WriteLine(text);
            Console.WriteLine(text);
        }

        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.Flush();
                _writer.Dispose();
            }
        }
    }
}
