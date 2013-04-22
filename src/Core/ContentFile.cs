using System;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using MarkdownDeep;

namespace RDumont.Frankie.Core
{
    public class ContentFile
    {
        public string Body { get; set; }
        private static Markdown markdownEngine;
        public NameValueCollection Metadata { get; protected set; }

        public static NameValueCollection GetMetadata(ref string contents)
        {
            var metadata = new NameValueCollection();

            var reader = new StringReader(contents);
            var line = reader.ReadLine();
            while (line != null && line.StartsWith("@"))
            {
                var match = Regex.Match(line, @"@(?<key>\w+)\s+(?<value>.+)$");
                if (match.Success)
                    metadata.Add(match.Groups["key"].Value, match.Groups["value"].Value);

                line = reader.ReadLine();
            }

            contents = line + Environment.NewLine + reader.ReadToEnd();

            return metadata;
        }

        public void TransformMarkdown()
        {
            markdownEngine = markdownEngine ?? new Markdown
            {
                ExtraMode = true,
                AutoHeadingIDs = true
            };

            this.Body = markdownEngine.Transform(this.Body);
        }

        protected void ExtractMetadata()
        {
            var body = this.Body;
            this.Metadata = GetMetadata(ref body);
            this.Body = body;
        }
    }
}