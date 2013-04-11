using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;
using RDumont.Frankie.Core;

namespace RDumont.Frankie.Tests
{
    [TestFixture]
    public class PostTests
    {
        public class Creation
        {
            [Test]
            public void Create_post_from_file_path()
            {
                // Arrange
                var path = @"C:\something\_posts\something else\blog\_posts\2013-02-23-some-nice-article.md";

                // Act
                var post = new Post(path);

                // Assert
                Assert.That(post.Date, Is.EqualTo(new DateTime(2013, 2, 23)));
                Assert.That(post.Slug, Is.EqualTo("some-nice-article"));
                Assert.That(post.Extension, Is.EqualTo("md"));
            }

            [Test]
            public void Create_post_from_file_path_with_category()
            {
                // Arrange
                var path = @"C:\something\_posts\something else\blog\_posts\Cat1\2013-02-23-some-nice-article.md";

                // Act
                var post = new Post(path);

                // Assert
                Assert.That(post.Category, Is.EqualTo(new[]
                    {
                        "Cat1"
                    }));
            }

            [Test]
            public void Create_post_from_file_path_with_many_categories()
            {
                // Arrange
                var path =
                    @"C:\something\_posts\something else\blog\_posts\Cat1\sub cat\2013-02-23-some-nice-article.md";

                // Act
                var post = new Post(path);

                // Assert
                Assert.That(post.Category, Is.EqualTo(new[]
                    {
                        "Cat1",
                        "sub cat"
                    }));
            }

            [Test]
            public void Create_post_from_file_path_in_unix()
            {
                // Arrange
                var path = @"home/something/_posts/something else/blog/_posts/2013-02-23-some-nice-article.md";

                // Act
                var post = new Post(path);

                // Assert
                Assert.That(post.Date, Is.EqualTo(new DateTime(2013, 2, 23)));
                Assert.That(post.Slug, Is.EqualTo("some-nice-article"));
                Assert.That(post.Extension, Is.EqualTo("md"));
            }

            [Test]
            public void Resolve_permalink()
            {
                // Arrange
                var post = new TestablePost
                    {
                        Date = new DateTime(2012, 7, 22),
                        Slug = "my-other-post"
                    };

                // Act
                var permalink = post.ResolvePermalink(":year/:month/:day/:title");

                // Assert
                Assert.That(permalink, Is.EqualTo("/2012/07/22/my-other-post"));
            }
        }

        public class ReadMetaData
        {
            [Test]
            public void Read_metadata()
            {
                // Arrange
                var post = new TestablePost
                    {
                        Body = @"@template some-template
@otherValue ok
this is the body"
                    };

                // Act
                post.ReadMetadata();

                // Assert
                Assert.That(post.Metadata.AsDictionary(), Is.EquivalentTo(new Dictionary<string, string>
                    {
                        {"template", "some-template"},
                        {"otherValue", "ok"}
                    }));
                Assert.That(post.Body, Is.EqualTo(@"this is the body"));
            }
        }
    }

    public class TestablePost : Post
    {
        public new string ResolvePermalink(string template)
        {
            return base.ResolvePermalink(template);
        }

        public new void ReadMetadata()
        {
            base.ReadMetadata();
        }
    }
}
