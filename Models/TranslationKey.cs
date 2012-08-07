using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Models
{
    [DataContract]
    public class TranslationKey
    {
        [DataMember, Key]
        public int Id { get; set; }

        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public virtual IList<Translation> Translations { get; set; }
    }
}