using NUnit.Framework;
using TechTalk.SpecFlow;

namespace RDumont.Frankie.Specs.Steps
{
    [Binding]
    public class FileSteps : StepsBase
    {
        #region Given

        [Given(@"the '(.+)' text file")]
        public void Given_the_text_file(string filePath, string contents)
        {
            WriteFile(filePath, contents);
        }

        #endregion

        #region When

        [When(@"I create the file '(.+)'")]
        public void When_I_create_the_file(string path, string contents)
        {
            WriteFile(path, contents);
        }

        [When(@"I delete the file '(.+)'")]
        public void When_I_delete_the_file(string path)
        {
            DeleteFile(path);
        }

        #endregion

        #region Then

        [Then(@"there should be an '(.+)' text file")]
        [Then(@"there should be a '(.+)' text file")]
        public void There_should_be_a_text_file(string filePath, string contents)
        {
            var actualContents = ReadFile(filePath);

            Assert.That(actualContents, Is.Not.Null, "File '{0}' could not be found", filePath);
            Assert.That(Trimmed(actualContents), Is.EqualTo(Trimmed(contents)));
        }

        [Then(@"the file '(.+)' should not exist")]
        public void The_file_should_not_exist(string filePath)
        {
            var contents = ReadFile(filePath);

            Assert.That(contents, Is.Null);
        }

        #endregion
    }
}