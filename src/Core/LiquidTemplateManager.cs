using System.Collections.Generic;
using DotLiquid;

namespace RDumont.Frankie.Core
{
    public class LiquidTemplateManager : TemplateManager
    {
        private readonly Dictionary<string, Template> templatesByName = new Dictionary<string, Template>();

        public override void Init()
        {
        }

        public override void CompileTemplate(string templatePath, string contents)
        {
            var name = templatePath.Remove(0, TEMPLATES_FOLDER.Length + 1).Replace(".html", "");
            templatesByName[name] = Template.Parse(contents);
        }

        public override string RenderPage(string pagePath, string contents, Page model)
        {
            var hash = Hash.FromAnonymousObject(new
                {
                    path = pagePath,
                    posts = SiteContext.Current.Posts
                });
            return Template.Parse(contents).Render(hash);
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
    }
}