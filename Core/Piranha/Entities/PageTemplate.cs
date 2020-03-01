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
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Piranha.Entities
{
	/// <summary>
	/// The page template entity.
	/// </summary>
	[Serializable]
	public class PageTemplate : StandardEntity<PageTemplate>
	{
		#region Properties
		/// <summary>
		/// Gets/sets the template name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the description for the manager interface.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets the html-preview for the manager interface.
		/// </summary>
		public string Preview { get; set; }

		/// <summary>
		/// Gets/sets the properties defined. This field is NOT searchable in queries.
		/// </summary>
		public IList<string> Properties { get; set; }

		/// <summary>
		/// Gets/sets the optional template that should render the view.
		/// </summary>
		public string ViewTemplate { get; set; }

		/// <summary>
		/// Gets/sets if the page should be able to override the template.
		/// </summary>
		public bool ShowViewTemplate { get; set; }

		/// <summary>
		/// Gets/sets the optional uri to which the page should redirect.
		/// </summary>
		public string ViewRedirect { get; set; }

		/// <summary>
		/// Gets/sets if the page should be able to override the redirect.
		/// </summary>
		public bool ShowViewRedirect { get; set; }

		/// <summary>
		/// Gets/sets if this is a site template.
		/// </summary>
		public bool IsSiteTemplate { get; set; }

		/// <summary>
		/// Gets/sets if this is a page block template.
		/// </summary>
		public bool IsBlock { get; set; }

		/// <summary>
		/// Gets/sets the type that created this template if it was create by code.
		/// </summary>
		public string Type { get; set; }
		#endregion

		#region Navigation properties
		/// <summary>
		/// Gets/sets the region template associated with this template.
		/// </summary>
		public IList<RegionTemplate> RegionTemplates { get; set; }
		#endregion

		#region Internal properties
		/// <summary>
		/// Gets/sets the persisted json data for the properties.
		/// </summary>
		internal string PropertiesJson { get; set; }
		#endregion

		/// <summary>
		/// Default constructor. Creates an empty page template.
		/// </summary>
		public PageTemplate() {
			Properties = new List<string>();
			RegionTemplates = new List<RegionTemplate>();
		}

		#region Events
		/// <summary>
		/// Called when the entity has been loaded.
		/// </summary>
		/// <param name="db">The db context</param>
		public override void OnLoad(DataContext db) {
			var js = new JavaScriptSerializer();

			Properties = !String.IsNullOrEmpty(PropertiesJson) ? js.Deserialize<List<string>>(PropertiesJson) : Properties;

			base.OnLoad(db);
		}

		/// <summary>
		/// Called when the entity is about to be saved.
		/// </summary>
		/// <param name="db">The db context</param>
		/// <param name="state">The current entity state</param>
		public override void OnSave(DataContext db, EntityState state) {
			var js = new JavaScriptSerializer();

			PropertiesJson = js.Serialize(Properties);

			base.OnSave(db, state);
		}
		#endregion
	}
}
