using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace RDumont.Frankie.Specs.Steps
{
    [Binding]
    public class PostSteps : StepsBase
    {
        [Then(@"a post with slug ""(.+)"" should be registered")]
        public void Then_a_post_with_slug_should_be_registered(string slug)
        {
            var siteContext = Core.SiteContext.Current;

            var post = siteContext.Posts.FirstOrDefault(p => p.Slug == slug);
            Assert.That(post, Is.Not.Null, "No post registered with slug '{0}'", slug);
        }

        [Then(@"a post called ""(.+)"" should be registered")]
        public void Then_a_post_called_should_be_registered(string postTitle)
        {
            var siteContext = Core.SiteContext.Current;

            var post = siteContext.Posts.FirstOrDefault(p => p.Title == postTitle);
            Assert.That(post, Is.Not.Null, "No post registered with title '{0}'", postTitle);
        }

        [Then(@"the post '(.+)' should have the categories")]
        public void Then_the_post_should_have_the_categories(string slug, Table table)
        {
            var expectedCategories = table.Rows.Select(r => r[0]).ToArray();
            var post = Core.SiteContext.Current.Posts.FirstOrDefault(p => p.Slug == slug);
            Assert.That(post, Is.Not.Null, "No post registered with slug '{0}'", slug);

            Assert.That(post.Category, Is.EqualTo(expectedCategories));
        }
    }
}