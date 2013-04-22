using TechTalk.SpecFlow;

namespace RDumont.Frankie.Specs.Steps
{
    [Binding]
    public class TemplateSteps : StepsBase
    {
        [Given(@"the '(.+)' template")]
        public void Given_the_template(string templateName, string contents)
        {
            WriteFile("_templates/" + templateName + ".html", contents);
        }
    }
}
