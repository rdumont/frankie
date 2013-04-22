using System.IO;
using DotLiquid;
using DotLiquid.FileSystems;

namespace RDumont.Frankie.Core
{
    public class TemplatesFileSystem : IFileSystem
    {
        private readonly string basePath;

        protected Io Io { get; set; }

        public TemplatesFileSystem(string basePath)
        {
            this.basePath = basePath;
            this.Io = new Io();
        }


        public string ReadTemplateFile(Context context, string templateName)
        {
            var pagePath = context["path"] as string;
            var templatePath = TemplateManager.GetFullPath(templateName);
            DependencyTracker.Current.Add(pagePath, templatePath);

            var templateFullPath = Path.Combine(this.basePath, templatePath);
            var contents = Io.ReadFile(templateFullPath, 3);

            contents = TemplateManager.Current.PrepareTemplateContents(contents, context, templateName);

            return contents;
        }
    }
}