using System.IO;
using System.Reflection;
using RDumont.Frankie.CommandLine.Commands;
using YamlDotNet.RepresentationModel.Serialization;

namespace RDumont.Frankie.CommandLine
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
                baseType.Namespace + ".Bootstrap.index.cshtml");

            var text = new StreamReader(contentStream).ReadToEnd();
            var filePath = Path.Combine(root, "index.cshtml");
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
            var yamlSerializer = new Serializer();
            var stringWriter = new StringWriter();
            yamlSerializer.Serialize(stringWriter, defaultConfig);

            File.WriteAllText(Path.Combine(root, "config.yaml"), stringWriter.GetStringBuilder().ToString());
        }
    }

    public class BootstrapOptions
    {
    }
}