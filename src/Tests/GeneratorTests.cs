using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RDumont.Frankie.Core;

namespace RDumont.Frankie.Tests
{
    [TestFixture]
    public class GeneratorTests
    {
        public static string FullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        [Test]
        public void Get_markdown_page_destination_path()
        {
            // Arrange
            var generator = new TestableGenerator
                {
                    BasePath = FullPath("my/blog"),
                    SitePath = FullPath("my/blog/final"),
                    PostsPath = FullPath("my/blog/_posts")
                };

            // Act
            var destination = generator.GetFileDestinationPath(FullPath("my/blog/about/somepage.md"));

            // Assert
            Assert.That(destination, Is.EqualTo(FullPath("my/blog/final/about/somepage.html")));
        }

        [Test]
        public void Get_post_destination_path()
        {
            // Arrange
            var generator = new TestableGenerator
                {
                    SitePath = FullPath("my/blog/final"),
                    PostsPath = FullPath("my/blog/_posts"),
                    Configuration = new SiteConfiguration
                        {
                            Permalink = ":year/:month/:day/:title"
                        }
                };

            // Act
            var destination = generator.GetFileDestinationPath(FullPath("my/blog/_posts/2013-05-10-some-post.md"));

            // Assert
            Assert.That(destination, Is.EqualTo(FullPath("my/blog/final/2013/05/10/some-post/index.html")));
        }

        [Test]
        public void Get_asset_destination_path()
        {
            // Arrange
            var generator = new TestableGenerator
            {
                BasePath = FullPath("my/blog"),
                SitePath = FullPath("my/blog/final"),
                PostsPath = FullPath("my/blog/_posts"),
                Configuration = new SiteConfiguration
                {
                    Permalink = ":year/:month/:day/:title"
                }
            };

            // Act
            var destination = generator.GetFileDestinationPath(FullPath("my/blog/images/logo.png"));

            // Assert
            Assert.That(destination, Is.EqualTo(FullPath("my/blog/final/images/logo.png")));
        }
    }
}
