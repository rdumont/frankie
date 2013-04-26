﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.18033
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace RDumont.Frankie.Specs.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Markdown page transformation")]
    public partial class MarkdownPageTransformationFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Markdown page transformation.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Markdown page transformation", "In order to save time on writing complex markup\r\nAs a site owner\r\nI want to gener" +
                    "ate a page using Markdown", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 6
#line 7
 testRunner.Given("the default directory structure", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 8
  testRunner.And("the in-memory logger", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Page with default template")]
        public virtual void PageWithDefaultTemplate()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Page with default template", ((string[])(null)));
#line 10
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
#line 11
 testRunner.Given("the \'_page\' template", "This is a page.\r\n{{ contents }}", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 16
  testRunner.And("the \'my-page.md\' text file", "**My page**", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 20
 testRunner.When("I run Frankie", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 21
 testRunner.Then("there should be a \'my-page.html\' text file", "This is a page.\r\n<p><strong>My page</strong></p>", ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 26
  testRunner.And("no errors should be logged", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Page with custom template")]
        public virtual void PageWithCustomTemplate()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Page with custom template", ((string[])(null)));
#line 28
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
#line 29
 testRunner.Given("the \'layout\' template", "This is the layout.\r\n{{ contents }}", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 34
  testRunner.And("the \'my-page.md\' text file", "@template layout\r\n**My page**", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 39
 testRunner.When("I run Frankie", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 40
 testRunner.Then("there should be a \'my-page.html\' text file", "This is the layout.\r\n<p><strong>My page</strong></p>", ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 45
  testRunner.And("no errors should be logged", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Page inside folder")]
        public virtual void PageInsideFolder()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Page inside folder", ((string[])(null)));
#line 47
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
#line 48
 testRunner.Given("the \'_page\' template", "{{ contents }}", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 52
  testRunner.And("the \'about/me.md\' text file", "# About me", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 56
 testRunner.When("I run Frankie", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 57
 testRunner.Then("there should be a \'about/me.html\' text file", "<h1 id=\"about-me\">About me</h1>", ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 61
  testRunner.And("no errors should be logged", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Page with embedded liquid syntax")]
        public virtual void PageWithEmbeddedLiquidSyntax()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Page with embedded liquid syntax", ((string[])(null)));
#line 63
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
#line 64
 testRunner.Given("the \'_page\' template", "{{ contents }}", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 68
  testRunner.And("the \'gravatar\' template", "This is a **gravatar** include", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 72
  testRunner.And("the \'liquid-page.md\' text file", "The page.\r\n\r\n{% include gravatar %}", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 78
 testRunner.When("I run Frankie", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 79
 testRunner.Then("there should be a \'liquid-page.html\' text file", "<p>The page.</p>\r\n<p>This is a <strong>gravatar</strong> include</p>", ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 84
  testRunner.And("no errors should be logged", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Code block with language")]
        public virtual void CodeBlockWithLanguage()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Code block with language", ((string[])(null)));
#line 86
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
#line 87
 testRunner.Given("the \'_page\' template", "{{ contents }}", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 91
 testRunner.And("the \'code-page.md\' text file", "Some code.\r\n\r\n```csharp\r\npublic class Foo { }\r\n```", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 99
 testRunner.When("I run Frankie", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 100
 testRunner.Then("there should be a \'code-page.html\' text file", "<p>Some code.</p>\r\n<pre><code data-language=\"csharp\">public class Foo { }\r\n</code" +
                    "></pre>", ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 106
  testRunner.And("no errors should be logged", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
