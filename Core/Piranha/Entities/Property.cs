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
	/// The property entity
	/// </summary>
	[Serializable]
	public class Property : StandardEntity<Property>
	{
		#region Properties
		/// <summary>
		/// Gets/sets whether this property instance is a draft or not.
		/// </summary>
		internal bool IsDraft { get; set; }

		/// <summary>
		/// Gets/sets the id of the parent this property is attached to.
		/// </summary>
		public Guid ParentId { get; set; }

		/// <summary>
		/// Gets/sets the property name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the property value.
		/// </summary>
		public string Value { get; set; }
		#endregion
	}
}
