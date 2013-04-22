﻿using System.IO;
using RDumont.Frankie.Specs.Testables;
using TechTalk.SpecFlow;

namespace RDumont.Frankie.Specs.Steps
{
    public class StepsBase
    {
        public static string BasePath
        {
            get { return (string) ScenarioContext.Current["base_path"]; }
            set { ScenarioContext.Current["base_path"] = value; }
        }

        public static TestLogger Logger
        {
            get { return ScenarioContext.Current.Get<TestLogger>(); }
            set { ScenarioContext.Current.Set(value); }
        }

        public void CreateDirectory(string path)
        {
            var fullPath = Path.Combine(BasePath, path);
            Directory.CreateDirectory(fullPath);
        }

        public void WriteFile(string path, string contents)
        {
            var fullPath = Path.Combine(BasePath, path);
            var directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            File.WriteAllText(fullPath, contents);
        }

        public string ReadFile(string path)
        {
            var fullPath = Path.Combine(BasePath, path);
            try
            {
                return File.ReadAllText(fullPath);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public string Trimmed(string contents)
        {
            return contents.Replace("\r\n", "\n").Trim('\r', '\n', ' ');
        }
    }
}