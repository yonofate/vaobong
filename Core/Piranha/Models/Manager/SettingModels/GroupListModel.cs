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

namespace Piranha.Models.Manager.SettingModels
{
	/// <summary>
	/// Group list model for the manager area.
	/// </summary>
	public class GroupListModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the available groups.
		/// </summary>
		public List<SysGroup> Groups { get; set; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new list model.
		/// </summary>
		public GroupListModel() {
			Groups = new List<SysGroup>();
		}

		/// <summary>
		/// Gets all available data.
		/// </summary>
		/// <returns>The model</returns>
		public static GroupListModel Get() {
			GroupListModel m = new GroupListModel();

			m.Groups = SysGroup.GetStructure().Flatten();

			return m;
		}
	}
}
