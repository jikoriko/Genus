using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    [Serializable]
    public class MessageOption
    {
        public string Option;
        public int OptionEventID;

        public MessageOption()
        {
            Option = "";
            OptionEventID = -1;
        }
    }
}
