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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;

using Piranha.Data;
using Piranha.Models;

namespace Piranha.Web.Handlers
{
	public class RssHandler : IRequestHandler
	{
		/// <summary>
		/// Handles the current request.
		/// </summary>
		/// <param name="context">The current context</param>
		/// <param name="args">Optional url arguments passed to the handler</param>
		public virtual void HandleRequest(System.Web.HttpContext context, params string[] args) {
			var top = Convert.ToInt32(SysParam.GetByName("RSS_NUM_POSTS").Value);
			var posts = Post.Get("post_draft = 0 AND post_rss = 1 AND post_template_id IN (SELECT posttemplate_id FROM posttemplate WHERE posttemplate_rss = 1)", new Params() { OrderBy = "post_published DESC", Top = top });

			Web.RssHelper.Generate(context,
				WebPages.WebPiranha.CurrentSite.MetaTitle,
				WebPages.WebPiranha.CurrentSite.MetaDescription,
				posts);
		}
	}
}
