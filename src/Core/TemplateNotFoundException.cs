using System;

namespace RDumont.Frankie.Core
{
    public class TemplateNotFoundException : Exception
    {
        public TemplateNotFoundException(string templateName)
            : base(string.Format("The template '{0}' does not exist", templateName))
        {
        }
    }
}