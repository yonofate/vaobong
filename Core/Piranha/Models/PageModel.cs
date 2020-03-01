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
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

using Piranha.Extend;

namespace Piranha.Models
{
	/// <summary>
	/// Page view model.
	/// </summary>
	public class PageModel
	{
		#region Properties
		/// <summary>
		/// Gets the page.
		/// </summary>
		public IPage Page { get; set; }

		/// <summary>
		/// Gets the available html regions for the page.
		/// </summary>
		public dynamic Regions { get; set; }

		/// <summary>
		/// Gets the available properties.
		/// </summary>
		public dynamic Properties { get; set; }

		/// <summary>
		/// Gets the available extensions.
		/// </summary>
		public dynamic Extensions { get; set; }

		/// <summary>
		/// Gets the available attachments.
		/// </summary>
		public List<Content> Attachments { get; set; }

		/// <summary>
		/// Gets the available page blocks.
		/// </summary>
		public IList<PageModel> Blocks { get; set; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new empty page model.
		/// </summary>
		public PageModel() {
			Regions = new ExpandoObject();
			Properties = new ExpandoObject();
			Extensions = new ExpandoObject();
			Attachments = new List<Content>();
			Blocks = new List<PageModel>();
		}

		#region Static accessors
		/// <summary>
		/// Gets the page model for the given page.
		/// </summary>
		/// <param name="p">The page record</param>
		/// <returns>The model</returns>
		public static PageModel Get(Page p) {
			return Get<PageModel>(p);
		}

		/// <summary>
		/// Gets the page model for the given page.
		/// </summary>
		/// <param name="p">The page record</param>
		/// <returns>The model</returns>
		public static T Get<T>(Page p) where T : PageModel {
			T m = Activator.CreateInstance<T>();

			m.Page = p;
			m.Init();
			return m;
		}

		/// <summary>
		/// Gets the page model for the startpage.
		/// </summary>
		/// <returns>The model</returns>
		public static PageModel GetByStartpage() {
			return GetByStartpage<PageModel>();
		}

		/// <summary>
		/// Gets tne page model for the startpage.
		/// </summary>
		/// <typeparam name="T">The model type</typeparam>
		/// <returns>The model</returns>
		public static T GetByStartpage<T>() where T : PageModel {
			T m = Activator.CreateInstance<T>();

			m.Page = Models.Page.GetStartpage();
			m.Init();
			return m;
		}

		/// <summary>
		/// Gets the page model for the specified permalink.
		/// </summary>
		/// <param name="permalink">The permalink</param>
		/// <returns>The model</returns>
		public static PageModel GetByPermalink(string permalink) {
			return GetByPermalink<PageModel>(permalink);
		}

		/// <summary>
		/// Gets the page model for the specified permalink.
		/// </summary>
		/// <typeparam name="T">The model type</typeparam>
		/// <param name="permalink">The permalink</param>
		/// <returns>The model</returns>
		public static T GetByPermalink<T>(string permalink) where T : PageModel {
			T m = Activator.CreateInstance<T>();

			m.Page = Models.Page.GetByPermalink(permalink);
			m.Init();
			return m;
		}

		/// <summary>
		/// Gets the page model for the specified page id.
		/// </summary>
		/// <param name="id">The page id</param>
		/// <returns>The model</returns>
		public static PageModel GetById(Guid id) {
			PageModel m = new PageModel() {
				Page = Models.Page.GetSingle(id)
			};
			m.Init();
			return m;
		}

		/// <summary>
		/// Gets the site page for the site with the given id.
		/// </summary>
		/// <param name="siteid">The site id.</param>
		/// <returns>The site page model</returns>
		public static PageModel GetBySite(Guid siteid) {
			var cachename = "SITE_" + siteid.ToString();

			if (!Application.Current.CacheProvider.Contains(cachename)) {
				using (var db = new DataContext()) {
					var id = db.Pages.Where(p => p.SiteTreeId == siteid && p.ParentId == siteid).Select(p => p.Id).SingleOrDefault();
					if (id != Guid.Empty)
						Application.Current.CacheProvider[cachename] = GetById(id);
					else Application.Current.CacheProvider[cachename] = new PageModel();
				}
			}
			return (PageModel)Application.Current.CacheProvider[cachename];
		}

		/// <summary>
		/// Remmoves the site page model for the site with the given id.
		/// </summary>
		/// <param name="siteid">The site id</param>
		internal static void RemoveSitePageFromCache(Guid siteid) {
			var cachename = "SITE_" + siteid.ToString();

			if (Application.Current.CacheProvider.Contains(cachename))
				Application.Current.CacheProvider.Remove(cachename);
		}
		#endregion

		#region Helper methods
		/// <summary>
		/// Gets the region with the given internal id.
		/// </summary>
		/// <typeparam name="T">The target type</typeparam>
		/// <param name="internalId">The internal id</param>
		/// <returns>The region</returns>
		public T Region<T>(string internalId) {
			if (((IDictionary<string, object>)Regions).ContainsKey(internalId))
				return (T)((IDictionary<string, object>)Regions)[internalId];
			return default(T);
		}

		/// <summary>
		/// Gets the extension with the given internal id.
		/// </summary>
		/// <typeparam name="T">The target type</typeparam>
		/// <param name="internalId">The internal id</param>
		/// <returns>The extension</returns>
		public T Extension<T>(string internalId) {
			if (((IDictionary<string, object>)Extensions).ContainsKey(internalId))
				return (T)((IDictionary<string, object>)Extensions)[internalId];
			return default(T);
		}
		#endregion

		/// <summary>
		/// Gets the associated regions for the current page
		/// </summary>
		protected void Init() {
			var id = Page.Id;

			// Handle page copies
			if (((Models.Page)Page).OriginalId != Guid.Empty) {
				var copy = (Models.Page)Page;
				var org = Models.Page.GetSingle(copy.OriginalId, copy.IsDraft);

				copy.Id = org.Id;
				copy.GroupId = org.GroupId;
				copy.DisabledGroups = org.DisabledGroups;
				copy.Keywords = org.Keywords;
				copy.Description = org.Description;
				copy.LastModified = copy.LastModified > org.LastModified ? copy.LastModified : org.LastModified;
				copy.Attachments = org.Attachments;
			}

			// Get the page template
			PageTemplate pt = PageTemplate.GetSingle(((Page)Page).TemplateId);

			// Regions
			var regions = RegionTemplate.GetByTemplateId(pt.Id);
			if (regions.Count > 0) {
				foreach (var rt in regions) {
					if (rt.Type != "Piranha.Extend.Regions.PostRegion") {
						if (ExtensionManager.Current.HasType(rt.Type)) {
							// Create empty region
							object body = ExtensionManager.Current.CreateInstance(rt.Type);
							// Initialize empty regions
							if (body != null) {
								body = ((IExtension)body).GetContent(this);
							}
							((IDictionary<string, object>)Regions).Add(rt.InternalId, body);
						} else {
							((IDictionary<string, object>)Regions).Add(rt.InternalId, null);
						}
					} else {
						((IDictionary<string, object>)Regions).Add(rt.InternalId, new List<Piranha.Entities.Post>());
					}
				}
				Piranha.Models.Region.GetContentByPageId(Page.Id, Page.IsDraft).ForEach(reg => {
					string cachename = null;
					if (!Page.IsDraft)
						cachename = Extend.Regions.PostRegion.CacheName(Page, reg);

					if (!(reg.Body is Extend.Regions.PostRegion)) {
						object content = reg.Body;

						// Initialize region
						content = ((IExtension)content).GetContent(this);

						if (((IDictionary<string, object>)Regions).ContainsKey(reg.InternalId))
							((IDictionary<string, object>)Regions)[reg.InternalId] = content;
					} else {
						if (((IDictionary<string, object>)Regions).ContainsKey(reg.InternalId))
							((IDictionary<string, object>)Regions)[reg.InternalId] =
								((Extend.Regions.PostRegion)reg.Body).GetMatchingPosts(cachename);
					}
				});
			}
			// Properties
			if (pt.Properties.Count > 0) {
				foreach (string str in pt.Properties)
					((IDictionary<string, object>)Properties).Add(str, "");
				Property.GetContentByParentId(Page.Id, Page.IsDraft).ForEach(pr => {
					if (((IDictionary<string, object>)Properties).ContainsKey(pr.Name))
						((IDictionary<string, object>)Properties)[pr.Name] = pr.Value;
				});
			}
			// Attachments
			foreach (var guid in ((Models.Page)Page).Attachments) {
				var a = Models.Content.GetSingle(guid, Page.IsDraft);
				if (a != null)
					Attachments.Add(a);
			}
			// Extensions
			foreach (var ext in ((Page)Page).GetExtensions()) {
				object body = ext.Body;
				if (body != null) {
					var getContent = body.GetType().GetMethod("GetContent");
					if (getContent != null)
						body = getContent.Invoke(body, new object[] { this });
				}
				((IDictionary<string, object>)Extensions)[ExtensionManager.Current.GetInternalIdByType(ext.Type)] = body;
			}
			// Blocks
			var blocks = Piranha.Models.Page.GetFields("page_id, page_last_modified", "page_parent_id=@0 AND page_draft = 0 AND pagetemplate_is_block = 1", Page.Id, new Piranha.Data.Params() { OrderBy = "page_seqno" });
			foreach (var block in blocks) {
				var blockModel = PageModel.GetById(block.Id);

				// Add the block to the page model
				Blocks.Add(blockModel);

				// Check if we should update the page's last modified date.
				if (((Models.Page)blockModel.Page).LastModified > ((Models.Page)Page).LastModified)
					((Models.Page)Page).LastModified = ((Models.Page)blockModel.Page).LastModified;
			}			

			// Reset the page id if we changed it to load a copy
			((Models.Page)Page).Id = id;
		}
	}
}
