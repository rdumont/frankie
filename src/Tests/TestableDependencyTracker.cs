using System.Collections.Generic;
using RDumont.Frankie.Core;

namespace RDumont.Frankie.Tests
{
    public class TestableDependencyTracker : DependencyTracker
    {
        public new Dictionary<string, HashSet<string>> FileDependencies
        {
            get { return base.FileDependencies; }
        }

        public new Dictionary<string, HashSet<string>> DependentFiles
        {
            get { return base.DependentFiles; }
        }
    }
}