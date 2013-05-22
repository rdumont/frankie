using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RDumont.Frankie.Core
{
    public class PluginManager
    {
        public List<IPluginDescriptor> Descriptors { get; private set; }
        public List<IHandlerPlugin> CustomHandlerPlugins { get; private set; }

        public void LoadPlugins()
        {
            Descriptors = new List<IPluginDescriptor>();
            CustomHandlerPlugins = new List<IHandlerPlugin>();

            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var dlls = Directory.EnumerateFiles(location, "Frankie.Plugins.*.dll");
            foreach (var dll in dlls)
            {
                var assembly = Assembly.LoadFile(dll);
                var descriptorType = assembly.GetTypes()
                    .SingleOrDefault(t => t.IsAssignableFrom(typeof (IPluginDescriptor)));
                
                if (descriptorType == null)
                    throw new InvalidOperationException("Could not find a plugin descriptor for " + assembly.FullName);

                var descriptor = (IPluginDescriptor) Activator.CreateInstance(descriptorType);
                Descriptors.Add(descriptor);

                if (descriptor is IHandlerPlugin)
                    CustomHandlerPlugins.Add(((IHandlerPlugin) descriptor));
            }
        }
    }
}
