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
using System.Web;

namespace Piranha.Entities
{
	/// <summary>
	/// The site tree entity defines the different site tree structures available
	/// in the current site.
	/// </summary>
	[Serializable]
	public class SiteTree : StandardEntity<SiteTree>
	{
		#region Properties
		/// <summary>
		/// Gets/sets the namespace id.
		/// </summary>
		public Guid NamespaceId { get; set; }

		/// <summary>
		/// Gets/sets the internal id.
		/// </summary>
		public string InternalId { get; set; }

		/// <summary>
		/// Gets/sets the display name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the optional description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets the optional hostnames.
		/// </summary>
		public string HostNames { get; set; }

		/// <summary>
		/// Gets/sets the SEO meta title.
		/// </summary>
		public string MetaTitle { get; set; }

		/// <summary>
		/// Gets/sets the SEO meta description.
		/// </summary>
		public string MetaDescription { get; set; }
		#endregion

		#region Navigation properties
		/// <summary>
		/// Gets/sets the namespace.
		/// </summary>
		public Namespace Namespace { get; set; }
		#endregion
	}
}