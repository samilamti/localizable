using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ngenstrings;

namespace LanguageFileParsers
{
    public static class StringsParser
    {
        private const string StringsPattern = @"(\/\*\s*(?<comment>.+?)\s*\*\/\s*)?\""(?<key>.+?)\""\s*\=\s*\""(?<value>.+?)\""";
        private const string PListPattern = @"(<!--\s*(?<comment>.+?)\s*-->)?\s*<key>(?<key>.+?)</key>\s*<string>(?<value>.+?)</string>";
        private const string ResxPattern = @"<data name=\""(?<key>.+?)\"" xml:space=\""preserve\"">\s*<value>(?<value>.+?)</value>\s*(<comment>(?<comment>.+?)</comment>\s*)*</data>";

        public static LocalizedStringTable ExtractTable(string fileContents)
        {
            var returnValue = new LocalizedStringTable("not set");

            var collection = Regex.Matches(fileContents, StringsPattern);
            if (collection.Count == 0)
                collection = Regex.Matches(fileContents, PListPattern);
            if (collection.Count == 0)
                collection = Regex.Matches(fileContents, ResxPattern);

            foreach(Match match in collection)
            {
                var addition = new LocalizedString();
                addition.Comment = ExtractValue(match, "comment");
                addition.Key = ExtractValue(match, "key");
                addition.Value = ExtractValue(match, "value");
                returnValue.Add(addition.Key, addition);
            }

            return returnValue;
        }

        private static string ExtractValue(Match match, string groupName)
        {
            if (!match.Groups[groupName].Success)
                return null;
            return match.Groups[groupName].Value;
        }
    }
}
