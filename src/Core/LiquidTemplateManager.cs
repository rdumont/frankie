using System.Collections.Generic;
using System.IO;
using DotLiquid;

namespace RDumont.Frankie.Core
{
    public class LiquidTemplateManager : TemplateManager
    {
        private readonly Dictionary<string, Template> templatesByName = new Dictionary<string, Template>();

        public override void Init(string basePath)
        {
            Template.FileSystem = new TemplatesFileSystem(basePath);
        }

        public override void CompileTemplate(string templatePath, string contents)
        {
            var name = templatePath.Remove(0, TEMPLATES_FOLDER.Length + 1).Replace(".html", "");
            templatesByName[name] = Template.Parse(contents);
        }

        public override string RenderPage(string pagePath, Page model)
        {
            WrapWithTemplate(model);

            var hash = Hash.FromAnonymousObject(new
                {
                    path = pagePath,
                    posts = SiteContext.Current.Posts
                });
            return Template.Parse(model.Body).Render(hash);
        }

        protected virtual void WrapWithTemplate(ContentFile model)
        {
            var template = model.Metadata["template"];
            if (template == null) return;

            var bodyWriter = new StringWriter();
            bodyWriter.WriteLine("{{% extends {0} -%}}", template);
            bodyWriter.WriteLine("{{% block {0}_contents -%}}", template);
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

            var hash = Hash.FromAnonymousObject(new
                {
                    post = model
                });
            return template.Render(hash);
        }

        public override string PrepareTemplateContents(string contents, Context context, string templateName)
        {
            var templatePage = new TemplatePage(contents);

            templatePage.Body = templatePage.Body.Replace("{{ contents }}",
                string.Format("{{% block {0}_contents -%}}{{% endblock -%}}", templateName));

            WrapWithTemplate(templatePage);

            return templatePage.Body;
        }
    }
}