using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MarkdownDeep;
using RazorEngine;

namespace RDumont.Frankie.Core
{
    public class Post
    {
        public const string POSTS_FOLDER = "_posts";
        private static readonly Regex PostFileRegex = new Regex(@"^.+(\\|/)" + POSTS_FOLDER
            + @"(\\|/)((?<category>.+?)(\\|/))*(?<year>\d{4})-(?<month>\d{1,2})-(?<day>\d{1,2})-(?<title>.+)\.(?<ext>.+)$",
            RegexOptions.Compiled);

        private readonly string absoluteFilePath;
        private static Markdown markdownEngine;

        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Extension { get; set; }
        public string[] Category { get; set; }
        public string Body { get; set; }
        public NameValueCollection Metadata { get; private set; }

        protected Post()
        {
        }

        public Post(string absoluteFilePath)
        {
            this.absoluteFilePath = absoluteFilePath;
            var match = PostFileRegex.Match(absoluteFilePath);
            if (!match.Success)
                throw new ArgumentException("Invalid post file path");

            var year = int.Parse(match.Groups["year"].Value);
            var month = int.Parse(match.Groups["month"].Value);
            var day = int.Parse(match.Groups["day"].Value);

            this.Date = new DateTime(year, month, day);
            this.Title = match.Groups["title"].Value;
            this.Extension = match.Groups["ext"].Value;
            this.Category = match.Groups["category"].Captures
                .Cast<Capture>().Select(c => c.Value).ToArray();
        }

        public string ResolvePermalink(string template)
        {
            return template
                .Replace(":year", this.Date.Year.ToString("0000"))
                .Replace(":month", this.Date.Month.ToString("00"))
                .Replace(":day", this.Date.Day.ToString("00"))
                .Replace(":title", this.Title);
        }

        public static Post FromFile(string path)
        {
            try
            {
                return new Post(path);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public void LoadFile()
        {
            var contents = File.ReadAllText(this.absoluteFilePath);
            this.Body = contents;

            ReadMetadata();
        }

        protected virtual void ReadMetadata()
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

        public void ExecuteTransformationPipeline()
        {
            if (this.Extension == "md" || this.Extension == "markdown")
                this.TransformMarkdown();

            this.ParseTemplate();
        }

        private void TransformMarkdown()
        {
            markdownEngine = markdownEngine ?? new Markdown
                {
                    ExtraMode = true,
                    AutoHeadingIDs = true
                };

            this.Body = markdownEngine.Transform(this.Body);
        }

        private void ParseTemplate()
        {
            var templateName = this.Metadata["template"] ?? "_post";

            try
            {
                this.Body = Razor.Run(templateName, this);
            }
            catch (InvalidOperationException exception)
            {
                if (!exception.Message.StartsWith("No template exists")) throw;

                Logger.Current.LogError("{0}\n  No template exists with name '{1}'",
                    this.absoluteFilePath, templateName);
            }
        }
    }
}
