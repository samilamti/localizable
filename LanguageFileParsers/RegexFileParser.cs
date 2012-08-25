using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ngenstrings;

namespace LanguageFileParsers
{
    public abstract class RegexFileParser : ILanguageFileParser
    {
        private string name;
        protected readonly Regex expression;
        public RegexFileParser(string name, string pattern)
        {
            this.name = name;
            expression = new Regex(pattern);
        }

        public bool CanParse(string fileContents)
        {
            return expression.IsMatch(fileContents);
        }

        public LocalizedStringTable Parse(string fileContents)
        {
            var returnValue = new LocalizedStringTable("not set");

            foreach (Match match in expression.Matches(fileContents))
            {
                var addition = new LocalizedString();
                addition.Comment = ExtractValue(match, "comment");
                addition.Key = ExtractValue(match, "key");
                addition.Value = ExtractValue(match, "value");
                returnValue.Add(addition.Key, addition);
            }

            return returnValue;
        }

        public string Name { get { return name; } }

        protected string ExtractValue(Match match, string groupName)
        {
            if (!match.Groups[groupName].Success)
                return null;
            return match.Groups[groupName].Value;
        }
    }
}
