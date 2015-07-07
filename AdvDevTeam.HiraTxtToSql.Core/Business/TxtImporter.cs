using AdvDevTeam.HiraTxtToSql.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvDevTeam.HiraTxtToSql.Core.Business
{
    public class TxtImporter
    {
        private readonly IDataRepository _dataRepository;
        private readonly IProgressLogger _progressLogger;
        public string InputArhiveFilename { get; set; }

        public TxtImporter(IDataRepository dataRepository, IProgressLogger progressLogger)
        {
            this._dataRepository = dataRepository;
            this._progressLogger = progressLogger;
        }

        public void Import()
        {
            List<Hymn> hymns = new List<Hymn>();
            this._progressLogger.WriteLine("Loading zip file...");
            using (ZipArchive zipArchive = ZipFile.Open(this.InputArhiveFilename, ZipArchiveMode.Read, Encoding.GetEncoding(1252)))
            {
                foreach (var entry in zipArchive.Entries)
                {
                    string filename = entry.Name;
                    using (StreamReader sr = new StreamReader(entry.Open(), Encoding.GetEncoding(1252)))
                    {
                        
                        string content = sr.ReadToEnd();
                        HymnBuilder builder = new HymnBuilder();
                        Hymn hymn = builder.BuildHymn(content, filename);
                        hymns.Add(hymn);
                    }
                }
            }
            this._progressLogger.WriteLine("Zip file successfully loaded.");
            this._dataRepository.InsertHymns(hymns);
        }
    }
}
