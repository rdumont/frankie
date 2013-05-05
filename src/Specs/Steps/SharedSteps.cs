using System;
using System.IO;
using NUnit.Framework;
using RDumont.Frankie.CommandLine.Commands;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace RDumont.Frankie.Specs.Steps
{
    [Binding]
    public class SharedSteps : StepsBase
    {
        [BeforeScenario]
        public void SetUpFolders()
        {
            BasePath = Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString());
        }

        [AfterScenario]
        public void DeleteFolders()
        {
            Directory.Delete(BasePath, true);
        }

        [Given(@"the default directory structure")]
        public void Given_the_default_directory_structure()
        {
            CreateDirectory("_posts");
            CreateDirectory("_site");
            CreateDirectory("_templates");
            WriteFile("config.yaml", "Permalink: permalink_not_set");
        }

        [Given(@"the configuration file")]
        public void Given_the_configuration_file(Table table)
        {
            var configuration = table.CreateInstance<Core.SiteConfiguration>();
            var serialized = configuration.Serialize();
            WriteFile("config.yaml", serialized);
        }

        [Given(@"the '(.+)' text file")]
        public void Given_the_text_file(string filePath, string contents)
        {
            WriteFile(filePath, contents);
        }

        [When(@"I run Frankie")]
        public void When_I_run_Frankie()
        {
            var options = new BaseOptions
                {
                    Output = BasePath + "/_site",
                    Location = BasePath,
                };
            var command = new RunCommand();
            command.ExecuteCommand(options);
        }

        [When(@"I create the file '(.+)'")]
        public void When_I_create_the_file(string path, string contents)
        {
            WriteFile(path, contents);
        }

        [Then(@"there should be an '(.+)' text file")]
        [Then(@"there should be a '(.+)' text file")]
        public void There_should_be_a_text_file(string filePath, string contents)
        {
            var actualContents = ReadFile("_site/" + filePath);

            Assert.That(actualContents, Is.Not.Null, "File '{0}' could not be found", filePath);
            Assert.That(Trimmed(actualContents), Is.EqualTo(Trimmed(contents)));
        }
    }
}