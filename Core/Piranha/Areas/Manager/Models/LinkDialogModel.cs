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

using Piranha.Models;

namespace Piranha.Areas.Manager.Models
{
	public class LinkDialogModel
	{
		#region Properties
		/// <summary>
		/// Get/sets the currently active site.
		/// </summary>
		public Guid ActiveSite { get; set; }

		/// <summary>
		/// Gets/sets the sitemap for the current site.
		/// </summary>
		public IList<Sitemap> Pages { get; set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public LinkDialogModel() {
			Pages = new List<Sitemap>();
		}

		/// <summary>
		/// Gets the model for the given site.
		/// </summary>
		/// <param name="siteid">The site id</param>
		/// <returns>The model</returns>
		public static LinkDialogModel GetBySiteId(Guid siteid) {
			var m = new LinkDialogModel();

			m.ActiveSite = siteid;

			using (var db = new DataContext()) {
				var site = db.SiteTrees.Where(s => s.Id == m.ActiveSite).Single();
				m.Pages = Sitemap.GetStructure(site.InternalId);
			}
			return m;
		}
	}
}