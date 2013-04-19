﻿using System;
using System.Collections.Generic;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace RDumont.Frankie.Core
{
    public class RazorTemplateManager : TemplateManager
    {
        private static TemplateService _templateService;
        private static readonly Dictionary<Type, string> TemplatePathsByType = new Dictionary<Type, string>();

        public override void Init()
        {
            _templateService = new TemplateService(new TemplateServiceConfiguration
                {
                    BaseTemplateType = typeof(PageTemplate<>)
                });
            Razor.SetTemplateService(_templateService);
        }

        public override void CompileTemplate(string templatePath, string contents)
        {
            var name = templatePath.Remove(0, TEMPLATES_FOLDER.Length + 1).Replace(".html", "");
            var type = Razor.GetTemplate(contents, name).GetType();
            try
            {
                TemplatePathsByType.Add(type, name);
            }
            catch (ArgumentException)
            {
                // ok, template is already registered
            }
        }

        public override string RenderPage(string pagePath, string contents, Page model)
        {
            var template = Razor.CreateTemplate(contents, model);
            var viewBag = new DynamicViewBag();
            viewBag.AddValue("PagePath", pagePath);
            return _templateService.Run(template, viewBag);
        }

        public override string RenderPost(string postPath, string templateName, Post model)
        {
            DependencyTracker.Current.Add(postPath, GetFullPath(templateName));
            return Razor.Run(templateName, model, new DynamicViewBag());
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