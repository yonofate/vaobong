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
using System.Linq;
using System.Text;
using System.Web;

using Piranha.Models;

namespace Piranha.Web.Handlers
{
	/// <summary>
	/// Request handler for permalinks.
	/// </summary>
	public class PermalinkHandler : IRequestHandler
	{
		/// <summary>
		/// Handles the current request.
		/// </summary>
		/// <param name="context">The current context</param>
		/// <param name="args">Optional url arguments passed to the handler</param>
		public virtual void HandleRequest(HttpContext context, params string[] args) {
			HandleRequest(context, false, args);
		}

		/// <summary>
		/// Handles the current request.
		/// </summary>
		/// <param name="context">The current context</param>
		/// <param name="draft">Whether to view the draft</param>
		/// <param name="args">Optional url arguments passed to the handler</param>
		protected virtual void HandleRequest(HttpContext context, bool draft, params string[] args) {
			if (Application.Current.RouteHandler != null) {
				if (args != null && args.Length > 0) {
					Permalink perm = null;
					int segments = 0;

                   
                    // Accept permalinks with '/' in them
                    for (int n = 0; n < args.Length; n++)
                    {
                        // Check if we can find a permalink in the current namespace
                        perm = Permalink.GetByName(Config.SiteTreeNamespaceId,
                            String.Join("/", args.Subset(0, args.Length - n).Where(x => !string.IsNullOrEmpty(x))));
                        segments = args.Length - n;
						// Check if we're only matching exact permalink
                        if (Config.ExactPermalinkMatching && perm == null)
                            break;
                        if (perm != null)
                            break;
                    } 
                    
					

					// If we didn't find a permalink, check for posts in the default namespace
					if (perm == null && Config.SiteTreeNamespaceId != Config.DefaultNamespaceId) {
						segments = 0;
						// Accept permalinks with '/' in them
						for (int n = 0; n < args.Length; n++) {
							Permalink post = Permalink.GetByName(Config.DefaultNamespaceId, args.Subset(0, args.Length - n).Implode("/"));
							segments = args.Length - n;
							// Check if we're only matching exact permalink
							if (Config.ExactPermalinkMatching && post == null)
								break;
							if (post != null && post.Type == Permalink.PermalinkType.POST) {
								perm = post;
								break;
							}
						}
					}

					if (perm != null) {
						if (perm.Type == Permalink.PermalinkType.PAGE) {
							Page page = Page.GetByPermalinkId(perm.Id, draft);

							if (page != null) {
								if (!String.IsNullOrEmpty(page.Redirect)) {
									if (page.Redirect.StartsWith("http://"))
										context.Response.Redirect(page.Redirect, false);
									else context.Response.Redirect(page.Redirect, false);
									context.Response.EndClean();
								} else {
									//
									// Call the route handler to route the current page.
									Application.Current.RouteHandler.HandlePage(context, perm, page, args.Subset(segments));
								}
							} else {
								if (!Config.PrefixlessPermalinks)
									context.Response.StatusCode = 404;
							}
						} else if (perm.Type == Permalink.PermalinkType.POST) {
							Post post = Post.GetByPermalinkId(perm.Id, draft);

							if (post != null) {
								//
								// Call the route handler to route the current post.
								Application.Current.RouteHandler.HandlePost(context, perm, post, args.Subset(segments));
							} else {
								if (!Config.PrefixlessPermalinks)
									context.Response.StatusCode = 404;
							}
						}
					} else {
						if (!Config.PrefixlessPermalinks)
							context.Response.StatusCode = 404;
					}
				} else {
					//
					// Call the route handler to route to the startpage.
					Application.Current.RouteHandler.HandleStartpage(context);
				}
			}
		}

		/// <summary>
		/// Gets the current culture param.
		/// </summary>
		/// <param name="draft">Whether this is a draft or not.</param>
		/// <returns>The request param</returns>
		private string GetCultureParam(HttpContext context, bool draft) {
			var query = "";

			foreach (var param in context.Request.QueryString.AllKeys) {
				query += "&" + param + "=" + context.Request.QueryString[param];
			}
			return (draft ? "&" : "?") + "piranha-culture=" + System.Globalization.CultureInfo.CurrentUICulture.Name + query;
		}
	}
}
