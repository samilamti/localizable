using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LanguageFileParsers;

namespace Localizable.Tests.Parsers
{
    [TestClass]
    public class AndroidXmlFileParserTests
    {
        [TestMethod]
        public void CanParseAndroidXmlResourceFile()
        {
            const string fileContents = @"<?xml version=""1.0"" encoding=""utf-8""?>
<resources>
    <string name=""hello"">Hello!</string>
    <!-- Url from which this example content was retrieved -->
    <string name=""sourceUrl"">http://developer.android.com/guide/topics/resources/string-resource.html</string>
</resources>";

            ILanguageFileParser parser = new AndroidXmlFileParser();

            var table = parser.Parse(fileContents);

            Assert.AreEqual(2, table.Keys.Count);
            Assert.AreEqual("hello", table.Keys.First());

            var secondKey = table.Keys.Skip(1).First();
            var value = table[secondKey];
            Assert.AreEqual("Url from which this example content was retrieved", value.Comment);
            Assert.AreEqual("sourceUrl", secondKey);
            Assert.AreEqual("http://developer.android.com/guide/topics/resources/string-resource.html", value.Value);
        }
    }
}
