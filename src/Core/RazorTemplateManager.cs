using System;
using System.Collections.Generic;
using DotLiquid;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace RDumont.Frankie.Core
{
    public class RazorTemplateManager : TemplateManager
    {
        private static TemplateService _templateService;
        private static readonly Dictionary<Type, string> TemplatePathsByType = new Dictionary<Type, string>();

        public override void Init(string basePath)
        {
            _templateService = new TemplateService(new TemplateServiceConfiguration
                {
                    BaseTemplateType = typeof(PageTemplate<>)
                });
            Razor.SetTemplateService(_templateService);
        }

        public override void CompileTemplate(string templatePath)
        {
            var name = templatePath.Remove(0, TEMPLATES_FOLDER.Length + 1).Replace(".html", "");
            var fullPath = GetFullPath(templatePath);
            var type = Razor.GetTemplate(fullPath, name).GetType();
            try
            {
                TemplatePathsByType.Add(type, name);
            }
            catch (ArgumentException)
            {
                // ok, template is already registered
            }
        }

        public override string RenderPage(string pagePath, Page model)
        {
            var template = Razor.CreateTemplate(model.Body, model);
            var viewBag = new DynamicViewBag();
            viewBag.AddValue("PagePath", pagePath);
            return _templateService.Run(template, viewBag);
        }

        public override string RenderPost(string postPath, string templateName, Post model)
        {
            DependencyTracker.Current.Add(postPath, GetFullPath(templateName));
            return Razor.Run(templateName, model, new DynamicViewBag());
        }

        public override string PrepareTemplateContents(string contents, Context context, string templateName)
        {
            return contents;
        }

        public override string RenderMarkdownPage(string pagePath, string template, Page page)
        {
            throw new NotImplementedException();
        }

        public string GetTemplatePath(Type templateType)
        {
            string name;
            return TemplatePathsByType.TryGetValue(templateType, out name)
                ? GetFullPath(name)
                : null;
        }
    }
}