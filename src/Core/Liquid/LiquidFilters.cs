using System;

namespace RDumont.Frankie.Core.Liquid
{
    public static class Filters
    {
        public static string DateNet(DateTime date, string format)
        {
            return date.ToString(format);
        }

        public static string IsoDate(DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }
    }
}
