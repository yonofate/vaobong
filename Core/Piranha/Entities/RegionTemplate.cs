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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Piranha.Entities
{
	/// <summary>
	/// Template for defining regions for a page type.
	/// </summary>
	[Serializable]
	public class RegionTemplate : StandardEntity<RegionTemplate>
	{
		#region Properties
		/// <summary>
		/// Gets/sets the template this regions template is attached to.
		/// </summary>
		public Guid TemplateId { get; set; }

		/// <summary>
		/// Gets/sets the internal id.
		/// </summary>
		public string InternalId { get; set; }

		/// <summary>
		/// Gets/sets the sequence number.
		/// </summary>
		public int Seqno { get; set; }

		/// <summary>
		/// Gets/sets the display name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the optional description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets the type of the region body.
		/// </summary>
		public string Type { get; set; }
		#endregion
	}
}
