using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvDevTeam.HiraTxtToSql.Core.Models
{
    /// <summary>
    /// Represents a stanza of an hymn
    /// </summary>
    public class Stanza
    {
        /// <summary>
        /// Stanza number
        /// </summary>
        public virtual int Number { get; set; }
        /// <summary>
        /// Stanza text
        /// </summary>
        public string Content { get; set; }
    }
}
