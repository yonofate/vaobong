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
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Piranha
{
	/// <summary>
	/// Provides access to web.config parameters.
	/// </summary>
	public static class Config
	{
		#region Members
		private static string[] namespaces = null;
		private static object nsmutex = new object();
		private static ConfigProvider mediaprovider = null;
		private static ConfigProvider mediaCacheProvider = null;
		private static ConfigProvider cacheProvider = null;
		private static ConfigProvider logProvider = null;
		private static object mpmutex = new object();
		private static object mcpmutex = new object();
		private static object cpmutex = new object();
		private static object lpmutex = new object();
		private static readonly ConfigFile config = GetConfig();
		private static bool? prefixlessPermalinks = true;
		#endregion

		/// <summary>
		/// Gets if method binding is disabled. When method binding is enabled, the first UrlData
		/// argument on a GET will be matched against the available methods in the executing page class.
		/// If a matching method is found this method will be executed instead of the usual ExecutePage.
		/// </summary>
		public static bool DisableMethodBinding {
			get {
				return config.Settings.DisableMethodBinding.Value;
			}
		}

		/// <summary>
		/// Gets if model state binding is disabled. When model state binding is enabled, values
		/// mapped by the ModelBinder is automatically stored into the model state of the current page.
		/// </summary>
		public static bool DisableModelStateBinding {
			get {
				return config.Settings.DisableModelStateBinding.Value;
			}
		}

		/// <summary>
		/// Gets if the manager interface should be disabled for the current instance.
		/// </summary>
		public static bool DisableManager {
			get {
				return config.Settings.DisableManager.Value;
			}
		}

		/// <summary>
		/// Gets an optional default schema for Entity Framework.
		/// </summary>
		public static string EntitySchema {
			get {
				return config.Settings.EntitySchema.Value;
			}
		}

		/// <summary>
		/// Gets/sets if the page & post type builder of the Extension Manager
		/// should be disabled.
		/// </summary>
		public static bool DisableTypeBuilder {
			get {
				return config.Settings.DisableTypeBuilder.Value;
			}
		}

		/// <summary>
		/// Gets if the providers should be searched outside of Piranha*.ddls
		/// </summary>
		public static bool DisableCatalogSearch {
			get {
				return config.Settings.DisableCatalogSearch.Value;
			}
		}

		/// <summary>
		/// Gets/sets if MEF composition should be disabled.
		/// </summary>
		public static bool DisableComposition {
			get {
				return config.Settings.DisableComposition.Value;
			}
		}

		/// <summary>
		/// Gets if "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge,chrome=1\"> should be rendered for IE
		/// </summary>
		public static bool RenderX_UA_CompatibleForIE {
			get {
				return config.Settings.RenderX_UA_CompatibleForIE.Value;
			}
		}

		/// <summary>
		/// Gets if database access errors should be shown instead of redirecting to /manager/install
		/// </summary>
		public static bool ShowDBErrors {
			get {
				return config.Settings.ShowDBErrors.Value;
			}
		}

		/// <summary>
		/// Gets the additional namespaces that should be inlcuded in the manager interface.
		/// </summary>
		public static string[] ManagerNamespaces {
			get {
				if (namespaces != null)
					return namespaces;

				lock (nsmutex) {
					if (namespaces == null) {
						var str = config.Settings.ManagerNamespaces.Value;

						if (!String.IsNullOrEmpty(str)) {
							var tmp = str.Split(new char[] { ',' });

							for (int n = 0; n < tmp.Length; n++)
								tmp[n] = tmp[n].Trim();
							namespaces = tmp;
						} else {
							namespaces = new string[0];
						}
					}
				}
				return namespaces;
			}
		}

		/// <summary>
		/// Gets if the application is running in passive mode or not.
		/// </summary>
		public static bool PassiveMode {
			get {
				return config.Settings.PassiveMode.Value;
			}
		}

		/// <summary>
		/// Gets/sets whether to use prefixless permalinks or not.
		/// </summary>
		public static bool PrefixlessPermalinks {
			get {
				if (!prefixlessPermalinks.HasValue)
					return config.Settings.PrefixlessPermalinks.Value;
				return prefixlessPermalinks.Value;
			}
			set {
				prefixlessPermalinks = value;
			}
		}

		/// <summary>
		/// Gets the configuration for the media provider to use. If the media provider is not
		/// specified the default LocalMediaProvider is used.
		/// </summary>
		internal static ConfigProvider MediaProvider {
			get {
				if (mediaprovider != null)
					return mediaprovider;

				lock (mpmutex) {
					if (mediaprovider == null) {
						var str = config.Providers.MediaProvider.Value;
						if (!String.IsNullOrEmpty(str)) {
							mediaprovider = GetProvider(str);
						} else {
							mediaprovider = new ConfigProvider() {
								TypeName = typeof(IO.LocalMediaProvider).FullName,
								AssemblyName = Assembly.GetExecutingAssembly().FullName
							};
						}
					}
				}
				return mediaprovider;
			}
		}

		/// <summary>
		/// Gets the configuration for the media cache provider to use. If the provider is not
		/// specified the default LocalMediaCacheProvider is used.
		/// </summary>
		internal static ConfigProvider MediaCacheProvider {
			get {
				if (mediaCacheProvider != null)
					return mediaCacheProvider;

				lock (mcpmutex) {
					if (mediaCacheProvider == null) {
						var str = config.Providers.MediaCacheProvider.Value;
						if (!String.IsNullOrEmpty(str)) {
							mediaCacheProvider = GetProvider(str);
						} else {
							mediaCacheProvider = new ConfigProvider() {
								TypeName = typeof(IO.LocalMediaCacheProvider).FullName,
								AssemblyName = Assembly.GetExecutingAssembly().FullName
							};
						}
					}
				}
				return mediaCacheProvider;
			}
		}

		/// <summary>
		/// Gets the configuration for the cache provider to use. If the cache provider is not
		/// specified the default WebCacheProvider is used.
		/// </summary>
		internal static ConfigProvider CacheProvider {
			get {
				if (cacheProvider != null)
					return cacheProvider;

				lock (cpmutex) {
					if (cacheProvider == null) {
						var str = config.Providers.CacheProvider.Value;
						if (!String.IsNullOrEmpty(str)) {
							cacheProvider = GetProvider(str);
						} else {
							cacheProvider = new ConfigProvider() {
								TypeName = typeof(Cache.WebCacheProvider).FullName,
								AssemblyName = Assembly.GetExecutingAssembly().FullName
							};
						}
					}
				}
				return cacheProvider;
			}
		}

		/// <summary>
		/// Gets the configuration for the log provider to use. If the log provider is not
		/// specified the default LocalLogProvider is used.
		/// </summary>
		internal static ConfigProvider LogProvider {
			get {
				if (logProvider != null)
					return logProvider;

				lock (lpmutex) {
					if (logProvider == null) {
						var str = config.Providers.LogProvider.Value;
						if (!String.IsNullOrEmpty(str)) {
							logProvider = GetProvider(str);
						} else {
							logProvider = new ConfigProvider() {
								TypeName = typeof(Log.LocalLogProvider).FullName,
								AssemblyName = Assembly.GetExecutingAssembly().FullName
							};
						}
					}
				}
				return logProvider;
			}
		}

		/// <summary>
		/// Gets the optional internal id of the current sitetree. If the parameter is empty
		/// the current host name is resolved. If the hostname isn't found DEFAULT_SITE is used.
		/// </summary>
		public static string SiteTree {
			get { return WebPages.WebPiranha.CurrentSite.InternalId; }
		}

		/// <summary>
		/// Gets the id of the currently active site tree.
		/// </summary>
		public static Guid SiteTreeId {
			get { return WebPages.WebPiranha.CurrentSite.Id; }
		}

		/// <summary>
		/// Gets the id of the namespace for the currently active site tree.
		/// </summary>
		public static Guid SiteTreeNamespaceId {
			get { return WebPages.WebPiranha.CurrentSite.NamespaceId; }
		}


        /// <summary>
        /// Gets  whether to match only exact permalinks in routing
        /// </summary>
	    public static bool ExactPermalinkMatching
	    {
            get { return config.Settings.ExactPermalinkMatching.Value; }
	    }

		/// <summary>
		/// Gets the id of the default sys user.
		/// </summary>
		public static readonly Guid SysUserId = new Guid("CA19D4E7-92F0-42F6-926A-68413BBDAFBC");

		/// <summary>
		/// Gets the id of the default sys admin group.
		/// </summary>
		public static readonly Guid SysAdminGroupId = new Guid("7C536B66-D292-4369-8F37-948B32229B83");

		/// <summary>
		/// Gets the id of the default namespace.
		/// </summary>
		public static readonly Guid DefaultNamespaceId = new Guid("8FF4A4B4-9B6C-4176-AAA2-DB031D75AC03");

		/// <summary>
		/// Gets the id of the archive namespace.
		/// </summary>
		public static readonly Guid ArchiveNamespaceId = new Guid("C8342FB4-D38E-4EAF-BBC1-4EF3BDD7500C");

		/// <summary>
		/// Gets the id of the media namespace.
		/// </summary>
		public static readonly Guid MediaNamespaceId = new Guid("368249B1-7F9C-4974-B9E3-A55D068DD9B6");

		/// <summary>
		/// Gets the id of the default site tree.
		/// </summary>
		public static readonly Guid DefaultSiteTreeId = new Guid("c2f87b2b-f585-4696-8a2b-3c9df882701e");

		#region Private methods
		/// <summary>
		/// Gets the configuration section from the Web.config.
		/// </summary>
		/// <returns></returns>
		private static ConfigFile GetConfig() {
			var section = (ConfigFile)ConfigurationManager.GetSection("piranha");

			if (section == null)
				section = new ConfigFile();
			return section;
		}

		/// <summary>
		/// Gets a provider object from the given string
		/// </summary>
		/// <param name="str">The provider string</param>
		/// <returns>The parsed provider</returns>
		private static ConfigProvider GetProvider(string str) {
			var vals = str.Split(new char[] { ',' });

			return new ConfigProvider() {
				TypeName = vals[0].Trim(),
				AssemblyName = vals[1].Trim()
			};
		}
		#endregion
	}
}