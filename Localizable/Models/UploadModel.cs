using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Localizable.Models
{
    public class UploadModel
    {
        public HttpPostedFileBase PostedFile { get; set; }
        public string Language { get; set; }
    }
}