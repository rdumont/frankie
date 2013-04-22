using System;

namespace RDumont.Frankie.Core
{
    public abstract class TemplateManager
    {
        public const string TEMPLATES_FOLDER = "_templates";

        public static TemplateManager Current { get; private set; }

        public static void SetTemplateManager(TemplateManager templateManager)
        {
            Current = templateManager;
        }

        public abstract void Init(string basePath);

        public abstract void CompileTemplate(string templatePath, string contents);

        public abstract string RenderPage(string pagePath, Page model);

        public abstract string RenderPost(string postPath, string templateName, Post model);

        public static string GetFullPath(string name)
        {
            return TEMPLATES_FOLDER + "\\" + name + ".html";
        }
    }
}
