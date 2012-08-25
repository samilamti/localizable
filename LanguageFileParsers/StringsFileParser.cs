using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageFileParsers
{
    /// <summary>
    /// iOS .strings file parser
    /// </summary>
    public class StringsFileParser : RegexFileParser
    {
        public StringsFileParser() : base("iOS .strings file parser", @"(\/\*\s*(?<comment>.+?)\s*\*\/\s*)?\""(?<key>.+?)\""\s*\=\s*\""(?<value>.+?)\""") { }
    }
}
