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

using Piranha.Data;

namespace Piranha.Models.Manager.TemplateModels
{
	/// <summary>
	/// Template list model for the manager area.
	/// </summary>
	public class PageListModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the page templates.
		/// </summary>
		public List<PageTemplate> Templates { get; set; }
		#endregion

		/// <summary>
		/// Default constructor, creates a new model.
		/// </summary>
		public PageListModel() {
			Templates = new List<PageTemplate>();
			Extend.ExtensionManager.Current.ForceInit();
		}

		/// <summary>
		/// Gets the list model for all available templates.
		/// </summary>
		/// <returns>The model.</returns>
		public static PageListModel Get() {
			PageListModel m = new PageListModel();
			m.Templates = PageTemplate.GetFields("pagetemplate_id,pagetemplate_name,pagetemplate_type,pagetemplate_created,pagetemplate_updated",
				"pagetemplate_site_template = 0", new Params() { OrderBy = "pagetemplate_name ASC" });
			return m;
		}
	}
}
