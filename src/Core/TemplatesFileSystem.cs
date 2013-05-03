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
            var templatePath = TemplateManager.GetFullPath(templateName);
            var dependantPath = context["dependantPath"] as string;
            if (dependantPath != null)
            {
                DependencyTracker.Current.Add(dependantPath, templatePath);
                context["dependantPath"] = null;
            }

            var templateFullPath = Path.Combine(this.basePath, templatePath);
            var contents = Io.ReadFile(templateFullPath, 5);

            contents = TemplateManager.Current.PrepareTemplateContents(contents, context, templateName);

            return contents;
        }
    }
}