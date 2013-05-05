using System;
using System.Collections.Generic;
using System.Linq;

namespace RDumont.Frankie.Core
{
    public class DependencyTracker
    {
        private object _fileDependenciesLock = new object();
        private object _dependentFilesLock = new object();

        protected Dictionary<string, HashSet<string>> FileDependencies = new Dictionary<string, HashSet<string>>();
        protected Dictionary<string, HashSet<string>> DependentFiles = new Dictionary<string, HashSet<string>>();

        private static DependencyTracker _instance;
        public static DependencyTracker Current
        {
            get { return _instance ?? (_instance = new DependencyTracker()); }
        }

        protected DependencyTracker()
        {
        }

        /// <summary>
        /// Finds files that depend on a resource
        /// </summary>
        /// <param name="name">The resource name on which the files depend</param>
        public string[] FindDependentFiles(string name)
        {
            lock (_dependentFilesLock)
            {
                HashSet<string> dependents;
                if (DependentFiles.TryGetValue(name, out dependents))
                {
                    return dependents.ToArray();
                }
                return new string[0];
            }
        }

        /// <summary>
        /// Finds the resource on which this file depends
        /// </summary>
        /// <param name="name">The dependent file</param>
        public string[] FindFileDependencies(string name)
        {
            lock (_fileDependenciesLock)
            {
                HashSet<string> dependencies;
                if (FileDependencies.TryGetValue(name, out dependencies)) return dependencies.ToArray();
                return null;
            }
        }

        /// <summary>
        /// Adds a file with its dependency
        /// </summary>
        /// <param name="dependentFile"></param>
        /// <param name="dependency"></param>
        public void Add(string dependentFile, string dependency)
        {
            if (dependency == null) return;

            lock (_fileDependenciesLock)
            {
                try
                {
                    FileDependencies.Add(dependentFile, new HashSet<string> {dependency});
                }
                catch (ArgumentException)
                {
                    FileDependencies[dependentFile].Add(dependency);
                }
            }

            lock (_dependentFilesLock)
            {
                try
                {
                    DependentFiles.Add(dependency, new HashSet<string> { dependentFile });
                }
                catch(ArgumentException)
                {
                    DependentFiles[dependency].Add(dependentFile);
                }
            }
        }

        public void Remove(string name)
        {
            lock (_fileDependenciesLock)
            {
                HashSet<string> dependsOn;
                if (!FileDependencies.TryGetValue(name, out dependsOn)) return;

                lock (_dependentFilesLock)
                {
                    var dependentFiles = DependentFiles[dependsOn.First()];
                    dependentFiles.Remove(name);
                    if (!dependentFiles.Any()) DependentFiles.Remove(dependsOn.First());
                }

                FileDependencies.Remove(name);
            }
        }

        public string[] FindAllDependentFiles(string path)
        {
            var files = new List<string>();
            var stack = new Stack<string>();
            stack.Push(path);
            do
            {
                var next = stack.Pop();
                var sons = FindDependentFiles(next);
                files.AddRange(sons);
                foreach (var son in sons.Reverse()) stack.Push(son);
            } while (stack.Any());

            return files.ToArray();
        }
    }
}