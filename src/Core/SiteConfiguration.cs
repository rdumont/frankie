using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.RepresentationModel.Serialization;

namespace RDumont.Frankie.Core
{
    public class SiteConfiguration
    {
        private Regex[] _compiledExcludes;

        public string Url { get; set; }

        public string Permalink { get; set; }

        public string Culture { get; set; }

        public string[] Excludes { get; set; }

        public string[] TransformExtensions { get; set; }

        [YamlIgnore]
        public string SitePath { get; set; }

        [YamlIgnore]
        public string SourcePath { get; set; }

        public SiteConfiguration()
        {
            Excludes = new string[0];
            TransformExtensions = new[]
                {
                    "html"
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

        public bool IsExcluded(string path)
        {
            _compiledExcludes = _compiledExcludes ?? CompileExcludes().ToArray();

            for (var i = 0; i < _compiledExcludes.Length; i++)
                if (_compiledExcludes[i].IsMatch(path)) return true;

            return false;
        }

        public string GetRelativePath(string fullPath)
        {
            return fullPath.Remove(0, SourcePath.Length + 1);
        }

        public string GetFullPath(string relativePath)
        {
            return Path.Combine(SourcePath, relativePath);
        }

        /**
         * The code for this method was based on the DirectoryScanner.ConvertPatterns method from NAnt in:
         * https://github.com/nant/nant/blob/master/src/NAnt.Core/DirectoryScanner.cs
         */
        private IEnumerable<Regex> CompileExcludes()
        {
            for (int i = 0; i < Excludes.Length; i++)
            {
                var prePattern = Excludes[i];
                if (prePattern.StartsWith("*.") || prePattern.StartsWith("."))
                    prePattern = "**" + Path.DirectorySeparatorChar + prePattern;
                var pattern = new StringBuilder(prePattern);

                // The '\' character is a special character in regular expressions
                // and must be escaped before doing anything else.
                pattern.Replace(@"\", @"\\");
                
                // Escape the rest of the regular expression special characters.
                pattern.Replace(".", @"\.");
                pattern.Replace("$", @"\$");
                pattern.Replace("^", @"\^");
                pattern.Replace("{", @"\{");
                pattern.Replace("[", @"\[");
                pattern.Replace("(", @"\(");
                pattern.Replace(")", @"\)");
                pattern.Replace("+", @"\+");

                // Special case directory seperator string under Windows.
                string seperator = Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture);
                if (seperator == @"\")
                    seperator = @"\\";

                // Start with ? - it's used below
                pattern.Replace("?", "[^" + seperator + "]?");

                // SPECIAL CASE: any *'s directory between slashes or at the end of the
                // path are replaced with a 1..n pattern instead of 0..n: (?<=\\)\*(?=($|\\))
                // This ensures that C:\*foo* matches C:\foo and C:\* won't match C:.
                pattern = new StringBuilder(Regex.Replace(pattern.ToString(), "(?<=" + seperator + ")\\*(?=($|" + seperator + "))", "[^" + seperator + "]+"));

                // SPECIAL CASE: to match subdirectory OR current directory, If
                // we do this then we can write something like 'src/**/*.cs'
                // to match all the files ending in .cs in the src directory OR
                // subdirectories of src.
                pattern.Replace(seperator + "**" + seperator, seperator + "(.|?" + seperator + ")?");
                pattern.Replace("**" + seperator, ".|(?<=^|" + seperator + ")");
                pattern.Replace(seperator + "**", "(?=$|" + seperator + ").|");

                // .| is a place holder for .* to prevent it from being replaced in next line
                pattern.Replace("**", ".|");
                pattern.Replace("*", "[^" + seperator + "]*");
                pattern.Replace(".|", ".*"); // replace place holder string

                // Help speed up the search
                if (pattern.Length > 0)
                {
                    pattern.Insert(0, '^'); // start of line
                    pattern.Append('$'); // end of line
                }

                string patternText = pattern.ToString();
                if (patternText.StartsWith("^.*"))
                    patternText = patternText.Substring(3);
                if (patternText.EndsWith(".*$"))
                    patternText = patternText.Substring(0, pattern.Length - 3);

                yield return new Regex(patternText, RegexOptions.Compiled);
            }
        }
    }
}