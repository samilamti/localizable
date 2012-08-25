using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ComponentModel;
using System;

namespace Models
{
    [DataContract]
    public class Translation
    {
        private string p;

        /// <summary>
        /// Serialization constructor
        /// </summary>
        public Translation()
        {

        }

        public Translation(string language, string value)
        {
            Language = language;
            Value = value;
            Added = DateTime.UtcNow;
        }

        [DataMember, Key]
        public int Id { get; set; }

        [DataMember]
        public virtual TranslationKey Key { get; set; }
        
        [DataMember, StringLength(1024)]
        public string Value { get; set; }
        
        [DataMember]
        public int UpVotes { get; set; }

        [DataMember]
        public int DownVotes { get; set; }

        [DataMember]
        public int RelativeScore { get; set; }
        
        [DataMember]
        public virtual Translator Translator { get; set; }

        [DataMember, StringLength(2)]
        public string Language { get; set; }

        [DataMember, Required]
        public DateTime Added { get; set; }
    }
}
