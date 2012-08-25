using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ngenstrings;
using System.Web.Mvc;

namespace Localizable.Models
{
    public class UploadModel
    {
        public HttpPostedFileBase PostedFile { get; set; }
        public string Language { get; set; }
        public IEnumerable<SelectListItem> OutputFormats { get; set; }
        public string SelectedOutputFormat { get; set; }
    }
}