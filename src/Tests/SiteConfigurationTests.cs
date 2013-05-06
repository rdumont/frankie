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
    }
}