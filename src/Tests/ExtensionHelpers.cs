using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDumont.Frankie.Tests
{
    public static class HelperExtensions
    {
        public static Dictionary<string, string> AsDictionary(this NameValueCollection nameValueCollection)
        {
            return nameValueCollection.AllKeys.ToDictionary(x => x, x => nameValueCollection[x]);
        }
    }
}
