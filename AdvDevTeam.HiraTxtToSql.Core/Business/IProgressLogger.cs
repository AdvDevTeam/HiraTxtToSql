using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvDevTeam.HiraTxtToSql.Core.Business
{
    /// <summary>
    /// Provides methods to log progression of importing
    /// </summary>
    public interface IProgressLogger
    {
        void WriteLine(string text);
        void WriteLine(string format, params object[] args);
        void Write(string text);
        void Write(string format, params object[] args);
    }
}
