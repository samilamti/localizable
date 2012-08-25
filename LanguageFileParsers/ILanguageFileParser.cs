using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ngenstrings;

namespace LanguageFileParsers
{    
    public interface ILanguageFileParser
    {
        bool CanParse(string fileContents);
        LocalizedStringTable Parse(string fileContents);
        string Name { get; }
    }
}
