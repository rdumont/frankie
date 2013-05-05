using System.Diagnostics;
using System.Reflection;
using DotLiquid;

namespace RDumont.Frankie.Core.Liquid
{
    public class FrankieDrop : Drop
    {
        private static readonly string AssemblyVersion;

        public string Name { get { return "Frankie"; } }
        public string Version { get { return AssemblyVersion; } }

        static FrankieDrop()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            AssemblyVersion = fileVersionInfo.FileVersion;
        }
    }
}
