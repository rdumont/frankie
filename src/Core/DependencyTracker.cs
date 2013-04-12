using System;
using System.Collections.Generic;
using System.Linq;

namespace RDumont.Frankie.Core
{
    public class DependencyTracker
    {
        protected Dictionary<string, string> FileDependencies = new Dictionary<string, string>();
        protected Dictionary<string, HashSet<string>> DependentFiles = new Dictionary<string, HashSet<string>>();

        /// <summary>
        /// Finds files that depend on a resource
        /// </summary>
        /// <param name="name">The resource name on which the files depend</param>
        public string[] FindDependentFiles(string name)
        {
            HashSet<string> dependents;
            if (DependentFiles.TryGetValue(name, out dependents))
            {
                return dependents.ToArray();
            }
            return new string[0];
        }

        /// <summary>
        /// Finds the resource on which this file depends
        /// </summary>
        /// <param name="name">The dependent file</param>
        public string FindFileDependencies(string name)
        {
            string dependency;
            if (FileDependencies.TryGetValue(name, out dependency)) return dependency;
            return null;
        }

        /// <summary>
        /// Adds a file with its dependency
        /// </summary>
        /// <param name="dependentFile"></param>
        /// <param name="dependency"></param>
        public void Add(string dependentFile, string dependency)
        {
            if (dependency == null) return;
            try
            {
                this.FileDependencies.Add(dependentFile, dependency);
            }
            catch (ArgumentException)
            {
                return;
            }

            HashSet<string> dependentFiles;
            if (!DependentFiles.TryGetValue(dependency, out dependentFiles))
            {
                this.DependentFiles.Add(dependency, new HashSet<string> {dependentFile});
            }
            else
            {
                dependentFiles.Add(dependentFile);
            }
        }

        public void Remove(string name)
        {
            string dependsOn;
            if (!FileDependencies.TryGetValue(name, out dependsOn)) return;

            var dependentFiles = DependentFiles[dependsOn];
            dependentFiles.Remove(name);
            if (!dependentFiles.Any()) DependentFiles.Remove(dependsOn);

            FileDependencies.Remove(name);
        }
    }
}