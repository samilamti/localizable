using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ionic.Zip;
using ngenstrings;
using System.IO;
using System.Text;

namespace Localizable.Services
{
    public class CompressionService
    {
        internal Stream ProduceOutputStream(IDictionary<string, Models.OutputTable> outputTables, OutputFormat outputFormat)
        {
            var memoryStream = new MemoryStream();

            using (var zip = new ZipFile())
            {
                foreach (var pair in outputTables)
                {
                    var language = pair.Key;
                    var table = pair.Value;

                    var fileName = FileNameFor(language, outputFormat);
                    var contents = table.ToString(outputFormat);

                    zip.AddEntry(fileName, contents, Encoding.UTF8);
                }
                zip.Save(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
        }

        public static string FileNameFor(string language, OutputFormat outputFormat)
        {
            if (outputFormat == OutputFormat.AndroidXml)
            {
                var folderName = (language == "en" ? "values" : String.Concat("values-", language));
                return Path.Combine("Resources", folderName, "Strings.xml");
            }

            if (outputFormat == OutputFormat.PList || outputFormat == OutputFormat.Strings)
                return Path.Combine(language + ".lproj", "Localizable.strings");

            if (outputFormat == OutputFormat.ResX)
                return String.Concat("AppResources.", language, ".resx");

            return String.Concat(language, ".xml");
        }
    }
}