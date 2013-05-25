using System.IO;
using NUnit.Framework;
using RDumont.Frankie.Core;

namespace RDumont.Frankie.Tests
{
    [TestFixture]
    public class SiteConfigurationTests
    {
        public class IsExcluded
        {
            [TestCase("**/*.abc", "path/to/some/file.abc")]
            [TestCase("*.abc", "path/to/some/file.abc")]
            [TestCase("*.abc", "file.abc")]
            [TestCase(".abc", ".abc")]
            [TestCase(".abc", "path/to/.abc")]
            [TestCase("**/file.abc", "path/to/some/file.abc")]
            [TestCase("file.abc", "file.abc")]
            [TestCase("**/some/file.abc", "path/to/some/file.abc")]
            [TestCase("some/file.abc", "some/file.abc")]
            [TestCase("path/**/*.abc", "path/to/some/file.abc")]
            [TestCase("path/**/*.abc", "path/file.abc")]
            public void Verify_that_file_is_excluded(string pattern, string path)
            {
                // Arrange
                path = path.Replace('/', Path.DirectorySeparatorChar);
                var configuration = SiteConfigurationExcluding(pattern);

                // Act
                var excluded = configuration.IsExcluded(path);

                // Assert
                Assert.That(excluded, Is.True);
            }

            [TestCase("*.abc", "path/to/some/file.def")]
            [TestCase("*.abc", "file.def")]
            [TestCase("*.abc", ".def")]
            [TestCase("*/file.abc", "file.abc")]
            [TestCase("file.abc", "path/to/otherfile.abc")]
            [TestCase("file.abc", "otherfile.abc")]
            [TestCase("some/file.abc", "path/to/some/file.abc")]
            [TestCase("some/file.abc", "other/file.abc")]
            [TestCase("path/**/*.abc", "path/to/some/file.def")]
            [TestCase("path/**/*.abc", "some/file.abc")]
            public void Verify_that_file_is_not_excluded(string pattern, string path)
            {
                // Arrange
                path = path.Replace('/', Path.DirectorySeparatorChar);
                var configuration = SiteConfigurationExcluding(pattern);

                // Act
                var excluded = configuration.IsExcluded(path);

                // Assert
                Assert.That(excluded, Is.False);
            }

            public SiteConfiguration SiteConfigurationExcluding(string exclude)
            {
                return new SiteConfiguration
                    {
                        Excludes = new[] {exclude}
                    };
            }
        }

        public class GetRelativePath
        {
            [TestCase(@"C:\documents\blog\source\_site\index.html", @"_site\index.html")]
            [TestCase(@"C:\documents\blog\source\_site\file with spaces.html", @"_site\file with spaces.html")]
            [TestCase(@"C:\documents\blog\site\index.html", @"..\site\index.html")]
            [TestCase(@"C:\some\other\path\bla.txt", @"..\..\..\some\other\path\bla.txt")]
            public void Windows_paths(string toPath, string expectedRelativePath)
            {
                // Arrange
                var configuration = new SiteConfiguration
                {
                    SourcePath = @"C:\documents\blog\source"
                };
                expectedRelativePath = expectedRelativePath.Replace("\\", Path.DirectorySeparatorChar.ToString());

                // Act
                var relativePath = configuration.GetRelativePath(toPath);

                // Assert
                Assert.That(relativePath, Is.EqualTo(expectedRelativePath));
            }

            [TestCase(@"/home/blog/source/_site/index.html", @"_site/index.html")]
            [TestCase(@"/home/blog/site/index.html", @"../site/index.html")]
            [TestCase(@"/some/other/path/bla.txt", @"../../../some/other/path/bla.txt")]
            public void Unix_paths(string toPath, string expectedRelativePath)
            {
                // Arrange
                var configuration = new SiteConfiguration
                {
                    SourcePath = @"/home/blog/source"
                };
                expectedRelativePath = expectedRelativePath.Replace("/", Path.DirectorySeparatorChar.ToString());

                // Act
                var relativePath = configuration.GetRelativePath(toPath);

                // Assert
                Assert.That(relativePath, Is.EqualTo(expectedRelativePath));
            }
        }
    }
}