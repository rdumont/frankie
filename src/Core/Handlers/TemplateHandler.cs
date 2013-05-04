using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RDumont.Frankie.Core.Handlers
{
    public class TemplateHandler : IAssetHandler
    {
        private readonly Generator _generator;

        public TemplateHandler(Generator generator)
        {
            _generator = generator;
        }

        public bool Matches(string path)
        {
            return path.StartsWith(TemplateManager.TEMPLATES_FOLDER);
        }

        public void Handle(string path)
        {
            var dependentFiles = DependencyTracker.Current.FindAllDependentFiles(path);

            try
            {
                _generator.CompileTemplate(path);
            }
            catch (FileNotFoundException)
            {
                // ok, probably a temp file
            }

            foreach (var dependentFile in dependentFiles)
            {
                _generator.ReAddDependentFile(dependentFile);
            }

            _generator.UpdatePostsCollection();
        }
    }
}
