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
	/// User list model for the manager area.
	/// </summary>
	public class UserListModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the available users.
		/// </summary>
		public List<SysUser> Users { get; set; }

		/// <summary>
		/// Gets/sets the available groups.
		/// </summary>
		public List<SysGroup> Groups { get; set; }

		/// <summary>
		/// Gets/sets the currently active group.
		/// </summary>
		public Guid ActiveGroup { get; set; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new list model.
		/// </summary>
		public UserListModel() {
			Users = new List<SysUser>();
			Groups = new List<SysGroup>();
		}

		/// <summary>
		/// Gets all available data.
		/// </summary>
		/// <returns>The model</returns>
		public static UserListModel Get() {
			UserListModel m = new UserListModel();

			m.Users = SysUser.Get(new Params() { OrderBy = "sysuser_login" });
			m.Groups = SysGroup.GetStructure().Flatten();

			return m;
		}

		/// <summary>
		/// Gets the available user for the given group.
		/// </summary>
		/// <param name="groupId">The group id</param>
		/// <returns>The model</returns>
		public static UserListModel GetByGroupId(Guid groupId) {
			UserListModel m = new UserListModel();

			m.Users = SysUser.Get("sysuser_group_id= @0", groupId, new Params() { OrderBy = "sysuser_login" });
			m.Groups = SysGroup.GetStructure().Flatten();
			m.ActiveGroup = groupId;

			return m;
		}
	}
}
