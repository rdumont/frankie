using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;

namespace RDumont.Frankie.Core
{
    public class ContentFile
    {
        public string Body { get; set; }
        public NameValueCollection Metadata { get; protected set; }

        protected void ExtractMetadata()
        {
            this.Metadata = new NameValueCollection();
            var reader = new StringReader(this.Body);
            var line = reader.ReadLine();
            while (line != null && line.StartsWith("@"))
            {
                var match = Regex.Match(line, @"@(?<key>\w+)\s+(?<value>.+)$");
                if (match.Success)
                    this.Metadata.Add(match.Groups["key"].Value, match.Groups["value"].Value);

                line = reader.ReadLine();
            }

            this.Body = line + reader.ReadToEnd();
        }
    }
}