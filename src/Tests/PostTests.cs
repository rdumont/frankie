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
                Assert.That(post.OriginalPath, Is.EqualTo("_posts/2013-02-23-some-nice-article.md"));
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

        public class ExtractMetadata
        {
            [Test]
            public void Extract_metadata()
            {
                // Arrange
                var post = new TestablePost
                    {
                        Body = @"@template some-template
@otherValue ok
this is the body"
                    };

                // Act
                post.ExtractMetadata();

                // Assert
                Assert.That(post.Metadata.AsDictionary(), Is.EquivalentTo(new Dictionary<string, string>
                    {
                        {"template", "some-template"},
                        {"otherValue", "ok"}
                    }));
                Assert.That(post.Body, Is.EqualTo(@"this is the body" + Environment.NewLine));
            }
        }

        [Test]
        public void Load_tags()
        {
            // Arrange
            var post = new TestablePost();
            post.Metadata = new NameValueCollection
                {
                    {"tags", "life, stuff, more stuff"}
                };

            // Act
            var tags = post.LoadTags();

            // Assert
            Assert.That(tags, Is.EquivalentTo(new[]
                {
                    "life",
                    "stuff",
                    "more stuff",
                }));
        }

        public class RetrieveTitle
        {
            [Test]
            public void Retrieve_post_title()
            {
                // Arrange
                var post = new TestablePost();
                post.Body = @"
<h1>This is the title</h1>
<p>This is a paragraph</p>";

                // Act
                post.RetrieveTitle();

                // Assert
                Assert.That(post.Title, Is.EqualTo("This is the title"));
                Assert.That(post.Body, Is.EqualTo(@"

<p>This is a paragraph</p>"));
            }

            [Test]
            public void Retrieve_post_title_when_h1_has_attributes()
            {
                // Arrange
                var post = new TestablePost();
                post.Body = @"
<h1 id=""this-is-the-title"">This is the title</h1>
<p>This is a paragraph</p>";

                // Act
                post.RetrieveTitle();

                // Assert
                Assert.That(post.Title, Is.EqualTo("This is the title"));
                Assert.That(post.Body, Is.EqualTo(@"

<p>This is a paragraph</p>"));
            }

            [Test]
            public void Retrieve_post_title_with_many_h1_elements()
            {
                // Arrange
                var post = new TestablePost();
                post.Body = @"
<h1>This is the title</h1>
<h1>This is not the title</h1>
<p>This is a paragraph</p>";

                // Act
                post.RetrieveTitle();

                // Assert
                Assert.That(post.Title, Is.EqualTo("This is the title"));
                Assert.That(post.Body, Is.EqualTo(@"

<h1>This is not the title</h1>
<p>This is a paragraph</p>"));
            }
        }
    }

    public class TestablePost : Post
    {
        public new NameValueCollection Metadata
        {
            get { return base.Metadata; }
            set { base.Metadata = value; }
        }

        public new string ResolvePermalink(string template)
        {
            return base.ResolvePermalink(template);
        }

        public new void ExtractMetadata()
        {
            base.ExtractMetadata();
        }

        public new void RetrieveTitle()
        {
            base.RetrieveTitle();
        }

        public new string[] LoadTags()
        {
            return base.LoadTags();
        }
    }
}
