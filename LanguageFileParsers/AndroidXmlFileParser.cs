using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageFileParsers
{
    /// <summary>
    /// Android XML file parser
    /// </summary>
    public class AndroidXmlFileParser : RegexFileParser
    {
        public AndroidXmlFileParser() : base("Android XML file parser", @"(<!--\s*(?<comment>.+?)\s*-->\s*)*<string name=\""(?<key>.+?)\"">(?<value>.+?)</string>") { }
    }
}
