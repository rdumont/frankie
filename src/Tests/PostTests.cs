using NUnit.Framework;
using RDumont.Frankie.Core;

namespace RDumont.Frankie.Tests
{
    [TestFixture]
    public class PostTests
    {
        [Test]
        public void Create_post_from_file_path()
        {
            // Arrange
            var path = @"C:\something\_posts\something else\blog\_posts\2013-02-23-some-nice-article.md";

            // Act
            var post = new Post(path);

            // Assert
            Assert.That(post.Year, Is.EqualTo(2013));
            Assert.That(post.Month, Is.EqualTo(2));
            Assert.That(post.Day, Is.EqualTo(23));
            Assert.That(post.Title, Is.EqualTo("some-nice-article"));
            Assert.That(post.Extension, Is.EqualTo("md"));
        }

        [Test]
        public void Create_post_from_file_path_in_unix()
        {
            // Arrange
            var path = @"home/something/_posts/something else/blog/_posts/2013-02-23-some-nice-article.md";

            // Act
            var post = new Post(path);

            // Assert
            Assert.That(post.Year, Is.EqualTo(2013));
            Assert.That(post.Month, Is.EqualTo(2));
            Assert.That(post.Day, Is.EqualTo(23));
            Assert.That(post.Title, Is.EqualTo("some-nice-article"));
            Assert.That(post.Extension, Is.EqualTo("md"));
        }

        [Test]
        public void Resolve_permalink()
        {
            // Arrange
            var post = new TestablePost
                {
                    Year = 2012,
                    Month = 7,
                    Day = 22,
                    Title = "my-other-post"
                };

            // Act
            var permalink = post.ResolvePermalink(":year/:month/:day/:title");

            // Assert
            Assert.That(permalink, Is.EqualTo("2012/07/22/my-other-post"));
        }
    }

    public class TestablePost : Post
    {
    }
}
