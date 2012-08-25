using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ngenstrings;
using System.IO;

namespace LanguageFileParsers
{
    public class FileParserFacade
    {
        private List<ILanguageFileParser> parsers;

        public FileParserFacade(IEnumerable<ILanguageFileParser> parsers)
        {
            this.parsers = new List<ILanguageFileParser>(parsers);
        }

        public List<LocalizedStringTable> ExtractStringTables(Stream inputStream, string sourceFileName = null)
        {
            if (sourceFileName != null && Path.GetExtension(sourceFileName) == ".dll")
                return new List<LocalizedStringTable>(ngenstrings.MainClass.ParseDll(inputStream).Values);

            using(var reader = new StreamReader(inputStream, Encoding.UTF8))
            {
                var fileContents = reader.ReadToEnd();
                return new List<LocalizedStringTable> {
                    parsers
                        .First(p => p.CanParse(fileContents))
                        .Parse(fileContents)
                };
            }
        }
    }
}
