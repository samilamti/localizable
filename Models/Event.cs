using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Models
{
    [DataContract]
    public class Event
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public virtual Translator Translator { get; set; }

        [DataMember]
        public int ObjectId { get; set; }

        [DataMember]
        public string EventName { get; set; }
    }
}
