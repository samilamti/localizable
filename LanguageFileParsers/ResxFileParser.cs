using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace LanguageFileParsers
{
    /// <summary>
    /// Microsoft resource file (.resx) parser
    /// </summary>
    public class ResxFileParser : RegexFileParser
    {
        public ResxFileParser() : base("Microsoft resource file (.resx) parser", @"<data name=\""(?<key>.+?)\"" xml:space=\""preserve\"">\s*<value>(?<value>.+?)</value>\s*(<comment>(?<comment>.+?)</comment>\s*)*</data>") { }
    }
}
