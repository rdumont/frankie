﻿using System.Linq;
using NUnit.Framework;
using RDumont.Frankie.Specs.Testables;
using TechTalk.SpecFlow;

namespace RDumont.Frankie.Specs.Steps
{
    [Binding]
    public class LoggerSteps : StepsBase
    {
        [Given(@"the in-memory logger")]
        public void Given_the_in_memory_logger()
        {
            var logger = new TestLogger();
            TestLogger.Start(logger);
            Logger = logger;
        }

        [Then(@"no errors should be logged")]
        public void Then_no_errors_should_be_logged()
        {
            var errorCount = Logger.Errors.Count;
            var errorList = string.Join("\n\n", Logger.Errors.Select((e, i) => "==> Error " + i + "\n" + e.Message));

            Assert.That(errorCount, Is.EqualTo(0), "The following errors were logged:\n" + errorList);
        }
    }
}