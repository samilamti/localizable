using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Models
{
    [DataContract]
    public class TranslationKey
    {
        /// <summary>
        /// Serialization constructor
        /// </summary>
        public TranslationKey() {}

        public TranslationKey(string key, string comment)
        {
            Key = key;
            Comment = comment;
        }

        [Key]
        public int Id { get; set; }

        [DataMember, StringLength(255)]
        public string Key { get; set; }

        [DataMember, StringLength(1024)]
        public string Comment { get; set; }

        [DataMember]
        public virtual IList<Translation> Translations { get; set; }

        [DataMember]
        public int DownVotes { get; set; }
    }
}