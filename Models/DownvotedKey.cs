using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Models
{
    [DataContract]
    public class DownvotedKey
    {
        [Key]
        public int Id { get; set; }

        [DataMember]
        public virtual TranslationKey Key { get; set; }

        [DataMember]
        public virtual Translator Translator { get; set; }
    }
}
