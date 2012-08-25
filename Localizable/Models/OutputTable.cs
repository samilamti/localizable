using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ngenstrings;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Localizable.Models
{
    public class OutputTable : Dictionary<string, OutputTranslation>
    {
		public string ToString(OutputFormat format, string attributions = null)
		{
			var builder = new StringBuilder();
			switch (format)
			{
				case OutputFormat.Strings:
					builder.Append(LocalizedString.FileHeaderCStyleString(attributions));
                    foreach (var value in Values) builder.Append(LocalizedString.ValueCStyleString(value.Key, value.Value, value.Comment));
					break;
                //case OutputFormat.Xml:
                //    var list = this.Values.ToList();
                //    var serializer = new XmlSerializer(typeof(List<LocalizedString>));
                //    using (var writer = new StringWriter(builder))
                //    {
                //        serializer.Serialize(writer, list);
                //    }
                //    break;
				case OutputFormat.PList:
					builder.Append(LocalizedString.FileHeaderXmlString(attributions));
					foreach (var value in Values) builder.Append(LocalizedString.ValuePListString(value.Key, value.Value, value.Comment));
					builder.Append(LocalizedString.FileFooterXmlString());
					break;
				case OutputFormat.ResX:
					builder.Append(LocalizedString.FileHeaderResxString(attributions));
					foreach (var value in Values) builder.Append(LocalizedString.ValueResxString(value.Key, value.Value, value.Comment));
					builder.Append(LocalizedString.FileFooterResxString());
					break;
				case OutputFormat.AndroidXml:
					builder.Append(LocalizedString.FileHeaderAndroidXmlString(attributions));
					foreach (var value in Values) builder.Append(LocalizedString.ValueAndroidXmlString(value.Key, value.Value, value.Comment));
					builder.Append(LocalizedString.FileFooterAndroidXmlString());
					break;
			}
			return builder.ToString();
		}            
    }
}