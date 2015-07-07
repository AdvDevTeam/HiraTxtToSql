using AdvDevTeam.HiraTxtToSql.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdvDevTeam.HiraTxtToSql.Core.Business
{
    /// <summary>
    /// Builds an hymn from string content
    /// </summary>
    public class HymnBuilder
    {
        private const string PATTERN_DIGIT = "^[0-9]$";
        private const string PATTERN_CHORUS = "^Ref$";

        public Hymn BuildHymn(string content, string title)
        {
            Hymn hymn = new Hymn();
            string numberPart = "";
            Regex regexNumber = new Regex(PATTERN_DIGIT);

            // the number is the first part of the title
            numberPart = title.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries).First();
            hymn.Number = int.Parse(numberPart);
            using (StringReader sr = new StringReader(content))
            {
                // first line is the title
                hymn.Title = sr.ReadLine();
                StringBuilder stanzaContent = new StringBuilder();
                int stanzaNumber = 0;

                // we assume that there is no blank line until the end of the content
                string line = sr.ReadLine();
                string[] stanzaParts = null;
                while (!string.IsNullOrWhiteSpace(line))
                {
                    // we try to figure out if the current line contains stanza number
                    stanzaParts = line.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    if (stanzaParts != null && stanzaParts.Length == 2 && regexNumber.IsMatch(stanzaParts[0]))
                    {
                        // we have new stanza,
                        // we store the current stanza first
                        if (stanzaNumber > 0)
                        {
                            Stanza stanza = new Stanza();
                            stanza.Number = stanzaNumber;
                            stanza.Content = stanzaContent.ToString();
                            hymn.Stanzas.Add(stanza);
                            stanzaContent = new StringBuilder();
                        }
                        // and then, we will consider the new one
                        stanzaNumber = int.Parse(stanzaParts[0]);
                        stanzaContent.AppendLine(stanzaParts[1]);
                    }
                    else
                        // "normal" content line
                        stanzaContent.AppendLine(line);
                    line = sr.ReadLine();
                }
                // we add the last stanza
                var r = from s in hymn.Stanzas where s.Number == stanzaNumber select s;
                if (!r.Any())
                    hymn.Stanzas.Add(new Stanza()
                    {
                        Number = stanzaNumber,
                        Content = stanzaContent.ToString()
                    });
            }
            return hymn;
        }
    }
}
