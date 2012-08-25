using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LanguageFileParsers;

namespace Localizable.Tests.Parsers
{
    [TestClass]
    public class StringsFileParserTests
    {
        [TestMethod]
        public void CanParseStringsFile()
        {
            const string fileContents = @"
/* Arbitrary file-wide comment ... */
/* Insert Element menu item */
""Insert Element"" = ""Insert Element"";
/* Error string used for unknown error types. */
""ErrorString_1"" = ""An unknown error occurred."";
/* Credits goes out to ... */";

            ILanguageFileParser parser = new StringsFileParser();

            var table = parser.Parse(fileContents);
            Assert.AreEqual(2, table.Keys.Count);

            var key = table.Keys.First();
            Assert.AreEqual("ErrorString_1", key);

            var value = table[key];
            Assert.AreEqual("Error string used for unknown error types.", value.Comment);
            Assert.AreEqual("An unknown error occurred.", value.Value);

            key = table.Keys.Last();
            Assert.AreEqual("Insert Element", key);

            value = table[key];
            Assert.AreEqual("Insert Element menu item", value.Comment);
            Assert.AreEqual("Insert Element", value.Value);
        }
    }
}
