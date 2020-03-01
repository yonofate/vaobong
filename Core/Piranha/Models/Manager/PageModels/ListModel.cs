﻿/*
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

namespace Piranha.Models.Manager.PageModels
{
	/// <summary>
	/// Page list model for the manager area.
	/// </summary>
	public class ListModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the pages.
		/// </summary>
		public List<Sitemap> Pages { get; set; }

		/// <summary>
		/// Gets/sets the sitemap.
		/// </summary>
		public List<Sitemap> SiteMap { get; set; }

		/// <summary>
		/// Gets/sets the page templates.
		/// </summary>
		public List<PageTemplate> Templates { get; set; }

		/// <summary>
		/// Gets/sets the currently available site trees.
		/// </summary>
		public List<Entities.SiteTree> SiteTrees { get; set; }

		/// <summary>
		/// Gets/sets the internal id of the active site.
		/// </summary>
		public string ActiveSite { get; set; }

		/// <summary>
		/// Gets/sets the id of the active site.
		/// </summary>
		public Guid ActiveSiteId { get; set; }

		/// <summary>
		/// Gets/sets the list of all pages to create copies from.
		/// </summary>
		public List<Sitemap> AllPages { get; set; }

		/// <summary>
		/// Gets/sets if the site with the given id has a site page.
		/// </summary>
		public Dictionary<Guid, Guid> SitePage { get; set; }

		/// <summary>
		/// Gets/sets the number of seo warnings for the sites.
		/// </summary>
		public Dictionary<Guid, int> SiteWarnings { get; set; }

		/// <summary>
		/// GEts/sets the number of total seo warnings.
		/// </summary>
		public Dictionary<Guid, int> TotalSiteWarnings { get; set; }

		/// <summary>
		/// Gets/sets the number of completion warnings for the pages.
		/// </summary>
		public Dictionary<Guid, int> PageWarnings { get; set; }

		/// <summary>
		/// Gets/sets whether this is the seo list or not.
		/// </summary>
		public bool IsSeoList { get; set; }

		/// <summary>
		/// Gets/sets the default new seqno for pages.
		/// </summary>
		public int NewSeqno { get; set; }
		#endregion

		/// <summary>
		/// Default constructor, creates a new model.
		/// </summary>
		public ListModel() {
			Pages = new List<Sitemap>();
			Templates = PageTemplate.GetFields("pagetemplate_id, pagetemplate_name, pagetemplate_preview, pagetemplate_description, pagetemplate_is_block, pagetemplate_blocktypes, pagetemplate_subpages",
				"pagetemplate_site_template = 0", new Params() { OrderBy = "pagetemplate_name ASC" });
			AllPages = Sitemap.GetFields("page_id, page_title, page_navigation_title, pagetemplate_name, sitetree_name", "page_draft = 1 AND page_original_id IS NULL AND (page_parent_id IS NULL OR page_parent_id NOT IN (SELECT sitetree_id FROM sitetree))",
				new Params() { OrderBy = "sitetree_name, COALESCE(page_navigation_title, page_title)" });

			SitePage = new Dictionary<Guid, Guid>();
			SiteWarnings = new Dictionary<Guid, int>();
			TotalSiteWarnings = new Dictionary<Guid, int>();
			PageWarnings = new Dictionary<Guid, int>();

			using (var db = new DataContext()) {
				SiteTrees = db.SiteTrees.OrderBy(s => s.Name).ToList();

				foreach (var site in SiteTrees) {
					SitePage[site.Id] = db.Pages.Where(p => p.SiteTreeId == site.Id && p.ParentId == site.Id).Select(p => p.Id).SingleOrDefault();
					SiteWarnings[site.Id] = 0 + (String.IsNullOrEmpty(site.MetaTitle) ? 1 : 0) + (String.IsNullOrEmpty(site.MetaDescription) ? 1 : 0);

					TotalSiteWarnings[site.Id] =
						Page.GetScalar("SELECT COUNT(*) FROM page JOIN pagetemplate ON pagetemplate_id = page_template_id WHERE page_draft = 1 AND page_sitetree_id = @0 AND page_parent_id != @0 AND page_original_id IS NULL AND page_published IS NOT NULL AND pagetemplate_is_block = 0 AND page_keywords IS NULL", site.Id) +
						Page.GetScalar("SELECT COUNT(*) FROM page JOIN pagetemplate ON pagetemplate_id = page_template_id WHERE page_draft = 1 AND page_sitetree_id = @0 AND page_parent_id != @0 AND page_original_id IS NULL AND page_published IS NOT NULL AND pagetemplate_is_block = 0 AND page_description IS NULL", site.Id);
				}
			}
		}

		/// <summary>
		/// Gets the list model for all available pages.
		/// </summary>
		/// <param name="internalId">Optional internal id of the site tree</param>
		/// <returns>The model.</returns>
		public static ListModel Get(string internalId = "") {
			if (String.IsNullOrEmpty(internalId))
				internalId = Config.SiteTree;

			ListModel m = new ListModel();
			m.SiteMap = Sitemap.GetStructure(internalId, false);
			m.Pages = Sitemap.GetStructure(internalId, false).Flatten();
			m.ActiveSite = internalId.ToUpper();
			m.NewSeqno = m.SiteMap.Count + 1;

			using (var db = new DataContext()) {
				m.ActiveSiteId = db.SiteTrees.Where(s => s.InternalId == internalId).Select(s => s.Id).Single();
			}

			// Check completion warnings
			foreach (var page in m.Pages) {
				m.PageWarnings[page.Id] = 0 + (String.IsNullOrEmpty(page.Keywords) ? 1 : 0) + (String.IsNullOrEmpty(page.Description) ? 1 : 0);
			}
			return m;
		}

		/// <summary>
		/// Gets the SEO list for the given internal id.
		/// </summary>
		/// <param name="internalId">The internal id.</param>
		/// <returns>The SEO list</returns>
		public static ListModel GetSEO(string internalId = "") {
			if (String.IsNullOrEmpty(internalId))
				internalId = Config.SiteTree;

			ListModel m = new ListModel();
			m.Pages = Sitemap.GetStructure(internalId, false).Flatten()
				.Where(s => !s.IsBlock && s.Published != DateTime.MinValue && (String.IsNullOrEmpty(s.Keywords) || String.IsNullOrEmpty(s.Description))).ToList();
			m.ActiveSite = internalId.ToUpper();

			using (var db = new DataContext()) {
				m.ActiveSiteId = db.SiteTrees.Where(s => s.InternalId == internalId).Select(s => s.Id).Single();
				m.NewSeqno = db.Pages.Where(p => p.ParentId == null).Count() + 1;
			}

			// Check completion warnings
			foreach (var page in m.Pages) {
				m.PageWarnings[page.Id] = 0 + (String.IsNullOrEmpty(page.Keywords) ? 1 : 0) + (String.IsNullOrEmpty(page.Description) ? 1 : 0);
			}
			m.IsSeoList = true;
			return m;
		}
	}
}
