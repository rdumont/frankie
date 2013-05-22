using System;
using System.Collections.Generic;
using System.Linq;

namespace RDumont.Frankie.Core.Handlers
{
    public class AssetHandlerManager
    {
        private readonly SiteConfiguration _configuration;
        private readonly Io _io;

        public TemplateHandler TemplateHandler { get; private set; }
        public PostHandler PostHandler { get; private set; }
        public GeneratedContentHandler GeneratedContentHandler { get; private set; }
        public MarkdownPageHandler MarkdownPageHandler { get; private set; }
        public TransformableContentHandler TransformableContentHandler { get; private set; }
        public StaticContentHandler StaticContentHandler { get; private set; }

        private readonly List<IAssetHandler> _allHandlers;

        public AssetHandlerManager(SiteConfiguration configuration, Io io,
            IEnumerable<IHandlerPlugin> customHandlerPlugins)
        {
            _configuration = configuration;
            _io = io;
            TemplateHandler = new TemplateHandler(configuration, io, this);
            PostHandler = new PostHandler(configuration, io);
            GeneratedContentHandler = new GeneratedContentHandler(configuration);
            MarkdownPageHandler = new MarkdownPageHandler(configuration, io);
            TransformableContentHandler = new TransformableContentHandler(configuration, io);
            StaticContentHandler = new StaticContentHandler(configuration, io);

            _allHandlers = new List<IAssetHandler>
                {
                    TemplateHandler,
                    PostHandler,
                    GeneratedContentHandler,
                    MarkdownPageHandler,
                    TransformableContentHandler
                };

            var customHandlers = customHandlerPlugins.Select(plugin => plugin.CreateHandler(configuration, io));
            _allHandlers.AddRange(customHandlers);

            _allHandlers.Add(StaticContentHandler);
        }

        public IAssetHandler FindMatchingHandler(string path)
        {
            return _allHandlers.FirstOrDefault(handler => handler.Matches(path));
        }

        public void Handle(string path)
        {
            if (_configuration.IsExcluded(path))
                return;

            var handler = FindMatchingHandler(path);
            if (handler != null) handler.Handle(path);
        }

        public void HandleRemoval(string path)
        {
            if (_configuration.IsExcluded(path))
                return;

            var handler = FindMatchingHandler(path);
            if (handler != null) handler.HandleRemoval(path);
        }
    }
}