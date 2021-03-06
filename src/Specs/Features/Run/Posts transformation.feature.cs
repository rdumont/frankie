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
namespace RDumont.Frankie.Specs.Features.Run
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Posts transformation")]
    public partial class PostsTransformationFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Posts transformation.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Posts transformation", "In order to share my ideas\r\nAs a site owner\r\nI want to write posts in markdown fo" +
                    "rmat", ProgrammingLanguage.CSharp, ((string[])(null)));
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
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table1.AddRow(new string[] {
                        "Permalink",
                        ":year/:month/:day/:title"});
#line 8
  testRunner.And("the configuration file", ((string)(null)), table1, "And ");
#line 11
  testRunner.And("the in-memory logger", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 12
  testRunner.And("the \'_post\' template", "{{ post.body }}", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Transform a post")]
        public virtual void TransformAPost()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Transform a post", ((string[])(null)));
#line 17
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
#line 18
 testRunner.Given("the \'_posts/2013-04-25-some-nice-post.md\' text file", "Here is a paragraph.", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 22
 testRunner.When("I run Frankie", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 23
 testRunner.Then("a post with slug \"some-nice-post\" should be registered", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 24
  testRunner.And("there should be a \'_site/2013/04/25/some-nice-post/index.html\' text file", "<p>Here is a paragraph.</p>", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Extract title from post")]
        public virtual void ExtractTitleFromPost()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Extract title from post", ((string[])(null)));
#line 29
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
#line 30
 testRunner.Given("the \'_posts/2013-04-25-some-nice-post.md\' text file", "# This is a nice post\r\n\r\nAnd here is a paragraph.", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 36
 testRunner.When("I run Frankie", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 37
 testRunner.Then("a post called \"This is a nice post\" should be registered", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("A post with a category")]
        public virtual void APostWithACategory()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("A post with a category", ((string[])(null)));
#line 39
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
#line 40
 testRunner.Given("the \'_posts/Food for Thought/2013-04-25-some-nice-post.md\' text file", "This is a post", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 44
 testRunner.When("I run Frankie", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Category"});
            table2.AddRow(new string[] {
                        "Food for Thought"});
#line 45
 testRunner.Then("the post \'some-nice-post\' should have the categories", ((string)(null)), table2, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("A post with many categories")]
        public virtual void APostWithManyCategories()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("A post with many categories", ((string[])(null)));
#line 49
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
#line 50
 testRunner.Given("the \'_posts/Food for Thought/Acentuação/2013-04-25-some-nice-post.md\' text file", "This is a post", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 54
 testRunner.When("I run Frankie", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "Category"});
            table3.AddRow(new string[] {
                        "Food for Thought"});
            table3.AddRow(new string[] {
                        "Acentuação"});
#line 55
 testRunner.Then("the post \'some-nice-post\' should have the categories", ((string)(null)), table3, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
