using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotLiquid;
using RDumont.Frankie.Core.Liquid;

namespace RDumont.Frankie.Core
{
    public class LiquidTemplateManager : TemplateManager
    {
        private SiteDrop _siteDrop;
        private readonly Regex _titleRegex = new Regex(@"<h1(?:.+)?>(.+)</h1>", RegexOptions.Compiled);
        private readonly Dictionary<string, Template> templatesByName = new Dictionary<string, Template>();

        public override void Init(SiteConfiguration configuration)
        {
            Template.FileSystem = new TemplatesFileSystem(configuration.SourcePath);
            Template.RegisterFilter(typeof(LiquidFilters));
            _siteDrop = new SiteDrop(configuration);
        }

        public override void CompileTemplate(string templatePath)
        {
            var name = GetTemplateName(templatePath);
            var context = new Context();
            var contents = Template.FileSystem.ReadTemplateFile(context, name);
            templatesByName[name] = Template.Parse(contents);
        }

        private static string GetTemplateName(string templatePath)
        {
            var name = templatePath.Remove(0, TEMPLATES_FOLDER.Length + 1).Replace(".html", "");
            return name;
        }

        private Hash CreateHash(object anonymousObject)
        {
            var hash = Hash.FromAnonymousObject(anonymousObject);
            hash["frankie"] = new FrankieDrop();
            hash["site"] = _siteDrop;
            return hash;
        }

        public override string RenderPage(string pagePath, Page model)
        {
            WrapWithTemplate(model);

            var hash = CreateHash(new
                {
                    title = _titleRegex.Match(model.Body).Groups[1].Value,
                    dependantPath = pagePath,
                    path = pagePath,
                    posts = SiteContext.Current.Posts
                });
            return Template.Parse(model.Body).Render(hash);
        }

        protected virtual void WrapWithTemplate(ContentFile model)
        {
            var template = model.Metadata["template"];
            if (template == null) return;

            WrapWithTemplate(model, template);
        }

        protected virtual void WrapWithTemplate(ContentFile model, string templateName)
        {
            var bodyWriter = new StringWriter();
            bodyWriter.WriteLine("{{% extends {0} -%}}", templateName);
            bodyWriter.WriteLine("{{% block {0}_content -%}}", templateName);
            bodyWriter.WriteLine(model.Body);
            bodyWriter.WriteLine("{% endblock -%}");

            model.Body = bodyWriter.ToString();
        }

        public override string RenderPost(string postPath, string templateName, Post model)
        {
            DependencyTracker.Current.Add(postPath, GetFullPath(templateName));
            Template template;
            if (!templatesByName.TryGetValue(templateName, out template))
                throw new TemplateNotFoundException(templateName);

            var hash = CreateHash(new
                {
                    title = model.Title,
                    dependantPath = postPath,
                    path = postPath,
                    post = model
                });
            return template.Render(hash);
        }

        public override string RenderMarkdownPage(string pagePath, string templateName, Page page)
        {
            DependencyTracker.Current.Add(pagePath, GetFullPath(templateName));

            var siteContext = SiteContext.Current;

            var hash = CreateHash(new
                {
                    dependantPath = pagePath,
                    path = pagePath,
                    posts = siteContext.Posts
                });

            page.Body = Template.Parse(page.Body).Render(hash);
            page.TransformMarkdown();
            WrapWithTemplate(page, templateName);

            hash.Add("title", _titleRegex.Match(page.Body).Groups[1].Value);

            return Template.Parse(page.Body).Render(hash);
        }

        public override void RemoveTemplate(string path)
        {
            var dependencies = DependencyTracker.Current.FindAllDependentFiles(path);
            if(dependencies.Any())
                throw new InvalidOperationException("Cannot remove template because it is still in use.");

            DependencyTracker.Current.Remove(path);

            var name = GetTemplateName(path);
            templatesByName.Remove(name);
        }

        public override string PrepareTemplateContents(string content, Context context, string templateName)
        {
            var templatePage = new TemplatePage(content);

            templatePage.Body = templatePage.Body.Replace("{{ content }}",
                string.Format("{{% block {0}_content -%}}{{% endblock -%}}", templateName));

            WrapWithTemplate(templatePage);

            return templatePage.Body;
        }
    }
}