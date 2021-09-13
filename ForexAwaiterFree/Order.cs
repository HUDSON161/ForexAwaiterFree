using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awaiter
{
    public struct Order
    {
        public double Value;
        public bool bInBruteTimeframe;
        public Order(double Value0,bool bInBruteTimeframe0)
        {
            Value = Value0;
            bInBruteTimeframe = bInBruteTimeframe0;
        }
    }
}
