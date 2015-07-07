using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvDevTeam.HiraTxtToSql.Core.Models
{
    public class CommonConstants
    {
        public class DatabaseTables
        {
            public const string TABLE_HYMN_NAME = "Hymn";
            public const string FIELD_HYMN_NUMBER_NAME = "Number";
            public const string FIELD_HYMN_TITLE_NAME = "Title";
            public const string TABLE_STANZA_NAME = "Stanza";
            public const string FIELD_STANZA_HYMN_NUMBER_NAME = "HymnNumber";
            public const string FIELD_STANZA_NUMBER_NAME = "Number";
            public const string FIELD_STANZA_CONTENT_NAME = "Content";
        }
    }
}
