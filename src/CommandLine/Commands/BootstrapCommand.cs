using System.IO;
using RDumont.Frankie.Core;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class BootstrapCommand : Command<BootstrapOptions>
    {
        public override string Name
        {
            get { return "bootstrap"; }
        }

        public override void ExecuteCommand(BootstrapOptions options)
        {
            var root = Directory.GetCurrentDirectory();

            CreateIndex(root);

            CreateConfig(root);

            CreatePostsFolder(root);
            CreateSiteFolder(root);
            CreateTemplates(root);
        }

        private static void CreateIndex(string root)
        {
            var baseType = typeof (Program);
            var contentStream = baseType.Assembly.GetManifestResourceStream(
                baseType.Namespace + ".Bootstrap.index.html");

            var text = new StreamReader(contentStream).ReadToEnd();
            var filePath = Path.Combine(root, "index.html");
            File.WriteAllText(filePath, text);
        }

        private static void CreatePostsFolder(string root)
        {
            var postsFolder = Path.Combine(root, "_posts");
            Directory.CreateDirectory(postsFolder);
        }

        private static void CreateSiteFolder(string root)
        {
            var siteFolder = Path.Combine(root, "_site");
            Directory.CreateDirectory(siteFolder);
            File.WriteAllText(Path.Combine(siteFolder, ".gitignore"), "*");
        }

        private static void CreateTemplates(string root)
        {
            var templatesFolder = Path.Combine(root, "_templates");
            Directory.CreateDirectory(templatesFolder);
        }

        private static void CreateConfig(string root)
        {
            var defaultConfig = new SiteConfiguration
                {
                    Permalink = ":year/:month/:day/:title"
                };
            var yaml = defaultConfig.Serialize();

            File.WriteAllText(Path.Combine(root, "config.yaml"), yaml);
        }
    }

    public class BootstrapOptions
    {
    }
}