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
using System.Web.Caching;

namespace Piranha.Web
{
	/// <summary>
	/// Attribute for marking a property to be automatically handled by the model loader.
	/// </summary>
	public class ModelPropertyAttribute : Attribute
	{
		/// <summary>
		/// Gets/sets the absolute expiration of the cache in minutes.
		/// </summary>
		public int AbsoluteExpiration { get; set; }

		/// <summary>
		/// Gets/sets the sliding expiration of the cache in minutes.
		/// </summary>
		public int SlidingExpiration { get; set; }

		/// <summary>
		/// Gets/sets the cache priority.
		/// </summary>
		public CacheItemPriority Priority { get; set; }

		/// <summary>
		/// Gets/sets the optional method name that should be executed to load the property data.
		/// </summary>
		public string OnLoad { get; set; }

		/// <summary>
		/// Gets/sets whether property should be populated on POST requests. Default value is true.
		/// </summary>
		public bool LoadOnPost { get; set; }

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ModelPropertyAttribute() {
			Priority = CacheItemPriority.Normal;
			LoadOnPost = true;
		}
	}
}