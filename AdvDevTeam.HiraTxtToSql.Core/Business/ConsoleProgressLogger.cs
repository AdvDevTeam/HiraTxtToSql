using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvDevTeam.HiraTxtToSql.Core.Business
{
    /// <summary>
    /// Provides methods to print progress logs to current console window
    /// </summary>
    public class ConsoleProgressLogger : IProgressLogger
    {
        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }
        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }
        public void Write(string text)
        {
            Console.Write(text);
        }
        public void Write(string format, params object[] args)
        {
            Console.Write(format, args);
        }
    }
}
