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
	public class PostListModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the list of templates.
		/// </summary>
		public List<PostTemplate> Templates { get; set; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new model.
		/// </summary>
		public PostListModel() {
			Templates = new List<PostTemplate>();
			Extend.ExtensionManager.Current.ForceInit();
		}

		/// <summary>
		/// Gets the list model for all post templates.
		/// </summary>
		public static PostListModel Get() {
			PostListModel m = new PostListModel();
			m.Templates = PostTemplate.GetFields("posttemplate_id,posttemplate_name,posttemplate_type,posttemplate_created,posttemplate_updated",
				new Params() { OrderBy = "posttemplate_name" });
			return m;
		}
	}
}
