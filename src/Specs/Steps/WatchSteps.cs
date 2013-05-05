using System;
using System.Threading;
using RDumont.Frankie.CommandLine.Commands;
using TechTalk.SpecFlow;

namespace RDumont.Frankie.Specs.Steps
{
    [Binding]
    public class WatchSteps : StepsBase
    {
        static WatchSteps()
        {
            OnBeforeCleanUp += () =>
                {
                    if(ScenarioContext.Current.ContainsKey(typeof(WatchCommand).FullName))
                        WatchCommand.Stop();
                };
        }

        [Given(@"that Frankie is watching my folder")]
        public void Given_that_Frankie_is_watching_my_folder()
        {
            var options = new WatchOptions
                {
                    Output = BasePath + "/_site",
                    Source = BasePath,
                    DontWaitForCommand = true
                };
            var command = new WatchCommand();
            WatchCommand = command;
            command.ExecuteCommand(options);
        }

        [When(@"wait for the watcher to finish")]
        public void Wait_for_the_watcher_to_finish()
        {
            const int timeout = 1000;
            const int step = 10;
            var elapsed = 0;
            while (!WatchCommand.IsIdle)
            {
                if (elapsed >= timeout)
                    throw new TimeoutException("Watch command did not complete after " + timeout + "ms");

                Thread.Sleep(step);
                elapsed += step;
            }
        }
    }
}