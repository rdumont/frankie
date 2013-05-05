﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using RDumont.Frankie.Core;
using RDumont.Frankie.Core.Handlers;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class RunCommand : Command<BaseOptions>
    {
        private readonly Generator generator;

        private readonly string[] exclude = new[]
            {
                Post.POSTS_FOLDER,
                "_site",
                "_templates",
                ".gitignore",
                ".hgignore",
                "config.yaml"
            };

        public override string Name
        {
            get { return "run"; }
        }

        public RunCommand() : this(new Generator(SiteContext.Current))
        {
        }

        public RunCommand(Generator generator)
        {
            this.generator = generator;
        }

        public override void ExecuteCommand(BaseOptions options)
        {
            Logger.Current.Log(LoggingLevel.Minimal, "Running Frankie...");
            Logger.Current.Log(LoggingLevel.Minimal, "Source: {0}", options.LocationPath);
            Logger.Current.Log(LoggingLevel.Minimal, "Target: {0}\n", options.OutputPath);

            var sw = System.Diagnostics.Stopwatch.StartNew();
            var root = options.LocationPath;
            var output = options.OutputPath;

            var configuration = LoadConfiguration(options);
            var io = new Io();
            var handlers = new AssetHandlerManager(configuration, io);
            this.generator.Init(handlers, configuration, io);

            this.CleanDirectory(output);

            handlers.TemplateHandler.CompileAllTemplates();

            var postFiles = FindPostPaths(root).Select(configuration.GetRelativePath);
            handlers.PostHandler.LoadAllPosts(postFiles, SiteContext.Current);
            handlers.PostHandler.WriteAllPosts();

            var allEntries = FindAllEntries(root).Select(configuration.GetRelativePath);
            foreach (var file in allEntries)
                handlers.Handle(file);
            
            sw.Stop();

            Logger.Current.Log(LoggingLevel.Minimal, "\nFINISHED! Took {0}ms", sw.ElapsedMilliseconds);
        }

        public static SiteConfiguration LoadConfiguration(BaseOptions options)
        {
            var configurationFilePath = Path.Combine(options.LocationPath, "config.yaml");
            var configuration = SiteConfiguration.Load(configurationFilePath);
            configuration.SourcePath = options.LocationPath.TrimEnd(Path.DirectorySeparatorChar);
            configuration.SitePath = options.OutputPath.TrimEnd(Path.DirectorySeparatorChar);

            if (configuration.Culture != null)
            {
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture
                    = CultureInfo.GetCultureInfo(configuration.Culture);
            }

            return configuration;
        }

        private void CleanDirectory(string path)
        {
            var folders = Directory.GetDirectories(path);
            foreach (var folder in folders)
                Directory.Delete(folder, true);

            var files = Directory.GetFiles(path).Where(f => !f.EndsWith(".gitignore"));
            foreach (var file in files)
                File.Delete(file);
        }

        private IEnumerable<string> FindPostPaths(string root)
        {
            var folder = Path.Combine(root, Post.POSTS_FOLDER);
            return Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories);
        }

        private IEnumerable<string> FindAllEntries(string root)
        {
            var excludedEndings = this.exclude.Select(e => Path.DirectorySeparatorChar + e).ToArray();

            var topLevelEntries = Directory.EnumerateFileSystemEntries(root)
                .Where(entry => !excludedEndings.Any(entry.EndsWith));

            var allEntries = new List<String>();
            foreach (var entry in topLevelEntries)
            {
                if (Directory.Exists(entry))
                    allEntries.AddRange(FindEntries(entry));
                else
                    allEntries.Add(entry);
            }
            return allEntries;
        }

        private static IEnumerable<string> FindEntries(string root)
        {
            var result = new List<string>();
            result.AddRange(Directory.GetFiles(root));

            foreach (var folder in Directory.GetDirectories(root))
                result.AddRange(FindEntries(folder));

            return result;
        }
    }
}
