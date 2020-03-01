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
using Piranha.Web;

namespace Piranha.WebPages
{
	/// <summary>
	/// Standard page class for a single page.
	/// </summary>
	public abstract class SinglePage : SinglePage<PageModel> { }

	/// <summary>
	/// Page class for a single page where the model is of the generic type T.
	/// </summary>
	/// <typeparam name="T">The model type</typeparam>
	public abstract class SinglePage<T> : ContentPage<T> where T : PageModel
	{
		#region Members
		private Models.Page page = null;
		#endregion

		/// <summary>
		/// Initializes the web page
		/// </summary>
		protected override void InitializePage() {
			string permalink = Request["permalink"];
			bool draft = false;
			bool cached = false;

			// Check if we want to see the draft
			if (User.HasAccess("ADMIN_PAGE")) {
				if (!String.IsNullOrEmpty(Request["draft"])) {
					try {
						draft = Convert.ToBoolean(Request["draft"]);
					} catch { }
				}
			}

			// Load the current page
			if (!String.IsNullOrEmpty(permalink))
				page = Models.Page.GetByPermalink(permalink, draft);
			else page = Models.Page.GetStartpage(draft);

			// Check permissions
			if (page.GroupId != Guid.Empty) {
				if (!User.IsMember(page.GroupId)) {
					SysParam param = SysParam.GetByName("LOGIN_PAGE");
					if (param != null)
						Response.Redirect(param.Value, false);
					else Response.Redirect("~/", false);
					HttpContext.Current.Response.EndClean();
				}
				Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
			} else if (!draft) {
				// Only cache public non drafts
				DateTime mod = GetLastModified(page);
				DateTime tmod = TemplateCache.GetLastModified(!String.IsNullOrEmpty(page.Controller) ?
					page.Controller : "~/page.cshtml");
				mod = tmod > mod ? tmod : mod;
				cached = ClientCache.HandleClientCache(HttpContext.Current, WebPiranha.GetCulturePrefix() + page.Id.ToString(), mod);
			}
			// Check for disabled groups
			if (page.DisabledGroups.Contains(User.GetProfile().GroupId)) {
				SysParam param = SysParam.GetByName("LOGIN_PAGE");
				if (param != null)
					Response.Redirect(param.Value, false);
				else Response.Redirect("~/", false);
				HttpContext.Current.Response.EndClean();
			}
			// Load the model if the page wasn't cached
			if (!cached)
				InitModel(PageModel.Get<T>(page));

			// Execute hook, if it exists
			if (Hooks.Model.PageModelLoaded != null)
				Hooks.Model.PageModelLoaded(Model);

			base.InitializePage();
		}

		/// <summary>
		/// Gets the lastest modification date for caching.
		/// </summary>
		/// <returns></returns>
		protected virtual DateTime GetLastModified(Models.Page page) {
			return page.LastModified;
		}

		#region Private methods
		/// <summary>
		/// Initializes the instance from the given model.
		/// </summary>
		/// <param name="pm">The page model</param>
		protected virtual void InitModel(T pm) {
			Model = pm;

			Page.Current = HttpContext.Current.Items["Piranha_CurrentPage"] = Model.Page;
			Page.CurrentPost = HttpContext.Current.Items["Piranha_CurrentPost"] = null;
		}
		#endregion
	}
}
