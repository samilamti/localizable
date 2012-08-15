using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Models
{
    [DataContract]
    public class Translator
    {
        [Key]
        public int Id { get; set; }
        
        [DataMember, Required, StringLength(256)]
        public string EMail { get; set; }

        [DataMember]
        public string FullName { get; set; }

        [DataMember]
        public string Language { get; set; }
    }
}