﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public string Value { get; set; }
        public int ValueId { get; set; }
    }
}