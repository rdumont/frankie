using System.Collections.Specialized;
using DotLiquid;
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
            Assert.That(page.Body, Is.EqualTo(@"{% extends some_template -%}
{% block some_template_content -%}
Some body
{% endblock -%}
"));
        }

        [Test]
        public void Prepare_template_contents()
        {
            // Arrange
            var manager = new TestableLiquidTemplateManager();
            var context = new Context();

            // Act
            var result = manager.PrepareTemplateContents(@"a
{{ content }}
b", context, "layout");

            // Assert
            Assert.That(result, Is.EqualTo(@"a
{% block layout_content -%}{% endblock -%}
b"));
        }

        [Test]
        public void Prepare_subtemplate_contents()
        {
            // Arrange
            var manager = new TestableLiquidTemplateManager();
            var context = new Context();

            // Act
            var result = manager.PrepareTemplateContents(@"@template base
a
{{ content }}
b", context, "layout");

            // Assert
            Assert.That(result, Is.EqualTo(@"{% extends base -%}
{% block base_content -%}
a
{% block layout_content -%}{% endblock -%}
b
{% endblock -%}
"));
        }
    }
}