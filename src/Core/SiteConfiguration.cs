using System.IO;
using YamlDotNet.RepresentationModel.Serialization;

namespace RDumont.Frankie.Core
{
    public class SiteConfiguration
    {
        public string Permalink { get; set; }

        public string[] Ignore { get; set; }

        public SiteConfiguration()
        {
            Ignore = new[]
                {
                    ".bat",
                    ".gitignore",
                    ".less",
                    ".name",
                    ".xml",
                    ".iml"
                };
        }

        public static SiteConfiguration Load(string filePath)
        {
            var stream = File.OpenRead(filePath);
            var reader = new StreamReader(stream);
            var serializer = new YamlSerializer<SiteConfiguration>();

            try
            {
                return serializer.Deserialize(reader);
            }
            finally
            {
                reader.Close();
            }
        }

        public string Serialize()
        {
            var yamlSerializer = new Serializer();
            var stringWriter = new StringWriter();
            yamlSerializer.Serialize(stringWriter, this);
            return stringWriter.GetStringBuilder().ToString();
        }
    }
}