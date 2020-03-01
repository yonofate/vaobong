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

namespace Piranha.Entities
{
	/// <summary>
	/// The permission entity.
	/// </summary>
	[Serializable]
	public class Permission : StandardEntity<Permission>
	{
		#region Properties
		/// <summary>
		/// Gets/sets the id of the group who is attached to the permission.
		/// </summary>
		public Guid GroupId { get; set; }

		/// <summary>
		/// Gets/sets the name of the permission.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the description shown in the manager interface.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets whether this permission can be removed or not.
		/// </summary>
		public bool IsLocked { get; set; }
		#endregion

		#region Navigation properties
		/// <summary>
		/// Gets/sets the group attached to the permission.
		/// </summary>
		public Group Group { get; set; }
		#endregion
	}
}
