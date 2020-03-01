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
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Mvc;

using Piranha.Data;
using Piranha.Entities;
using System.ComponentModel.Composition;

namespace Piranha.Extend.Regions
{
	/// <summary>
	/// Standard post container region.
	/// </summary>
	[Export(typeof(IExtension))]
	[ExportMetadata("InternalId", "PostRegion")]
	[ExportMetadata("Name", "PostRegionName")]
	[ExportMetadata("ResourceType", typeof(Resources.Extensions))]
	[ExportMetadata("Type", ExtensionType.Region)]
	[Serializable]
	public class PostRegion : Extension
	{
		#region Inner classes
		/// <summary>
		/// Gets/sets the different ways to order the posts.
		/// </summary>
		public enum OrderByType
		{
			TITLE, PUBLISHED, LAST_PUBLISHED
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the post type that should be included.
		/// </summary>
		[Display(ResourceType = typeof(Piranha.Resources.Extensions), Name = "PostRegionTemplate")]
		public Guid PostTemplateId { get; set; }

		/// <summary>
		/// Gets/sets how posts should be ordered.
		/// </summary>
		[Display(ResourceType = typeof(Piranha.Resources.Extensions), Name = "PostRegionOrderBy")]
		public OrderByType OrderBy { get; set; }

		/// <summary>
		/// Gets/sets the amount of posts that should be fetched.
		/// </summary>
		[Display(ResourceType = typeof(Piranha.Resources.Extensions), Name = "PostRegionTake")]
		public int Take { get; set; }

		/// <summary>
		/// Gets/sets the number of minutes the posts should be cached server-side.
		/// </summary>
		[Display(ResourceType = typeof(Piranha.Resources.Extensions), Name = "PostRegionCache")]
		public int Cache { get; set; }

		/// <summary>
		/// Gets/sets if the user who created the post should be included.
		/// </summary>
		[Display(ResourceType = typeof(Piranha.Resources.Extensions), Name = "PostRegionIncludeCreatedBy")]
		public bool IncludeCreatedBy { get; set; }

		/// <summary>
		/// Gets/sets if the user who last updated the post should be included.
		/// </summary>
		[Display(ResourceType = typeof(Piranha.Resources.Extensions), Name = "PostRegionIncludeUpdatedBy")]
		public bool IncludeUpdatedBy { get; set; }

		/// <summary>
		/// Gets/sets if the categories should be included.
		/// </summary>
		[Display(ResourceType = typeof(Piranha.Resources.Extensions), Name = "PostRegionIncludeCategories")]
		public bool IncludeCategories { get; set; }

		/// <summary>
		/// Gets/sets if the template should be included.
		/// </summary>
		[Display(ResourceType = typeof(Piranha.Resources.Extensions), Name = "PostRegionIncludeTemplate")]
		public bool IncludeTemplate { get; set; }
		#endregion

		#region Ignored properties
		/// <summary>
		/// Gets the available templates.
		/// </summary>
		[ScriptIgnore()]
		public SelectList TemplateTypes { get; private set; }

		/// <summary>
		/// Gets the available order by types.
		/// </summary>
		[ScriptIgnore()]
		public SelectList OrderByTypes { get; private set; }

		/// <summary>
		/// Gets the posts matching the given criterias.
		/// </summary>
		[ScriptIgnore()]
		public List<Post> Posts { get; private set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public PostRegion() {
			Posts = new List<Post>();
			Take = 2;
			OrderBy = OrderByType.PUBLISHED;
			IncludeCategories = true;
			IncludeTemplate = false;
			IncludeCreatedBy = true;
			IncludeUpdatedBy = false;
		}

		/// <summary>
		/// Generates the cache name for the current region on the current page.
		/// </summary>
		/// <param name="page">The page</param>
		/// <param name="reg">The region</param>
		/// <returns>The cache name</returns>
		public static string CacheName(Piranha.Models.IPage page, Piranha.Models.Region reg) {
			return "CACHE_" + page.Permalink.ToUpper() + "_" + reg.Type.ToUpper().Replace(".", "_") + "_" + reg.InternalId.ToUpper();
		}

		/// <summary>
		/// Clears the cache for the given key.
		/// </summary>
		/// <param name="cachename">The cache name</param>
		public virtual void ClearCache(string cachename) {
			if (HttpContext.Current.Cache[cachename] != null)
				HttpContext.Current.Cache.Remove(cachename);
		}

		/// <summary>
		/// Clears the cache for the given page & region.
		/// </summary>
		/// <param name="page">The page</param>
		/// <param name="reg">The region</param>
		public void ClearCache(Piranha.Models.IPage page, Piranha.Models.Region reg) {
			ClearCache(CacheName(page, reg));
		}

		/// <summary>
		/// Gets the posts matching the current post region.
		/// </summary>
		/// <returns>The posts</returns>
		public virtual List<Post> GetMatchingPosts(string cachename = null, bool admin = false) {
			if (PostTemplateId != Guid.Empty) {
				List<Post> posts = null;
				if (!String.IsNullOrEmpty(cachename) && Cache > 0)
					posts = (List<Post>)HttpContext.Current.Cache[cachename];

				if (posts == null) {
					using (var db = new DataContext()) {
						// Create base query
						var query = db.Posts.Include(p => p.Permalink).Where(p => p.TemplateId == PostTemplateId);

						// Include
						if (!admin && IncludeCategories)
							query = query.Include(p => p.Categories);
						if (admin || IncludeTemplate)
							query = query.Include(p => p.Template);
						if (!admin && IncludeCreatedBy)
							query = query.Include(p => p.CreatedBy);
						if (!admin && IncludeUpdatedBy)
							query = query.Include(p => p.UpdatedBy);

						// Order
						if (OrderBy == OrderByType.PUBLISHED)
							query = query.OrderByDescending(p => p.Published);
						else if (OrderBy == OrderByType.LAST_PUBLISHED)
							query = query.OrderByDescending(p => p.LastPublished);
						else if (OrderBy == OrderByType.TITLE)
							query = query.OrderBy(p => p.Title);

						// Take
						if (Take > 0)
							query = query.Take(Take);

						// Execute query
						posts = query.ToList();
					}
				}
				if (!String.IsNullOrEmpty(cachename) && Cache > 0 && HttpContext.Current.Cache[cachename] == null) {
					HttpContext.Current.Cache.Add(cachename, posts, null, DateTime.Now.AddMinutes(Cache),
						System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
				}
				return posts;
			}
			return new List<Post>();
		}

		/// <summary>
		/// Initializes the region.
		/// </summary>
		public override void InitManager(object model) {
			// Gets all of the post templates
			var templates = Piranha.Models.PostTemplate.GetFields("posttemplate_id, posttemplate_name", new Params() { OrderBy = "posttemplate_name" });
			templates.Insert(0, new Piranha.Models.PostTemplate());
			TemplateTypes = new SelectList(templates, "Id", "Name", PostTemplateId);

			// Gets all of the order by types
			List<SelectListItem> orderby = new List<SelectListItem>();
			orderby.Add(new SelectListItem() { Text = Piranha.Resources.Global.Published, Value = "PUBLISHED" });
			orderby.Add(new SelectListItem() { Text = Piranha.Resources.Global.LastPublished, Value = "LAST_PUBLISHED" });
			orderby.Add(new SelectListItem() { Text = Piranha.Resources.Post.Title, Value = "TITLE" });
			OrderByTypes = new SelectList(orderby, "Value", "Text", OrderBy);

			// Gets all of the matching posts
			Posts = GetMatchingPosts(null, true);
		}
	}
}