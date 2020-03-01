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
using System.Runtime.Serialization;
using System.Web;

namespace Piranha.Legacy.Services.DataContracts
{
	/// <summary>
	/// Data contract for the page templates.
	/// </summary>
	[DataContract()]
	public class PageTemplate
	{
		/// <summary>
		/// Get/sets the id of the template.
		/// </summary>
		[DataMember()]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets/sets the name.
		/// </summary>
		[DataMember()]
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the optional description.
		/// </summary>
		[DataMember()]
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets the rendering view.
		/// </summary>
		[DataMember()]
		public string View { get; set; }
	}
}