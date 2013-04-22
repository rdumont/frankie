using System.Collections.Specialized;
using NUnit.Framework;

namespace RDumont.Frankie.Tests
{
    [TestFixture]
    public class LiquidTemplateManagerTests
    {
        [Test]
        public void Wrap_page_with_template()
        {
            // Arrange
            var manager = new TestableLiquidTemplateManager();
            var page = new TestablePage
                {
                    Body = "Some body",
                    Metadata = new NameValueCollection {{"template", "some_template"}}
                };

            // Act
            manager.WrapWithTemplate(page);

            // Assert
            Assert.That(page.Body, Is.EqualTo(@"{% extends some_template %}
{% block contents %}
Some body
{% endblock %}
"));
        }
    }
}