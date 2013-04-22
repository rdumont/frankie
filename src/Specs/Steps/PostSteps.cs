using System.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow;

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
    }
}