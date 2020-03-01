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
using System.Linq;
using System.Web;

using Piranha.Models;

namespace Piranha.Mvc
{
	/// <summary>
	/// Permalink route handler for ASP.NET MVC
	/// </summary>
	[Export(typeof(Web.IRouteHandler))]
	public class RouteHandler : Web.IRouteHandler
	{
		/// <summary>
		/// Handles requests to the current startpage
		/// </summary>
		/// <param name="context">The current http context</param>
		public void HandleStartpage(HttpContext context) {
			var page = Page.GetStartpage();

			if (!String.IsNullOrEmpty(page.Controller))
				context.RewritePath("~/" + page.Controller + "?permalink=" + page.Permalink + FormatQuerystring(context), false);
			else context.RewritePath("~/page?permalink=" + page.Permalink + FormatQuerystring(context), false);
		}

		/// <summary>
		/// Handles requests to the given post.
		/// </summary>
		/// <param name="context">The current http context</param>
		/// <param name="permalink">The permalink</param>
		/// <param name="post">The post</param>
		/// <param name="args">Optional route arguments</param>
		public void HandlePost(HttpContext context, Permalink permalink, Post post, params string[] args) {
			if (!String.IsNullOrEmpty(post.Controller)) {
				context.RewritePath("~/" + post.Controller + args.Implode("/") + "?permalink=" + permalink.Name +
					(post.IsDraft ? "&draft=true" : "") + FormatQuerystring(context), false);
			} else {
				context.RewritePath("~/post/" + args.Implode("/") + "?permalink=" + permalink.Name +
					(post.IsDraft ? "&draft=true" : "") + FormatQuerystring(context), false);
			}
		}

		/// <summary>
		/// Handles requests to the given page.
		/// </summary>
		/// <param name="context">The current http context</param>
		/// <param name="permalink">The permalink</param>
		/// <param name="page">The page</param>
		/// <param name="args">Optional route arguments</param>
		public void HandlePage(HttpContext context, Permalink permalink, Page page, params string[] args) {
			if (!String.IsNullOrEmpty(page.Controller)) {
				context.RewritePath("~/" + page.Controller + "/" + args.Implode("/") + "?permalink=" + permalink.Name +
					(page.IsDraft ? "&draft=true" : "") + FormatQuerystring(context), false);
			} else {
				context.RewritePath("~/page/" + args.Implode("/") + "?permalink=" + permalink.Name +
					(page.IsDraft ? "&draft=true" : "") + FormatQuerystring(context), false);
			}
		}

		/// <summary>
		/// Sets the optional culture param.
		/// </summary>
		/// <param name="draft">Whether this is a draft or not.</param>
		/// <returns>The request param</returns>
		private string FormatQuerystring(HttpContext context) {
			var query = "&piranha-culture=" + System.Globalization.CultureInfo.CurrentUICulture.Name;

			foreach (var param in context.Request.QueryString.AllKeys)
				query += "&" + param + "=" + HttpUtility.UrlEncode(context.Request.QueryString[param]);
			return query;
		}
	}
}