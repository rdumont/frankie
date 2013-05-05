using NUnit.Framework;
using RDumont.Frankie.Core.Liquid;

namespace RDumont.Frankie.Tests.Liquid
{
    [TestFixture]
    public class FiltersTests
    {
        public class Summary
        {
            [Test]
            public void One_paragraph()
            {
                // Arrange
                var body = @"bla bla
<p>
    Lorem ipsum <strong>dolor sit</strong> amet,
    consectetur adipisicing elit, sed do eiusmod
    tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam,
    quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo
    consequat.
</p>

<p>
    Duis aute irure dolor in reprehenderit in voluptate velit esse
    cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non
    proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
</p>

la la la";

                // Act
                var summary = Filters.Summary(body, 30);

                // Assert
                Assert.That(summary, Is.EqualTo("Lorem ipsum dolor sit amet, consectetur (...)"));
            }
        }

        [Test]
        public void Shorter_than_the_limit()
        {
            // Arrange
            var body = @"bla bla
<p>
    Lorem ipsum <strong>dolor sit</strong> amet,
    consectetur adipisicing.
</p>

<p>
    Duis aute irure dolor in reprehenderit in voluptate velit esse
    cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non
    proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
</p>

la la la";

            // Act
            var summary = Filters.Summary(body, 50);

            // Assert
            Assert.That(summary,
                Is.EqualTo("Lorem ipsum dolor sit amet, consectetur adipisicing. (...)"));
        }
    }
}
