using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDumont.Frankie.Core
{
    public static class LiquidFilters
    {
        public static string DateNet(DateTime date, string format)
        {
            return date.ToString(format);
        }
    }
}
