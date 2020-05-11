using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertLib.Controls
{
    public class Field
    {
        public string Chs { get; protected set; }

        public string Eng { get; protected set; }

        public string Category { get; protected set; }

        public string Group { get; protected set; }

        public int DisplayLevel { get; protected set; }

        public int DisplayIndex { get; protected set; }

        public string Remark { get; protected set; }
    }
}
