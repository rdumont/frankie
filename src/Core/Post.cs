using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotLiquid;
using MarkdownDeep;

namespace RDumont.Frankie.Core
{
    public class Post : ContentFile, ILiquidizable
    {
        public const string POSTS_FOLDER = "_posts";
        private static readonly Regex PostFileRegex = new Regex(@"^.+(\\|/)" + POSTS_FOLDER
            + @"(\\|/)((?<category>.+?)(\\|/))*(?<year>\d{4})-(?<month>\d{1,2})-(?<day>\d{1,2})-(?<title>.+)\.(?<ext>.+)$",
            RegexOptions.Compiled);

        private readonly string absoluteFilePath;

        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Slug { get; set; }
        public string Extension { get; set; }
        public string[] Category { get; set; }
        public string[] Tags { get; private set; }
        public string Permalink { get; protected set; }

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
            this.Slug = match.Groups["title"].Value;
            this.Extension = match.Groups["ext"].Value;
            this.Category = match.Groups["category"].Captures
                .Cast<Capture>().Select(c => c.Value).ToArray();
        }

        protected virtual string ResolvePermalink(string template)
        {
            return "/" + template
                .Replace(":year", this.Date.Year.ToString("0000"))
                .Replace(":month", this.Date.Month.ToString("00"))
                .Replace(":day", this.Date.Day.ToString("00"))
                .Replace(":title", this.Slug);
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

        public void LoadFile(SiteConfiguration configuration)
        {
            var contents = File.ReadAllText(this.absoluteFilePath);
            this.Body = contents;

            this.Permalink = ResolvePermalink(configuration.Permalink);

            ExtractMetadata();
            this.Tags = LoadTags();
        }

        protected string[] LoadTags()
        {
            var tagsDefinition = Metadata["tags"];
            if(tagsDefinition == null) return new string[0];

            return tagsDefinition.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(tag => tag.Trim()).ToArray();
        }

        public void ExecuteTransformationPipeline(string rootPath, SiteConfiguration configuration)
        {
            if (this.Extension == "md" || this.Extension == "markdown")
                this.TransformMarkdown();

            this.RetrieveTitle();

            var postPath = absoluteFilePath.Remove(0, rootPath.Length + 1);
            this.ParseTemplate(postPath);
        }

        protected void RetrieveTitle()
        {
            var titleRegex = new Regex(@"<h1(?:.+)?>(.+)</h1>", RegexOptions.Compiled);
            Body = titleRegex.Replace(Body, match =>
                {
                    Title = match.Groups[1].Value;
                    return "";
                }, 1);
        }

        private void ParseTemplate(string postPath)
        {
            var templateName = this.Metadata["template"] ?? "_post";

            try
            {
                this.Body = TemplateManager.Current.RenderPost(postPath, templateName, this);
            }
            catch (InvalidOperationException exception)
            {
                if (!exception.Message.StartsWith("No template exists")) throw;

                Logger.Current.LogError("{0}\n  No template exists with name '{1}'",
                    this.absoluteFilePath, templateName);
            }
        }

        public string GetDestinationFilePath(SiteConfiguration configuration)
        {
            var permalink = this.ResolvePermalink(configuration.Permalink).TrimStart('/');
            if (!(permalink.EndsWith(".html") || permalink.EndsWith(".htm")))
            {
                permalink = Path.Combine(permalink, "index.html");
            }
            return permalink;
        }

        public object ToLiquid()
        {
            return new
                {
                    date = Date,
                    slug = Slug,
                    category = Category,
                    permalink = Permalink,
                    metadata = Metadata,
                    body = Body
                };
        }
    }
}
