using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Models
{
    [DataContract]
    public class Translator
    {
        [DataMember, Key]
        public int Id { get; set; }
    }
}