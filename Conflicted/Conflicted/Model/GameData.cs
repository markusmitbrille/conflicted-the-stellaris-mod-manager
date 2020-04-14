using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Conflicted.Model
{
    [DataContract]
    internal class GameData : Comparer<Mod>
    {
        [DataMember(Name = "modsOrder")]
        public List<string> ModsOrder { get; set; }

        [DataMember(Name = "isEulaAccepted")]
        public bool IsEulaAccepted { get; set; }

        public override int Compare(Mod x, Mod y)
        {
            int xIndex = ModsOrder.IndexOf(x.ID);
            int yIndex = ModsOrder.IndexOf(y.ID);

            return xIndex == yIndex ? 0 : xIndex == -1 ? -1 : yIndex == -1 ? 1 : xIndex - yIndex;
        }
    }
}