using System;
using System.IO;
using System.Threading;
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
            BeforeCleanUp();
            try
            {
                Directory.Delete(BasePath, true);
            }
            catch (IOException)
            {
                Thread.Sleep(10);
                Directory.Delete(BasePath, true);
            }
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
    }
}