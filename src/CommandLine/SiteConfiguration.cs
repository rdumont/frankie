using System.IO;
using YamlDotNet.RepresentationModel.Serialization;

namespace RDumont.Frankie.CommandLine
{
    public class SiteConfiguration
    {
        public string Permalink { get; set; }

        public static SiteConfiguration Load(string filePath)
        {
            var stream = File.OpenRead(filePath);
            var reader = new StreamReader(stream);

            var serializer = new YamlSerializer<SiteConfiguration>();
            return serializer.Deserialize(reader);
        }
    }
}