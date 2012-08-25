using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using ngenstrings;
using System.Security.Principal;
using Localizable.Models;

namespace Localizable.Services
{
	public class TranslationService
	{
		internal void AddKeysToDatabase(IEnumerable<LocalizedStringTable> tables)
		{
			using (var context = new Database.Context())
			{
				var foundKeys = new List<TranslationKey>();
				foreach (var source in tables)
				{
					foreach (var key in source.Keys)
					{
						if (!context.Keys.Any(tk => tk.Key == key))
							context.Keys.Add(new TranslationKey(key, source[key].Comment));
					}
				}
				context.SaveChanges();
			}
		}

		internal void AddTranslationsToDatabase(IEnumerable<LocalizedStringTable> tables, IPrincipal User, string language)
		{
			using (var context = new Database.Context())
			{
				var translator = context.GetTranslator(User);

				var foundKeys = new List<TranslationKey>();
				foreach (var source in tables)
				{
					foreach (var pair in source)
					{
						var key = context.Keys.FirstOrDefault(k => k.Key == pair.Key) ?? 
							context.Keys.Add(new TranslationKey(pair.Key, pair.Value.Comment));

						if (!context.Values.Any(v => v.Key.Key == pair.Key && v.Value == pair.Value.Value && v.Language == language))
						{
                            context.Values.Add(new Translation(language, pair.Value.Value) { Key = key, Translator = translator });
						}
					}
				}
				context.SaveChanges();
			}
		}

        internal IDictionary<string, OutputTable> ProduceOutputTables(List<LocalizedStringTable> tables)
        {
            var distinctKeys = tables.SelectMany(t => t.Keys).Distinct();
            using (var context = new Database.Context())
            {
                var foundKeys = context.Keys.Where(k => distinctKeys.Contains(k.Key));
                var languages = foundKeys
                    .SelectMany(k => k.Translations)
                    .Select(t => t.Language)
                    .Distinct();
                var translators = foundKeys
                    .SelectMany(k => k.Translations)
                    .Where(t => t.Translator != null)
                    .Select(t => t.Translator)
                    .Distinct()
                    .Select(t => t.FullName == t.EMail ? t.EMail : String.Format("{0} ({1})", t.FullName, t.EMail));
                var dict = languages.ToDictionary(language => language, language => new OutputTable());
                foreach (var translationKey in foundKeys)
                {
                    foreach (var translation in translationKey.Translations.OrderByDescending(t => t.RelativeScore))
                    {
                        var key = translation.Key.Key;
                        var table = dict[translation.Language];

                        if (table.ContainsKey(key))
                            continue;

                        var t = translation.Translator;
                        table.Add(key, new OutputTranslation
                        {
                            Comment = translation.Key.Comment,
                            Key = key,
                            Value = translation.Value,
                            Translator = t.FullName == t.EMail ? t.EMail : String.Format("{0} ({1})", t.FullName, t.EMail)
                        });
                    }
                }
                return dict;
            }
        }
    }
}