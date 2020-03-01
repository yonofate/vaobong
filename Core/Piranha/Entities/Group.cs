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

namespace Piranha.Entities
{
	/// <summary>
	/// The group entity.
	/// </summary>
	[Serializable]
	public class Group : BaseEntity
	{
		#region Properties
		/// <summary>
		/// Gets/sets the unique group id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets/sets the id of the optional parent group.
		/// </summary>
		public Guid? ParentId { get; set; }

		/// <summary>
		/// Gets/sets the unique group name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the group description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets the date the group was initially created.
		/// </summary>
		public DateTime Created { get; set; }

		/// <summary>
		/// Gets/sets the date the group was last updated.
		/// </summary>
		public DateTime Updated { get; set; }

		/// <summary>
		/// Gets/sets the id of the user who initially created the group.
		/// </summary>
		public Guid? CreatedById { get; set; }

		/// <summary>
		/// Gets/sets the id of the user who last updated the group.
		/// </summary>
		public Guid? UpdatedById { get; set; }
		#endregion

		#region Navigation properties
		/// <summary>
		/// Gets/sets the optional parent group.
		/// </summary>
		public Group Parent { get; set; }

		/// <summary>
		/// Gets/sets the users who belong to the group.
		/// </summary>
		public IList<User> Users { get; set; }

		/// <summary>
		/// Gets/sets the permissions attached to the group.
		/// </summary>
		public IList<Permission> Permissions { get; set; }

		/// <summary>
		/// Gets/sets the currently available extensions.
		/// </summary>
		public IList<Extension> Extensions { get; set; }

		/// <summary>
		/// Gets/sets the user who initially created the group.
		/// </summary>
		public User CreatedBy { get; set; }

		/// <summary>
		/// Gets/sets the user who last updated the group.
		/// </summary>
		public User UpdatedBy { get; set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Group() {
			Extensions = new List<Extension>();
		}
	}
}
