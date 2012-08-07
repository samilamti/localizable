using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Models
{
    [DataContract]
    public class Translation
    {
        [DataMember, Key]
        public int Id { get; set; }

        [DataMember]
        public virtual TranslationKey Key { get; set; }
        
        [DataMember]
        public string Value { get; set; }
        
        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public int UpVotes { get; set; }

        [DataMember]
        public int DownVotes { get; set; }

        [DataMember]
        public int RelativeScore { get; set; }
        
        [DataMember]
        public virtual Translator Translator { get; set; }

        [DataMember]
        public string Language { get; set; }
    }
}
