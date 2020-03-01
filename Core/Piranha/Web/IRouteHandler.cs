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

using Piranha.Models;

namespace Piranha.Web
{
	/// <summary>
	/// Interface defining the methods that should be provided by the route handler. There currently
	/// exists two implementations of the IRouteHandler:
	/// 
	/// Piranha.WebPages.RouteHandler
	/// Piranha.Mvc.RouteHandler
	/// 
	/// </summary>
	public interface IRouteHandler
	{
		/// <summary>
		/// Handles requests to the current startpage
		/// </summary>
		/// <param name="context">The current http context</param>
		void HandleStartpage(HttpContext context);

		/// <summary>
		/// Handles requests to the given post.
		/// </summary>
		/// <param name="context">The current http context</param>
		/// <param name="permalink">The permalink</param>
		/// <param name="post">The post</param>
		/// <param name="args">Optional route arguments</param>
		void HandlePost(HttpContext context, Permalink permalink, Post post, params string[] args);

		/// <summary>
		/// Handles requests to the given page.
		/// </summary>
		/// <param name="context">The current http context</param>
		/// <param name="permalink">The permalink</param>
		/// <param name="page">The page</param>
		/// <param name="args">Optional route arguments</param>
		void HandlePage(HttpContext context, Permalink permalink, Page page, params string[] args);
	}
}
