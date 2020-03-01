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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Piranha.Data;
using Piranha.Entities;

namespace Piranha.Extend.Regions
{
	/// <summary>
	/// Post region returning the full post models.
	/// </summary>
	[Export(typeof(IExtension))]
	[ExportMetadata("InternalId", "PostModelRegion")]
	[ExportMetadata("Name", "PostModelRegionName")]
	[ExportMetadata("ResourceType", typeof(Resources.Extensions))]
	[ExportMetadata("Type", ExtensionType.Region)]
	[Serializable]
	public class PostModelRegion : Extension
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
		public PostModelRegion() {
			Posts = new List<Post>();
			Take = 2;
			OrderBy = OrderByType.PUBLISHED;
		}

		/// <summary>
		/// Gets the content for the client model.
		/// </summary>
		/// <param name="model">The current model</param>
		/// <returns>The post models</returns>
		public override object GetContent(object model) {
			if (PostTemplateId != Guid.Empty) {
				var posts = PostModel.Where(p => p.TemplateId == PostTemplateId);

				if (OrderBy == OrderByType.PUBLISHED)
					posts = posts.OrderByDescending(p => p.Post.Published).ToList();
				else if (OrderBy == OrderByType.LAST_PUBLISHED)
					posts = posts.OrderByDescending(p => p.Post.LastPublished).ToList();
				else if (OrderBy == OrderByType.TITLE)
					posts = posts.OrderBy(p => p.Post.Title).ToList();

				if (Take > 0)
					posts = posts.Take(Take).ToList();
				return posts;
			}
			return new List<PostModel>();
		}

		/// <summary>
		/// Initializes the region.
		/// </summary>
		public override void InitManager(object model) {
			using (var db = new DataContext()) {
				// Get all of the post types
				var templates = db.PostTemplates.OrderBy(t => t.Name).ToList();
				templates.Insert(0, new PostTemplate());
				TemplateTypes = new SelectList(templates, "Id", "Name", PostTemplateId);

				if (PostTemplateId != Guid.Empty) {
					// Get the currently matching posts
					var query = db.Posts.Where(p => p.TemplateId == PostTemplateId);

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
					Posts = query.ToList();
				} else {
					Posts = new List<Post>();
				}
			}

			// Gets all of the order by types
			List<SelectListItem> orderby = new List<SelectListItem>();
			orderby.Add(new SelectListItem() { Text = Piranha.Resources.Global.Published, Value = "PUBLISHED" });
			orderby.Add(new SelectListItem() { Text = Piranha.Resources.Global.LastPublished, Value = "LAST_PUBLISHED" });
			orderby.Add(new SelectListItem() { Text = Piranha.Resources.Post.Title, Value = "TITLE" });
			OrderByTypes = new SelectList(orderby, "Value", "Text", OrderBy);
		}
	}
}