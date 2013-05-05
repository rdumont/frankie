﻿using System.Threading;
using RDumont.Frankie.CommandLine.Commands;
using TechTalk.SpecFlow;

namespace RDumont.Frankie.Specs.Steps
{
    [Binding]
    public class WatchSteps : StepsBase
    {
        [Given(@"that Frankie is watching my folder")]
        public void Given_that_Frankie_is_watching_my_folder()
        {
            var options = new WatchOptions
                {
                    Output = BasePath + "/_site",
                    Location = BasePath,
                    DontWaitForCommand = true
                };
            var command = new WatchCommand();
            WatchCommand = command;
            command.ExecuteCommand(options);
        }

        [When(@"wait for the watcher to finish")]
        public void Wait_for_the_watcher_to_finish()
        {
            while (!WatchCommand.IsIdle)
            {
                Thread.Sleep(10);
            }
        }
    }
}