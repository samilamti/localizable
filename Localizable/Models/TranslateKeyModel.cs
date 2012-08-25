using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Models;
using System.ComponentModel.DataAnnotations;

namespace Localizable.Models
{
    public class TranslateKeyModel
    {
        [DisplayName("Language")]
        public string Language { get; set; }

        public int KeyId { get; set; }

        public IEnumerable<string> Translations { get; set; }

        [Required]
        public string NewTranslation { get; set; }

        [DisplayName("Key")]
        public string Text { get; set; }

        [DisplayName("Comment")]
        public string Comment { get; set; }
    }
}