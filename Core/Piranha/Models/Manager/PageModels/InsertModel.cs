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

namespace Piranha.Models.Manager.PageModels
{
	/// <summary>
	/// Page insert model for the manager area.
	/// </summary>
	public class InsertModel
	{
		#region Properties
		/// <summary>
		/// Page template id.
		/// </summary>
		public Guid TemplateId { get; set; }

		/// <summary>
		/// Parent id.
		/// </summary>
		public Guid ParentId { get; set; }

		/// <summary>
		/// Page seqno
		/// </summary>
		public int Seqno { get; set; }

		/// <summary>
		/// The id of the page to create a copy of.
		/// </summary>
		public Guid OriginalId { get; set; }

		/// <summary>
		/// Internal id of the site tree.
		/// </summary>
		public string SiteTree { get; set; }

		/// <summary>
		/// Gets the id of the selected site tree.
		/// </summary>
		public Guid SiteTreeId {
			get {
				using (var db = new DataContext()) {
					return db.SiteTrees.Where(s => s.InternalId == SiteTree).Select(s => s.Id).Single();
				}
			}
		}
		#endregion

		/// <summary>
		/// Default constructor, creates a new model.
		/// </summary>
		public InsertModel() {
			TemplateId = Guid.Empty;
			ParentId = Guid.Empty;
			Seqno = 1;
			SiteTree = Config.SiteTree;
		}
	}
}
