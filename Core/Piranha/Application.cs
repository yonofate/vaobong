/*
 * Copyright (c) 2011-2015 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Web;

using AutoMapper;
using Piranha.Web;
using Piranha.Web.Handlers;

namespace Piranha
{
	/// <summary>
	/// The application class contains the main state and configuration
	/// for the current running web application.
	/// </summary>
	public sealed class Application
	{
		#region Inner classes
		public sealed class AppConfig
		{
			public IRouteHandler RouteHandler { get; set; }
			public IClientFramework Framework { get; set; }
		}
		#endregion

		#region Members
		/// <summary>
		/// Static singleton instance of the application.
		/// </summary>
		public static readonly Application Current = new Application();

		/// <summary>
		/// The collection of registered request handlers.
		/// </summary>
		public readonly RequestHandlerCollection Handlers = new RequestHandlerCollection();

		/// <summary>
		/// The currently active media provider.
		/// </summary>
		public IO.IMediaProvider MediaProvider { get; private set; }

		/// <summary>
		/// The currently active media cache provider.
		/// </summary>
		public IO.IMediaCacheProvider MediaCacheProvider { get; private set; }

		/// <summary>
		/// The currently active cache provider.
		/// </summary>
		public Cache.ICacheProvider CacheProvider { get; private set; }

		/// <summary>
		/// The currently active log provider.
		/// </summary>
		public Log.ILogProvider LogProvider { get; private set; }

		/// <summary>
		/// The currently active user provider.
		/// </summary>
		internal readonly Security.IUserProvider UserProvider = new Security.LocalUserProvider();

		/// <summary>
		/// The manager resource handler.
		/// </summary>
		internal readonly ResourceHandler Resources = new ResourceHandler();
		#endregion

		#region Properties
		/// <summary>
		/// Gets the current route handler.
		/// </summary>
		[Import(typeof(IRouteHandler), AllowDefault = true)]
		public IRouteHandler RouteHandler { get; private set; }

		/// <summary>
		/// Gets the current client framework.
		/// </summary>
		[Import(typeof(IClientFramework), AllowDefault = true)]
		public IClientFramework ClientFramework { get; private set; }

		/// <summary>
		/// Gets if the currently installed client framework is intended for WebPages.
		/// </summary>
		public bool IsWebPages {
			get { return ClientFramework != null && ClientFramework.Type == FrameworkType.WebPages; }
		}

		/// <summary>
		/// Gets if the currently installed client framework is intended for MVC.
		/// </summary>
		public bool IsMvc {
			get { return ClientFramework != null && ClientFramework.Type == FrameworkType.Mvc; }
		}
		#endregion

		/// <summary>
		/// Default private constructor.
		/// </summary>
		private Application() {
			Initialize();
		}

		/// <summary>
		/// Compose the imports manually if MEF is disabled.
		/// </summary>
		/// <param name="configure">Configuration action</param>
		public static void Compose(Action<AppConfig> configure) {
			var config = new AppConfig();

			// Configure the config
			if (configure != null)
				configure(config);

			// Manually compose the application
			Current.RouteHandler = config.RouteHandler;
			Current.ClientFramework = config.Framework;
		}

		/// <summary>
		/// Registers the default handlers.
		/// </summary>
		private void RegisterHandlers() {
			Handlers.Add("", "STARTPAGE", new PermalinkHandler());
			Handlers.Add("home", "PERMALINK", new PermalinkHandler());
			Handlers.Add("draft", "DRAFT", new DraftHandler());
			Handlers.Add("media", "CONTENT", new ContentHandler());
			Handlers.Add("media.ashx", "CONTENTHANDLER", new ContentHandler());
			Handlers.Add("mediadraft", "CONTENTDRAFT", new DraftContentHandler());
			Handlers.Add("mediadraft.ashx", "CONTENTDRAFTHANDLER", new DraftContentHandler());
			Handlers.Add("thumb", "THUMBNAIL", new ThumbnailHandler());
			Handlers.Add("thumbdraft", "THUMBNAILDRAFT", new DraftThumbnailHandler());
			Handlers.Add("upload", "UPLOAD", new UploadHandler());
			Handlers.Add("archive", "ARCHIVE", new ArchiveHandler());
			Handlers.Add("rss", "RSS", new RssHandler());
			Handlers.Add("sitemap.xml", "SITEMAP", new SitemapHandler());
		}

		/// <summary>
		/// Initializes the application instance.
		/// </summary>
		private void Initialize() {
			if (!Config.DisableComposition) {
				var catalog = new AggregateCatalog();

				// Compose parts
				catalog.Catalogs.Add(Config.DisableCatalogSearch ? new DirectoryCatalog("Bin", "Piranha*.dll") : new DirectoryCatalog("Bin"));
				var container = new CompositionContainer(catalog);
				container.ComposeParts(this);
			}

			// Get the current media provider
			var assembly = Assembly.Load(Config.MediaProvider.AssemblyName);
			if (assembly != null) {
				var type = assembly.GetType(Config.MediaProvider.TypeName);
				if (type != null)
					MediaProvider = (IO.IMediaProvider)Activator.CreateInstance(type);
				else throw new TypeAccessException("MediaProvider " + Config.MediaProvider.TypeName + " was not found");
			}

			// Get the current media cache provider
			assembly = Assembly.Load(Config.MediaCacheProvider.AssemblyName);
			if (assembly != null) {
				var type = assembly.GetType(Config.MediaCacheProvider.TypeName);
				if (type != null)
					MediaCacheProvider = (IO.IMediaCacheProvider)Activator.CreateInstance(type);
				else throw new TypeAccessException("MediaCacheProvider " + Config.MediaCacheProvider.TypeName + " was not found");
			}

			// Get the current cache provider
			assembly = Assembly.Load(Config.CacheProvider.AssemblyName);
			if (assembly != null) {
				var type = assembly.GetType(Config.CacheProvider.TypeName);
				if (type != null)
					CacheProvider = (Cache.ICacheProvider)Activator.CreateInstance(type);
				else throw new TypeAccessException("CacheProvider " + Config.CacheProvider.TypeName + " was not found");
			}

			// Get the current log provider
            assembly = Assembly.Load(Config.LogProvider.AssemblyName);
            if (assembly != null)
            {
                var type = assembly.GetType(Config.LogProvider.TypeName);
                if (type != null)
                    LogProvider = (Log.ILogProvider)Activator.CreateInstance(type);
                else throw new TypeAccessException("LogProvider " + Config.LogProvider.TypeName + " was not found");
            }

			RegisterHandlers();

			// Configure AutoMapper
			Mapper.CreateMap<Models.Sitemap, Models.Sitemap>()
				.ForMember(s => s.Pages, o => o.Ignore());

			// Assert configuration
			Mapper.AssertConfigurationIsValid();
		}
	}
}