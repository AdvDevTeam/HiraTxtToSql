using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvDevTeam.HiraTxtToSql.Core.Models
{
    /// <summary>
    /// Represents an hymn
    /// </summary>
    public class Hymn
    {
        /// <summary>
        /// Hymn number
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Hymn title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Hymn stanzas and chorus
        /// </summary>
        private List<Stanza> _stanzas = null;
        public List<Stanza> Stanzas
        {
            get
            {
                if (_stanzas == null)
                    _stanzas = new List<Stanza>();
                return _stanzas;
            }
            set { _stanzas = value; }
        }
    }
}
