using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LanguageFileParsers;

namespace Localizable.Tests.Parsers
{
    [TestClass]
    public class PListFileParserTests
    {
        [TestMethod]
        public void CanParsePListFile()
        {
            const string fileContents = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
    <!-- Country-key -->
	<key>appName</key>
	<string>Localizable</string>

    <key>country</key>
	<string>Nederland</string>
</dict>
</plist>";
            ILanguageFileParser parser = new PlistFileParser();

            var table = parser.Parse(fileContents);
            Assert.AreEqual(2, table.Keys.Count);

            var key = table.Keys.Last();
            Assert.AreEqual("country", key);

            var value = table[key];
            Assert.AreEqual("", value.Comment);
            Assert.AreEqual("Nederland", value.Value);

            key = table.Keys.First();
            Assert.AreEqual("appName", key);

            value = table[key];
            Assert.AreEqual("Country-key", value.Comment);
            Assert.AreEqual("Localizable", value.Value);
        }
    }
}
