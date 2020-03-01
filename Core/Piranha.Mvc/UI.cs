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
using System.Web;
using Piranha.Web;

namespace Piranha.Mvc
{
	/// <summary>
	/// This is the static mvc wrapper for Piranha.Web.UIHelper
	/// </summary>
	public static class UI
	{
		#region Members
		private static UIHelper Helper = new UIHelper();
		#endregion

		/// <summary>
		/// Generates the tags appropriate for the html head.
		/// </summary>
		/// <returns>The head information</returns>
		public static IHtmlString Head() { return Helper.Head(); }
        public static IHtmlString RssFrom(int c) { return Helper.RssFrom(c); }
        /// <summary>
		/// Sets the current page to the given value. This can be
        /// useful when using the UI helper in passive mode and the
        /// routing never sets the current page.
        /// </summary>
        /// <param name="page">The page</param>
        public static void SetCurrent(Models.Page page) { 
			HttpContext.Current.Items["Piranha_CurrentPage"] = page;
		}

        /// <summary>
        /// Sets the current post to the given value. This can be
        /// useful when using the UI helper in passive mode and the
        /// routing never sets the current post.
        /// </summary>
        /// <param name="post">The post</param>
        public static void SetCurrent(Models.Post post) { 
			HttpContext.Current.Items["Piranha_CurrentPost"] = post;
		}

		/// <summary>
		/// Generates a full site url from the virtual path.
		/// </summary>
		/// <param name="virtualpath">The virtual path.</param>
		/// <returns>The full site url</returns>
		public static IHtmlString SiteUrl(string virtualpath) { return Helper.SiteUrl(virtualpath); }

		/// <summary>
		/// Generates an absolute url from the virtual path or site url.
		/// </summary>
		/// <param name="url">The url</param>
		/// <returns>The absolute url</returns>
		public static IHtmlString AbsoluteUrl(string url) { return Helper.AbsoluteUrl(url); }

		/// <summary>
		/// Generates the url to the given permalink.
		/// </summary>
		/// <param name="permalink">The permalink</param>
		/// <param name="prefix">Optional culture prefix</param>
		/// <returns>The url</returns>
		public static IHtmlString Permalink(string permalink = "", string prefix = "") { return Helper.Permalink(permalink, prefix); }

		/// <summary>
		/// Generates the url to the given permalink.
		/// </summary>
		/// <param name="permalinkid">The id of the permalink</param>
		/// <param name="prefix">Optional culture prefix</param>
		/// <returns></returns>
		public static IHtmlString Permalink(Guid permalinkid, string prefix = "") { return Helper.Permalink(permalinkid, prefix); }

		/// <summary>
		/// Gets the URL to the content with the given id.
		/// </summary>
		/// <param name="id">The content id</param>
		/// <param name="width">Optional width</param>
		/// <param name="height">Optional height</param>
		/// <returns>The content url</returns>
		public static IHtmlString Content(Guid id, int width = 0, int height = 0) { return Helper.Content(id, width, height); }

		/// <summary>
		/// Gets the URL to the content with the given id.
		/// </summary>
		/// <param name="id">The content id</param>
		/// <param name="width">Optional width</param>
		/// <param name="height">Optional height</param>
		/// <returns>The content url</returns>
		public static IHtmlString Content(string id, int width = 0, int height = 0) { return Helper.Content(id, width, height); }

		/// <summary>
		/// Generates an image tag for the specified thumbnail.
		/// </summary>
		/// <param name="id">The content, page or post id.</param>
		/// <param name="size">Optional size</param>
		/// <returns>The image html string</returns>
		public static IHtmlString Thumbnail(Guid id, int size = 0) { return Helper.Thumbnail(id, size); }

		/// <summary>
		/// Generates an image tag for the specified thumbnail.
		/// </summary>
		/// <param name="id">The content id</param>
		/// <param name="size">Optional size</param>
		/// <returns>The image html string</returns>
		public static IHtmlString Thumbnail(string id, int size = 0) { return Helper.Thumbnail(id, size); }

		/// <summary>
		/// Gets the url to the uploaded content with the given id.
		/// </summary>
		/// <param name="id">The upload id</param>
		/// <param name="width">Optional width</param>
		/// <param name="height">Optional height</param>
		/// <returns>The url</returns>
		public static IHtmlString Upload(Guid id, int width = 0, int height = 0) { return Helper.Upload(id, width, height); }

		/// <summary>
		/// Gets the url to the uploaded content with the given id.
		/// </summary>
		/// <param name="id">The upload id</param>
		/// <param name="width">Optional width</param>
		/// <param name="height">Optional height</param>
		/// <returns>The url</returns>
		public static IHtmlString Upload(string id, int width = 0, int height = 0) { return Helper.Upload(id, width, height); }

		/// <summary>
		/// Return the site structure as an ul/li list with the current page selected.
		/// </summary>
		/// <param name="StartLevel">The start level of the menu</param>
		/// <param name="StopLevel">The stop level of the menu</param>
		/// <param name="Levels">The number of levels. Use this if you don't know the start level</param>
		/// <returns>A html string</returns>
		public static IHtmlString Menu(int StartLevel = 1, int StopLevel = Int32.MaxValue, int Levels = 0,
			string RootNode = "", string CssClass = "menu fr") {
			return Helper.Menu(StartLevel, StopLevel, Levels, RootNode, CssClass);
		}

		/// <summary>
		/// Renders the current breadcrumb.
		/// </summary>
		/// <param name="StartLevel">Optional start level</param>
		/// <param name="RootNode">Optional root node</param>
		/// <returns>The breadcrumb</returns>
		public static IHtmlString Breadcrumb(int StartLevel = 1, string RootNode = "") { return Helper.Breadcrumb(StartLevel, RootNode); }

		/// <summary>
		/// Gets an encrypted API-key valid for 30 minutes.
		/// </summary>
		/// <param name="apiKey">The API-key</param>
		/// <returns>The ecnrypted key</returns>
		public static IHtmlString APIKey(Guid apiKey) { return Helper.APIKey(apiKey); }

		/// <summary>
		/// Gets an ecrypted API-key valid for 30 minutes. If no API-key is provided
		/// the key for the currently logged in user is used.
		/// </summary>
		/// <param name="apiKey"></param>
		/// <returns></returns>
		public static IHtmlString APIKey(string apiKey = "") { return Helper.APIKey(apiKey); }

		/// <summary>
		/// Generates the correct controller name for the given permalink. The generated
		/// controller can be used in forms and actionlinks as the controller name. If the
		/// permalink is omitted, the permalink to the current page or post is used.
		/// </summary>
		/// <param name="permalink">The permalink</param>
		/// <param name="prefix">Optional culture prefix</param>
		/// <returns>The controller url</returns>
		public static string Controller(string permalink = "", string prefix = "") {
			return Permalink(permalink, prefix).ToHtmlString().Substring(1);
		}
	}
}
