using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Conflicted.Model
{
    [DataContract]
    internal class GameData
    {
        [DataMember(Name = "modsOrder")]
        public List<string> ModsOrder { get; set; } = new List<string>();

        [DataMember(Name = "isEulaAccepted")]
        public bool IsEulaAccepted { get; set; } = true;
    }
}