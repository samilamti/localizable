using System.Collections.Generic;

namespace Localizable.Models
{
    public class TranslateModel
    {
        public string Language { get; set; }
        public IList<TranslationModel> Translations { get; set; }
    }

    public class TranslationModel
    {
        public string Key { get; set; }
        public int KeyId { get; set; }
        public string Value { get; set; }
    }
}