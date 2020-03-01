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

using Piranha.Models;

namespace Piranha.Mvc
{
	/// <summary>
	/// The single post controller is the base controller from which all controllers
	/// representing a post should derive from.
	/// </summary>
	public class SinglePostController : BaseController
	{
		/// <summary>
		/// Gets the current post model.
		/// </summary>
		/// <returns>The model</returns>
		protected PostModel GetModel() {
			return GetModel<PostModel>(CurrentPermalink);
		}

		/// <summary>
		/// Gets the current model.
		/// </summary>
		/// <typeparam name="T">The model type</typeparam>
		/// <returns>The model</returns>
		protected T GetModel<T>() where T : PostModel {
			return GetModel<T>(CurrentPermalink);
		}

		/// <summary>
		/// Gets the post model identified by the given permalink.
		/// </summary>
		/// <param name="permalink">The permalink</param>
		/// <returns>The model</returns>
		protected PostModel GetModel(string permalink) {
			return GetModel<PostModel>(permalink);
		}

		/// <summary>
		/// Gets the model identified by the given permalink.
		/// </summary>
		/// <typeparam name="T">The model type</typeparam>
		/// <param name="permalink">The permalink</param>
		/// <returns>The model</returns>
		protected T GetModel<T>(string permalink) where T : PostModel {
			// Get the model
			var post = Post.GetByPermalink(permalink, IsDraft);
			var model = PostModel.Get<T>(post);

			HttpContext.Items["Piranha_CurrentPage"] = null;
			HttpContext.Items["Piranha_CurrentPost"] = model.Post;

			// Execute hook, if it exists
			if (WebPages.Hooks.Model.PostModelLoaded != null)
				WebPages.Hooks.Model.PostModelLoaded(model);

			return model;
		}

		/// <summary>
		/// Check if the current user has access to the action before continuing.
		/// </summary>
		/// <param name="context">The current context</param>
		protected override void OnActionExecuted(System.Web.Mvc.ActionExecutedContext filterContext) {
			// Perform base class stuff
			base.OnActionExecuted(filterContext);

			var post = Post.GetByPermalink(CurrentPermalink, IsDraft);
			if (post != null && !IsDraft) {
				DateTime mod = post.LastModified;
				Web.ClientCache.HandleClientCache(HttpContext.ApplicationInstance.Context,
					WebPages.WebPiranha.GetCulturePrefix() + post.Id.ToString(), mod);
			}
		}
	}
}
