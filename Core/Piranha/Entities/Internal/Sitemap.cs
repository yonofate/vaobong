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

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Active record for a sitemap.
	/// </summary>
	[Table(Name = "page")]
	[Join(TableName = "pagetemplate", ForeignKey = "page_template_id", PrimaryKey = "pagetemplate_id")]
	[Join(TableName = "permalink", ForeignKey = "page_permalink_id", PrimaryKey = "permalink_id")]
	[Join(TableName = "sitetree", ForeignKey = "page_sitetree_id", PrimaryKey = "sitetree_id")]
	[Serializable]
	public class Sitemap : PiranhaRecord<Sitemap>, ISitemap
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name = "page_id")]
		public override Guid Id { get; set; }

		/// <summary>
		/// Gets/sets the site tree id.
		/// </summary>
		[Column(Name = "page_sitetree_id")]
		public Guid SiteTreeId { get; set; }

		/// <summary>
		/// Gets/sets the site tree internal id.
		/// </summary>
		[Column(Name = "sitetree_internal_id", Table = "sitetree", ReadOnly = true)]
		public string SiteTreeInternalId { get; set; }

		/// <summary>
		/// Gets/sets the site tree name.
		/// </summary>
		[Column(Name = "sitetree_name", Table = "sitetree", ReadOnly = true)]
		public string SiteTreeName { get; set; }

		/// <summary>
		/// Gets/sets the original id if this is a copy.
		/// </summary>
		[Column(Name = "page_original_id")]
		public Guid OriginalId { get; set; }

		/// <summary>
		/// Gets/sets the group needed to view the page.
		/// </summary>
		[Column(Name = "page_group_id")]
		public Guid GroupId { get; set; }

		/// <summary>
		/// Gets/sets the parent id.
		/// </summary>
		[Column(Name = "page_parent_id")]
		public Guid ParentId { get; set; }

		/// <summary>
		/// Gets/sets the seqno specifying the structural position.
		/// </summary>
		[Column(Name = "page_seqno")]
		public int Seqno { get; set; }

		/// <summary>
		/// Gets/sets the title.
		/// </summary>
		[Column(Name = "page_title")]
		public string Title { get; set; }

		/// <summary>
		/// Gets/sets the meta keywords.
		/// </summary>
		[Column(Name = "page_keywords")]
		public string Keywords { get; set; }

		/// <summary>
		/// Gets/sets the meta description.
		/// </summary>
		[Column(Name = "page_description")]
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets the title.
		/// </summary>
		[Column(Name = "page_navigation_title")]
		public string NavigationTitle { get; set; }

		/// <summary>
		/// Gets/sets whether the page should be visible in menus or not.
		/// </summary>
		[Column(Name = "page_is_hidden")]
		public bool IsHidden { get; set; }

		/// <summary>
		/// Gets/sets the permalink.
		/// </summary>
		//[Column(Name="page_permalink")]
		[Column(Name = "permalink_name", Table = "permalink")]
		public string Permalink { get; private set; }

		/// <summary>
		/// Gets/sets the template id.
		/// </summary>
		[Column(Name = "page_template_id")]
		public Guid TemplateId { get; set; }

		/// <summary>
		/// Gets/sets the template name.
		/// </summary>
		[Column(Name = "pagetemplate_name", Table = "pagetemplate")]
		public string TemplateName { get; private set; }

		/// <summary>
		/// Gets/sets the page controller.
		/// </summary>
		[Column(Name = "page_controller")]
		private string PageController { get; set; }

		/// <summary>
		/// Gets/sets the template controller.
		/// </summary>
		[Column(Name = "pagetemplate_controller", Table = "pagetemplate")]
		private string TemplateController { get; set; }

		/// <summary>
		/// Gets/sets the custom redirect.
		/// </summary>
		[Column(Name = "page_redirect")]
		public string PageRedirect { get; set; }

		/// <summary>
		/// Gets/sets the custom controller.
		/// </summary>
		[Column(Name = "pagetemplate_redirect", Table = "pagetemplate")]
		private string TemplateRedirect { get; set; }

		/// <summary>
		/// Gets/sets if this is a page block.
		/// </summary>
		[Column(Name = "pagetemplate_is_block", Table = "pagetemplate")]
		public bool IsBlock { get; set; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name = "page_created")]
		public override DateTime Created { get; set; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		[Column(Name = "page_updated")]
		public override DateTime Updated { get; set; }

		/// <summary>
		/// Gets/sets the published date.
		/// </summary>
		[Column(Name = "page_published")]
		public DateTime Published { get; set; }

		/// <summary>
		/// Gets/sets the last published date.
		/// </summary>
		[Column(Name = "page_last_published")]
		public DateTime LastPublished { get; set; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name = "page_created_by")]
		public override Guid CreatedBy { get; set; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name = "page_updated_by")]
		public override Guid UpdatedBy { get; set; }
		#endregion

		#region Properties
		/// <summary>
		/// Gets the controller for the page.
		/// </summary>
		public string Controller {
			get { return !String.IsNullOrEmpty(PageController) ? PageController : TemplateController; }
		}

		/// <summary>
		/// Gets the redirect for the page.
		/// </summary>
		public string Redirect {
			get { return !String.IsNullOrEmpty(PageRedirect) ? PageRedirect : TemplateRedirect; }
		}

		/// <summary>
		/// Gets/sets the page level.
		/// </summary>
		public int Level { get; private set; }

		/// <summary>
		/// Gets whether the page is published or not.
		/// </summary>
		public bool IsPublished {
			get { return Published != DateTime.MinValue && Published < DateTime.Now; }
		}

		/// <summary>
		/// Gets if the current the page is the site startpage.
		/// </summary>
		public bool IsStartpage {
			get { return ParentId == Guid.Empty && Seqno == 1; }
		}

		/// <summary>
		/// Gets/sets the possible subpages.
		/// </summary>
		public List<Sitemap> Pages { get; set; }
		#endregion

		/// <summary>
		/// Default constructor, creates a new sitemap record.
		/// </summary>
		public Sitemap() {
			Pages = new List<Sitemap>();
		}

		/// <summary>
		/// Gets the sorted sitemap structure.
		/// </summary>
		/// <param name="published">Whether only published pages should be included.</param>
		/// <returns>The site structure</returns>
		public static List<Sitemap> GetStructure(bool published = true) {
			return GetStructure(Config.SiteTree, published);
		}

		/// <summary>
		/// Gets the sorted sitemap structure for the site tree with the given internal id.
		/// </summary>
		/// <param name="internalId">The internal id of the site tree</param>
		/// <param name="published">Whether to only get published pages.</param>
		/// <returns>The sitemap</returns>
		public static List<Sitemap> GetStructure(string internalId, bool published = true) {
			if (published) {
				// Return the cached public sitemap if it exists.
				if (published && Piranha.Application.Current.CacheProvider[typeof(Sitemap).Name + "_" + internalId] != null)
					return (List<Sitemap>)Piranha.Application.Current.CacheProvider[typeof(Sitemap).Name + "_" + internalId];

				// Get the sitemap from the database
				List<Sitemap> pages = Get("sitetree_internal_id = @0 AND page_draft = 0", internalId, new Params() { OrderBy = "page_parent_id, page_seqno" });
				pages = Sort(pages, Guid.Empty);

				// If this is the public sitemap, cache it
				if (published)
					Piranha.Application.Current.CacheProvider[typeof(Sitemap).Name + "_" + internalId] = pages;
				return pages;
			} else {
				// Get the sitemap from the database
				List<Sitemap> pages = Get("sitetree_internal_id = @0 AND page_draft = 1", internalId, new Params() { OrderBy = "page_parent_id, page_seqno" });
				pages = Sort(pages, Guid.Empty);
				return pages;
			}
		}

		/// <summary>
		/// Invalidate the cache.
		/// </summary>
		/// <param name="internalId">The internal id of the sitemap</param>
		public static void InvalidateCache(string internalId = "DEFAULT_SITE") {
			Piranha.Application.Current.CacheProvider.Remove(typeof(Sitemap).Name + "_" + internalId);
		}

		/// <summary>
		/// Checks if the current element has a child with the given id.
		/// </summary>
		/// <param name="page">The sitemap element</param>
		/// <param name="id">The page id to search for</param>
		/// <returns>If the child id is found</returns>
		public bool HasChild(Guid id) {
			foreach (var sr in Pages)
				if (HasChild(sr, id))
					return true;
			return false;
		}

		#region Private methods
		/// <summary>
		/// Checks if the given sitemap has a child with the given id.
		/// </summary>
		/// <param name="page">The sitemap element</param>
		/// <param name="id">The page id to search for</param>
		/// <returns>If the child id is found</returns>
		private bool HasChild(Sitemap page, Guid id) {
			if (page.Id == id)
				return true;
			foreach (var sr in page.Pages) {
				if (HasChild(sr, id))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Sorts the page structure recursive.
		/// </summary>
		/// <param name="pages">The pages</param>
		/// <param name="parentid">The parent id</param>
		/// <returns>The site structure</returns>
		private static List<Sitemap> Sort(List<Sitemap> pages, Guid parentid, int level = 1) {
			List<Sitemap> ret = new List<Sitemap>();

			foreach (Sitemap page in pages) {
				if (page.ParentId == parentid) {
					page.Level = level;
					page.Pages = Sort(pages, page.Id, level + 1);
					ret.Add(page);
				}
			}
			return ret;
		}
		#endregion
	}

	/// <summary>
	/// Extension methods
	/// </summary>
	public static class SitemapHelpers
	{
		/// <summary>
		/// Flattens the recursive strucutre and returns it as a list.
		/// </summary>
		/// <returns>The sitemap</returns>
		public static List<Sitemap> Flatten(this List<Sitemap> pages) {
			List<Sitemap> ret = new List<Sitemap>();

			foreach (Sitemap page in pages) {
				ret.Add(page);
				if (page.Pages.Count > 0)
					ret.AddRange(Flatten(page.Pages));
			}
			return ret;
		}

		/// <summary>
		/// Counts the number of visible pages in the given list.
		/// </summary>
		/// <param name="pages">The pages</param>
		/// <returns>The number of visible pages</returns>
		public static int CountVisible(this List<Sitemap> pages) {
			int count = 0;
			foreach (Sitemap page in pages)
				if (!page.IsHidden && !page.IsBlock) count++;
			return count;
		}
	}
}
