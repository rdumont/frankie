using System;
using System.Collections.Generic;
using System.IO;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace RDumont.Frankie.Core
{
    public static class TemplateManager
    {
        private static readonly Dictionary<Type, string> TemplatePathsByType = new Dictionary<Type, string>();
        public const string TEMPLATES_FOLDER = "_templates";

        public static void Init()
        {
            Razor.SetTemplateService(new TemplateService(new TemplateServiceConfiguration
            {
                BaseTemplateType = typeof(PageTemplate<>)
            }));
        }

        public static void CompileTemplate(string templatePath, string contents)
        {
            var name = templatePath.Remove(0, TEMPLATES_FOLDER.Length + 1).Replace(".cshtml", "");
            var type = Razor.GetTemplate(contents, name).GetType();
            TemplatePathsByType.Add(type, name);
        }

        public static string RenderPage(string pagePath, string contents, Page model)
        {
            var name = pagePath.Replace(".cshtml", "");
            return Razor.Parse(contents, model, new DynamicViewBag(), name);
        }

        public static string RenderPost(string postPath, string templateName, Post model)
        {
            DependencyTracker.Current.Add(postPath, GetFullPath(templateName));
            return Razor.Run(templateName, model, new DynamicViewBag());
        }

        public static string GetTemplatePath(Type templateType)
        {
            string name;
            return TemplatePathsByType.TryGetValue(templateType, out name)
                ? GetFullPath(name)
                : null;
        }

        public static string GetFullPath(string name)
        {
            return TEMPLATES_FOLDER + "\\" + name + ".cshtml";
        }
    }
}
