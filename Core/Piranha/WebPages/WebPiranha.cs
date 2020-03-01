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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

using Piranha.Models;
using Piranha.Web.Handlers;
using System.Threading;

namespace Piranha.WebPages
{
	public static class WebPiranha
	{
		#region Inner classes
		/// <summary>
		/// Class for handling hostnames with url extensions.
		/// </summary>
		public sealed class ExtendedHostName
		{
			/// <summary>
			/// Gets/sets the host name.
			/// </summary>
			public string HostName { get; set; }

			/// <summary>
			/// Gets/sets the host name.
			/// </summary>
			public string Extension { get; set; }

			/// <summary>
			/// Gets/sets the targeted site tree.
			/// </summary>
			public Entities.SiteTree SiteTree { get; set; }
		}
		#endregion

		#region Members
		/// <summary>
		/// Internal member for the host names.
		/// </summary>
		private static Dictionary<string, Entities.SiteTree> hostNames = null;

		/// <summary>
		/// The registered cultures.
		/// </summary>
		internal static Dictionary<string, CultureInfo> Cultures = new Dictionary<string, CultureInfo>();

		/// <summary>
		/// The registered cultures prefixes.
		/// </summary>
		internal static Dictionary<string, string> CulturePrefixes = new Dictionary<string, string>();

		/// <summary>
		/// The registered hostnames.
		/// </summary>
		internal static Dictionary<string, Entities.SiteTree> HostNames {
			get {
				if (hostNames == null) {
					hostNames = new Dictionary<string, Entities.SiteTree>();
					RegisterDefaultHostNames();
				}
				return hostNames;
			}
		}

		/// <summary>
		/// The default site tree.
		/// </summary>
		internal static Entities.SiteTree DefaultSite = null;

		/// <summary>
		/// The current site tree.
		/// </summary>
		public static Entities.SiteTree CurrentSite {
			get {
				// Check for configured site tree from the host name
				if (HttpContext.Current != null && HttpContext.Current.Request != null) {
					if (RequestedSite != null) {
						return RequestedSite.SiteTree;
					} else {
						var hostname = HttpContext.Current.Request.Url.Host.ToLower();
						if (HostNames.ContainsKey(hostname))
							return HostNames[hostname];
					}
				}
				// Nothing found, return default
				return DefaultSite;
			}
		}

		/// <summary>
		/// Gets the URL extension for the currently requested site.
		/// </summary>
		internal static string CurrentSiteExtension {
			get {
				if (RequestedSite != null)
					return RequestedSite.Extension.ToLower() + "/" ;
				return "" ;
			}
		}

		/// <summary>
		/// Gets the currently requested site.
		/// </summary>
		private static ExtendedHostName RequestedSite {
			get {
				if (HttpContext.Current.Items.Contains("RequestedSite"))
					return (ExtendedHostName)HttpContext.Current.Items["RequestedSite"];
				return null;
			}
			set {
				HttpContext.Current.Items["RequestedSite"] = value;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the current application root.
		/// </summary>
		public static string ApplicationPath {
			get {
				string root = HttpContext.Current.Request.ApplicationPath;
				if (!root.EndsWith("/"))
					root += "/";
				return root;
			}
		}
		#endregion

		/// <summary>
		/// Registers the culture to the given prefix.
		/// </summary>
		/// <param name="urlprefix">The url prefix.</param>
		/// <param name="culture">The culture</param>
		public static void RegisterCulture(string urlprefix, CultureInfo culture) {
			Cultures.Add(urlprefix, culture);
			CulturePrefixes.Add(culture.Name, urlprefix);
		}

		/// <summary>
		/// Get's the current culture prefix.
		/// </summary>
		/// <returns>The culture prefix</returns>
		public static string GetCulturePrefix() {
			return (CulturePrefixes.ContainsKey(CultureInfo.CurrentUICulture.Name) ? CulturePrefixes[CultureInfo.CurrentUICulture.Name] + "/" : "");
		}

		/// <summary>
		/// Gets the public site url.
		/// </summary>
		/// <returns>The url</returns>
		public static string GetSiteUrl() {
			var context = HttpContext.Current;
		    var protocol = context.Request.IsSecureConnection ? "https://" : "http://";
			var url = protocol + context.Request.Url.DnsSafeHost +
				(!context.Request.Url.IsDefaultPort ? ":" + context.Request.Url.Port : "") +
				context.Request.ApplicationPath;

			if (url.EndsWith("/"))
				return url.Substring(0, url.Length - 1);
			return url;
		}

		/// <summary>
		/// Registers all of the hostnames configured in the database.
		/// </summary>
		public static void RegisterDefaultHostNames() {
			// Make sure we don't try to register the host names before
			// the database has been installed.
			if (SysParam.GetByName("SITE_VERSION") != null) {
				if (hostNames == null)
					hostNames = new Dictionary<string, Entities.SiteTree>();

				HostNames.Clear();

				// We need to check version so we don't try to access the column sitetree_hostnames
				// before it's been created in the database.
				if (Data.Database.InstalledVersion > 26) {
					using (var db = new DataContext()) {
						var sites = db.SiteTrees.ToList();

						foreach (var site in sites) {
							if (HttpContext.Current != null)
								Page.InvalidateStartpage(site.Id);

							if (!String.IsNullOrEmpty(site.HostNames)) {
								var hostnames = site.HostNames.Split(new char[] { ',' });

								foreach (var host in hostnames) {
									if (HostNames.ContainsKey(host))
										throw new Exception("Duplicates of the hostname [" + host + "] was found configured");
									HostNames.Add(host.ToLower(), site);
								}
							}
							if (site.Id == Config.DefaultSiteTreeId)
								DefaultSite = site;
						}
					}
				}
			}
		}

		/// <summary>
		/// Initializes the webb app.
		/// </summary>
		public static void Init() {
			// Register the basic account route
			if (!Config.PassiveMode) {
				try {
					RouteTable.Routes.MapRoute("Account", "account/{action}", new { controller = "auth", action = "index" }, new string[] { "Piranha.Web" });
				} catch { }
			}

			// Register hostnames
			RegisterDefaultHostNames();

			// Reset template cache
			Web.TemplateCache.Clear();

			// Register json deserialization for post data
			ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());
		}

		/// <summary>
		/// Handles the URL Rewriting for the application
		/// </summary>
		/// <param name="context">Http context</param>
		public static void BeginRequest(HttpContext context) {
			try {
				string path = context.Request.Path.Substring(context.Request.ApplicationPath.Length > 1 ?
					context.Request.ApplicationPath.Length : 0);

				string[] args = path.Split(new char[] { '/' }).Subset(1);

				if (args.Length > 0) {
					int pos = 0;

					// Ensure database
					if (args[0] == "" && SysParam.GetByName("SITE_VERSION") == null) {
						context.Response.Redirect("~/manager", false);
						context.Response.EndClean();
					}

					// Check for culture prefix
					if (Cultures.ContainsKey(args[0])) {
						System.Threading.Thread.CurrentThread.CurrentCulture =
							System.Threading.Thread.CurrentThread.CurrentUICulture = Cultures[args[0]];
						pos = 1;
					} else {
						var def = (GlobalizationSection)WebConfigurationManager.GetSection("system.web/globalization");
						if (def != null && !String.IsNullOrWhiteSpace(def.Culture) && !def.Culture.StartsWith("auto:")) {
							System.Threading.Thread.CurrentThread.CurrentCulture =
								System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(def.UICulture);
						}
					}

					// Check for hostname extension. This feature can't be combined with culture prefixes
					if (pos == 0) {
						var segments = context.Request.RawUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        if (segments.Length > 0) {
						    var hostExt = context.Request.Url.Host + "/" + segments[0];

						    if (HostNames.ContainsKey(hostExt)) {
							    RequestedSite = new ExtendedHostName() {
								    HostName = context.Request.Url.Host,
                                    Extension = segments[0],
								    SiteTree = HostNames[hostExt]
							    };
                                if (segments[0] == args[0]) {
                                    pos = 1;

                                    // If this was the last argument, add an empty one
                                    if (args.Length == 1) {
                                        args = args.Concat(new string[] { "" }).ToArray();
                                    }
                                }
						    }
                        }
					}

					var handled = false;

					// Find the correct request handler
					foreach (var hr in Application.Current.Handlers) {
						if (hr.UrlPrefix.ToLower() == args[pos].ToLower()) {
							if (hr.Id != "PERMALINK" || !Config.PrefixlessPermalinks) {
								// Don't execute permalink routing in passive mode
								if ((hr.Id != "PERMALINK" && hr.Id != "STARTPAGE") || !Config.PassiveMode) {
									// Execute the handler
									hr.Handler.HandleRequest(context, args.Subset(pos + 1));
									handled = true;
									break;
								}
							}
						}
					}

					if (!handled && args[pos].ToLower() == "res.ashx") {
						Application.Current.Resources.HandleRequest(context, args.Subset(pos + 1));
						handled = true;
					}

					// If no handler was found and we are using prefixless permalinks, 
					// route traffic to the permalink handler.
					if (!Config.PassiveMode) {
						if (!handled && Config.PrefixlessPermalinks && args[pos].ToLower() != "manager" && String.IsNullOrEmpty(context.Request["permalink"])) {
							var handler = Application.Current.Handlers["PERMALINK"];
							handler.HandleRequest(context, args.Subset(pos));
						}
					}
				}
			} catch (ThreadAbortException) {
				// We simply swallow this exception as we don't want unhandled
				// exceptions flying around causing the app pool to die.
			} catch (Exception e) {
				// One catch to rule them all, and in the log file bind them.
				Application.Current.LogProvider.Error("WebPiranha.BeginRequest", "Unhandled exception", e);
				context.Response.StatusCode = 500;
				context.Response.EndClean();
			}
		}

		/// <summary>
		/// Handles current UI culture.
		/// </summary>
		/// <param name="context">The http context</param>
		public static void HandleCulture(HttpContext context) {
			//
			// NOTE: This code will fail completely in the manager view as accessing the request 
			// collection triggers the form data validation.
			//
			try {
				if (context.Request.HttpMethod.ToUpper() == "POST") {
					if (!String.IsNullOrEmpty(context.Request["lang"]))
						context.Session["lang"] = context.Request["lang"];
				}
				if (context.Session != null && context.Session["lang"] != null)
					System.Threading.Thread.CurrentThread.CurrentUICulture =
						new System.Globalization.CultureInfo((string)context.Session["lang"]);
			} catch { }
		}
	}
}
