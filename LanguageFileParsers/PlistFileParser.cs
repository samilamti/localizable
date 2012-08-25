using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageFileParsers
{
    /// <summary>
    /// iOS .plist file parser
    /// </summary>
    public class PlistFileParser : RegexFileParser
    {
        public PlistFileParser() : base("iOS .plist file parser", @"(<!--\s*(?<comment>.+?)\s*-->)?\s*<key>(?<key>.+?)</key>\s*<string>(?<value>.+?)</string>") { }
    }
}
